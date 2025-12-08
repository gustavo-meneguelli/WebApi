using Application.DTO.Auth;
using Application.Interfaces.Repositories;
using Domain.Constants;
using FluentValidation;

namespace Application.Validators;

public class UserRegisterDtoValidator : AbstractValidator<UserRegisterDto>
{
    public UserRegisterDtoValidator(IUserRepository userRepository)
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("O nome de usuário é obrigatório.")
            .MinimumLength(3).WithMessage("O nome de usuário deve ter no mínimo 3 caracteres.")
            .MaximumLength(50).WithMessage("O nome de usuário deve ter no máximo 50 caracteres.");

        RuleFor(x => x.Username)
            .Cascade(CascadeMode.Stop)
            .MustAsync(async (username, _) =>
            {
                var user = await userRepository.GetUserByUsernameAsync(username);
                return user is null;
            })
            .WithMessage(string.Format(ErrorMessages.AlreadyExists, "usuário", "username"));

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A senha é obrigatória.")
            .MinimumLength(6).WithMessage("A senha deve ter no mínimo 6 caracteres.")
            .MaximumLength(100).WithMessage("A senha deve ter no máximo 100 caracteres.");
    }
}
