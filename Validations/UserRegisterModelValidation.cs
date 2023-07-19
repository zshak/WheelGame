using FluentValidation;
using Wheel.Models;

namespace Wheel.Validations
{
    public class UserRegisterModelValidation : AbstractValidator<UserRegisterModel>
    {
        public UserRegisterModelValidation()
        {
            RuleFor(UserRegisterModel => UserRegisterModel.Name).Must(x => isValidName(x)).WithMessage("Invalid Name");
            RuleFor(UserRegisterModel => UserRegisterModel.Password).Must(x => isValidName(x)).WithMessage("Invalid Password").
                Must(x => x.Length >= 8).WithMessage("Password must be longer than 8 symbols");
            RuleFor(UserRegisterModel => UserRegisterModel.UserName).Must(x => isValidName(x)).WithMessage("Invalid Name");
            RuleFor(UserRegisterModel => UserRegisterModel.BirthDate).
                NotEmpty().WithMessage("Invalid Birth Date").
                Must(x => isAdult(x)).WithMessage("garibashvilma arao");
        }

        private static bool isAdult(DateTime x)
        {
            TimeSpan t = DateTime.Now.Subtract(x);
            int years = (int)(t.TotalDays / 365.25);
            return years >= 25;
        }

        private static bool isValidName(string name)
        {
            return !string.IsNullOrEmpty(name);
        }
    }
}
