﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

/* --- Builder config --- */
builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

builder.Services.AddEsServices(builder.Configuration);
builder.Services.AddRedisCaches(builder.Configuration);

AddAuthentications(builder);
AddControllers(builder);
AddSwaggers(builder);
AddMapper(builder);
AddAuthorizations(builder);
AddServices(builder);


/* --- Application build --- */
var app = builder.Build();

// Migration database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    new DbInitializer(db).Seeding();
    db.Database.Migrate();
}

// Config HTTP
app.UseSwagger().UseSwaggerUI(options =>
{
    options.DefaultModelsExpandDepth(-1);
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = "swagger";
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

// Status code
app.UseStatusCodePages(async context =>
{
    BaseResponse response = null;
    switch (context.HttpContext.Response.StatusCode)
    {
        case 401:
            response = new BaseResponse()
            {
                code = (int)HttpStatusCode.Unauthorized,
                error = (int)ErrorCodes.UNAUTHORIZED,
                message = ErrorCodes.UNAUTHORIZED.GetEnumMemberValue()
            };
            break;
        case 403:
            response = new BaseResponse()
            {
                code = (int)HttpStatusCode.Forbidden,
                error = (int)ErrorCodes.FORBIDDEN,
                message = ErrorCodes.FORBIDDEN.GetEnumMemberValue()
            };
            break;
        case 404:
            response = new BaseResponse()
            {
                code = (int)HttpStatusCode.NotFound,
                error = (int)ErrorCodes.NOT_FOUND,
                message = ErrorCodes.NOT_FOUND.GetEnumMemberValue()
            };
            break;
        case 405:
            response = new BaseResponse()
            {
                code = (int)HttpStatusCode.MethodNotAllowed,
                error = (int)ErrorCodes.METHOD_NOT_ALLOWED,
                message = ErrorCodes.METHOD_NOT_ALLOWED.GetEnumMemberValue()
            };
            break;
        case 415:
            response = new BaseResponse()
            {
                code = (int)HttpStatusCode.UnsupportedMediaType,
                error = (int)ErrorCodes.UNSUPPORTED_MEDIA_TYPE,
                message = ErrorCodes.UNSUPPORTED_MEDIA_TYPE.GetEnumMemberValue()
            };
            break;
    }

    context.HttpContext.Response.ContentType = "application/json";
    context.HttpContext.Response.StatusCode = 200;
    await context.HttpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
});

app.UseMiddleware<ApplicationMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


// Config authentication
static void AddAuthentications(WebApplicationBuilder builder)
{
    var tokenConfigSection = builder.Configuration.GetSection(JwtTokenConfig.ConfigName) ??
        throw new Exception("Let's check token of config in appsettings.json");

    builder.Services.Configure<JwtTokenConfig>(tokenConfigSection);
    builder.Services.AddSingleton(tokenConfigSection.Get<JwtTokenConfig>());
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddTransient<IJwtHelper, JwtHelper>();

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }
    ).AddJwtBearer(options => {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            ValidAudience = builder.Configuration.GetValue<string>(JwtTokenKeys.JWT_VALID_AUDIENCE),
            ValidIssuer = builder.Configuration.GetValue<string>(JwtTokenKeys.JWT_VALID_ISSUER),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>(JwtTokenKeys.JWT_SECRET)))
        };
    });
}

// Config controller
static void AddControllers(WebApplicationBuilder builder)
{
    builder.Services.Configure<FormOptions>(x =>
    {
        x.ValueLengthLimit = int.MaxValue;
        x.MultipartBodyLengthLimit = int.MaxValue;
    });
    builder.Services.AddControllers(options =>
    {
        options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
    });

    // Custom response
    builder.Services.AddMvc().ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressMapClientErrors = true;
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = string.Join('\n', context.ModelState.Values.Where(v => v.Errors.Count > 0).SelectMany(v => v.Errors).Select(v => v.ErrorMessage));
            var body = new BaseResponse()
            {
                code = (int)HttpStatusCode.BadRequest,
                error = (int)ErrorCodes.BAD_REQUEST,
                message = errors
            };
            var result = new OkObjectResult(body);
            result.ContentTypes.Add(MediaTypeNames.Application.Json);
            result.ContentTypes.Add(MediaTypeNames.Application.Xml);
            return result;
        };
    });
}

// Config swagger
static void AddSwaggers(WebApplicationBuilder builder)
{
    var version = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? "Test";
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = $"Ticket.API, Phiên bản {version}", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,
                },
                new List<string>()
            }
        });

        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

// Config automap
static void AddMapper(WebApplicationBuilder builder)
{
    builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
}

