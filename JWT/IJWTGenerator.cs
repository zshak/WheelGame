using System.CodeDom.Compiler;
using Wheel.Models;

namespace Wheel.JWT
{
    public interface IJWTGenerator
    {
        string GenerateToken(UserLoginModel user, int id);
    }
}
