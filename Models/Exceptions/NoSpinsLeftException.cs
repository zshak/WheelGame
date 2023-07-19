namespace Wheel.Models.Exceptions
{
    public class NoSpinsLeftException : BaseRequestException
    {
        public NoSpinsLeftException(string message, int statusCode) : base(message, statusCode) { }
    }
}
