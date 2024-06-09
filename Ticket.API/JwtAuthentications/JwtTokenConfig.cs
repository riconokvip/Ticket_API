namespace Ticket.API.JwtAuthentications
{
    public class JwtTokenConfig
    {
        public const string ConfigName = "JWT";
        public string ValidAudience { get; set; }
        public string ValidIssuer { get; set; }
        public string Secret { get; set; }
        public int TokenValidityInMinutes { get; set; }
        public int RefreshTokenValidityInDays { get; set; }
    }

    public class JwtTokenKeys
    {
        public static string JWT_SECRET { get { return "JWT:Secret"; } }
        public static string JWT_VALID_ISSUER { get { return "JWT:ValidIssuer"; } }
        public static string JWT_VALID_AUDIENCE { get { return "JWT:ValidAudience"; } }
        public static string JWT_TOKEN_VALIDITY_IN_MINUTES { get { return "JWT:TokenValidityInMinutes"; } }
        public static string JWT_REFRESH_TOKEN_VALIDITY_IN_DAYS { get { return "JWT:RefreshTokenValidityInDays"; } }
    }
}
