namespace Ticket.API.Models
{
    public class JwtResponse
    {
        public string Token { get; set; }
        public DateTime AccessTokenExpriesAt { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expiration { get; set; }
    }
}
