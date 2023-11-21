using MyBlogs.Services;
using FluentValidation;
using MyBlogs.Models;
using MyBlogs.DTOModels;

namespace MyBlogs.Validations;
public class UserRegisterValidaton : AbstractValidator<UsersPostDTO>
{
    private readonly UsersService _userService;
    public UserRegisterValidaton(UsersService usersService)
    {
        _userService = usersService;
        RuleFor(x => x.Username)
            .Must(UsernameUniqueValidator)
            .WithMessage("Username already exists");

        RuleFor(x => x.Email)
            .Must(EmailUniqueValidator)
            .WithMessage("Email already exists")
            .EmailAddress()
            .WithMessage("Enter a valid email address");

        RuleFor(x => x.Phone)
            .Must(PhoneUniqueValidator)
            .WithMessage("Phone already exists")
            .MinimumLength(10)
            .WithMessage("Enter a valid phone number")
            .MaximumLength(10)
            .WithMessage("Enter a valid phone number");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Your password cannot be empty.")
            .MinimumLength(8)
            .WithMessage("Your Password must be atleast 8 characters.")
            .Matches(@"[A-Z]+")
            .WithMessage("Your password must contain at least one uppercase letter.")
            .Matches(@"[a-z]+")
            .WithMessage("Your password must contain at least one lowercase letter.")
            .Matches(@"[0-9]+")
            .WithMessage("Your password must contain at least one numeric value.")
            .Matches(@"[\!\?\*\.\@\#\$\%\&\^\*]+")
            .WithMessage("Your password must contain at least one special character.");
    }

    private bool EmailUniqueValidator(string email)
    {
        var exists = _userService.EmailExists(email);
        return !exists.Result;
    }

    private bool UsernameUniqueValidator(string username)
    {
        var exists = _userService.UsernameExists(username);
        return !exists.Result;
    }

    private bool PhoneUniqueValidator(string phone)
    {
        var exists = _userService.PhoneExists(phone);
        return !exists.Result;
    }
}