// Add dynamic authorization
static void AddAuthorizations(WebApplicationBuilder builder)
{
    /* --- Ticket policies --- */
    builder.Services.AddTransient<IAuthorizationHandler, GetListTicketAuthorization>();
    builder.Services.AddTransient<IAuthorizationHandler, GetListByUserTicketAuthorization>();
    builder.Services.AddTransient<IAuthorizationHandler, CreateTicketAuthorization>();
    builder.Services.AddTransient<IAuthorizationHandler, UpdateTicketAuthorization>();
    builder.Services.AddTransient<IAuthorizationHandler, DeleteTicketAuthorization>();

    /* --- WorkSpace policies --- */
    builder.Services.AddTransient<IAuthorizationHandler, GetListWorkSpaceAuthorization>();
    builder.Services.AddTransient<IAuthorizationHandler, CreateWorkSpaceAuthorization>();
    builder.Services.AddTransient<IAuthorizationHandler, UpdateWorkSpaceAuthorization>();
    builder.Services.AddTransient<IAuthorizationHandler, DeleteWorkSpaceAuthorization>();

    /* --- Project policies --- */
    builder.Services.AddTransient<IAuthorizationHandler, GetListProjectAuthorization>();
    builder.Services.AddTransient<IAuthorizationHandler, CreateProjectAuthorization>();
    builder.Services.AddTransient<IAuthorizationHandler, UpdateProjectAuthorization>();
    builder.Services.AddTransient<IAuthorizationHandler, DeleteProjectAuthorization>();

    /* --- WorkSpaceMember policies --- */
    builder.Services.AddTransient<IAuthorizationHandler, GetListWorkSpaceMemberAuthorization>();
    builder.Services.AddTransient<IAuthorizationHandler, AddWorkSpaceMemberAuthorization>();
    builder.Services.AddTransient<IAuthorizationHandler, RemoveWorkSpaceMemberAuthorization>();

    /* --- ProjectMember policies --- */
    builder.Services.AddTransient<IAuthorizationHandler, GetListProjectMemberAuthorization>();
    builder.Services.AddTransient<IAuthorizationHandler, AddProjectMemberAuthorization>();
    builder.Services.AddTransient<IAuthorizationHandler, RemoveProjectMemberAuthorization>();

    builder.Services.AddAuthorization(options =>
    {
        /* --- Ticket policies --- */
        options.AddPolicy(ApplicationPermissions.GetListTicket, policy => policy.Requirements.Add(new GetListTicketRequirement()));
        options.AddPolicy(ApplicationPermissions.GetListTicketByUser, policy => policy.Requirements.Add(new GetListByUserTicketRequirement()));
        options.AddPolicy(ApplicationPermissions.CreateTicket, policy => policy.Requirements.Add(new CreateTicketRequirement()));
        options.AddPolicy(ApplicationPermissions.UpdateTicket, policy => policy.Requirements.Add(new UpdateTicketRequirement()));
        options.AddPolicy(ApplicationPermissions.DeleteTicket, policy => policy.Requirements.Add(new DeleteTicketRequirement()));

        /* --- WorkSpace policies --- */
        options.AddPolicy(ApplicationPermissions.GetListWorkSpace, policy => policy.Requirements.Add(new GetListWorkSpaceRequirement()));
        options.AddPolicy(ApplicationPermissions.CreateWorkSpace, policy => policy.Requirements.Add(new CreateWorkSpaceRequirement()));
        options.AddPolicy(ApplicationPermissions.UpdateWorkSpace, policy => policy.Requirements.Add(new UpdateWorkSpaceRequirement()));
        options.AddPolicy(ApplicationPermissions.DeleteWorkSpace, policy => policy.Requirements.Add(new DeleteWorkSpaceRequirement()));

        /* --- Project policies --- */
        options.AddPolicy(ApplicationPermissions.GetListProject, policy => policy.Requirements.Add(new GetListProjectRequirement()));
        options.AddPolicy(ApplicationPermissions.CreateProject, policy => policy.Requirements.Add(new CreateProjectRequirement()));
        options.AddPolicy(ApplicationPermissions.UpdateProject, policy => policy.Requirements.Add(new UpdateProjectRequirement()));
        options.AddPolicy(ApplicationPermissions.DeleteProject, policy => policy.Requirements.Add(new DeleteProjectRequirement()));

        /* --- WorkSpaceMember policies --- */
        options.AddPolicy(ApplicationPermissions.GetListWorkSpaceMember, policy => policy.Requirements.Add(new GetListWorkSpaceMemberRequirement()));
        options.AddPolicy(ApplicationPermissions.AddWorkSpaceMember, policy => policy.Requirements.Add(new AddWorkSpaceMemberRequirement()));
        options.AddPolicy(ApplicationPermissions.RemoveWorkSpaceMember, policy => policy.Requirements.Add(new RemoveWorkSpaceMemberRequirement()));

        /* --- ProjectMember policies --- */
        options.AddPolicy(ApplicationPermissions.GetListProjectMember, policy => policy.Requirements.Add(new GetListProjectMemberRequirement()));
        options.AddPolicy(ApplicationPermissions.AddProjectMember, policy => policy.Requirements.Add(new AddProjectMemberRequirement()));
        options.AddPolicy(ApplicationPermissions.RemoveProjectMember, policy => policy.Requirements.Add(new RemoveProjectMemberRequirement()));
    });
}

// Register service
static void AddServices(WebApplicationBuilder builder)
{
    // Middleware
    builder.Services.AddTransient<ApplicationMiddleware>();

    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IWorkSpaceService, WorkSpaceService>();
    builder.Services.AddScoped<IProjectService, ProjectService>();
    builder.Services.AddScoped<IWorkSpaceMemberService, WorkSpaceMemberService>();
    builder.Services.AddScoped<IProjectMemberService, ProjectMemberService>();
    builder.Services.AddScoped<ITicketService, TicketService>();

    builder.Services.AddScoped<IUserService, UserService>();
}