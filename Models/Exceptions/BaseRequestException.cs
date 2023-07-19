namespace Wheel.Models.Exceptions
{
    public class BaseRequestException : Exception
    {
        public int statusCode {  get; set; }
        public BaseRequestException(string message, int statusCode) : base(message)
        {
            this.statusCode = statusCode;
        }
    }
}
