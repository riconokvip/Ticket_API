namespace Ticket.API.Middlewares
{
    public class BaseException : Exception
    {
        public ErrorCodes error { get; set; }
        public string message { get; set; }
        public string errMessage { get; set; }
        public HttpCodes httpCode { get; set; }

        public BaseException(ErrorCodes error, HttpCodes httpCode, string message, string errMessage = null)
        {
            this.error = error;
            this.message = message;
            this.httpCode = httpCode;
            this.errMessage = errMessage != null ? errMessage : message;
        }
    }
}
