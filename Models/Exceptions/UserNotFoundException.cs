namespace Wheel.Models.Exceptions
{
    public class UserNotFoundException : BaseRequestException
    {
        public UserNotFoundException(string message, int statusCode) : base(message, statusCode) { }
    }
}
