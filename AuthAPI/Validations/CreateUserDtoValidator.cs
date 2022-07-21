using Auth.Core.DTOs;
using FluentValidation;

namespace AuthAPI.Validations
{
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is requier").EmailAddress().WithMessage("Email is wrong");

            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is requier");
            RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName is requier");
        }
    }
}
