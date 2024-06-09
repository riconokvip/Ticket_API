using System.Net;

namespace Ticket.API.Middlewares
{
    public class ApplicationMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (BaseException ex)
            {
                var response = new BaseResponse
                {
                    code = (int)ex.httpCode,
                    error = (int)ex.error,
                    message = ex.message
                };
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            catch (Exception ex)
            {
                var response = new BaseResponse
                {
                    code = (int)HttpCodes.INTERNAL_SERVER_ERROR,
                    error = (int)ErrorCodes.ERROR,
                    message = ErrorCodes.ERROR.GetEnumMemberValue(),
                    errorMessage = ex.Message
                };
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
