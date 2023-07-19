namespace Wheel.Models.Exceptions
{
    public class InvalidRequestException : BaseRequestException
    {
        public InvalidRequestException(string message, int statusCode) : base(message, statusCode) { }
    }
}
