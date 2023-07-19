namespace Wheel.Models.Exceptions
{
    public class GameAlreadyActivatedException : BaseRequestException
    {
        public GameAlreadyActivatedException(string message, int statusCode) : base(message, statusCode) { }
    }
}
