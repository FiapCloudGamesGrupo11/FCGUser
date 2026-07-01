using FluentValidation;
using UserAPI.Application.DTOs.Request;

namespace UserAPI.Application.Validators
{
    public class UserUpdateValidator : AbstractValidator<UserRequestUpdateView>
    {
        public UserUpdateValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Nome é obrigatório.")
                .Must(HasRealCharacter).WithMessage("Não deve ser vazio.")
                .Length(2, 50).WithMessage("O nome deve conter entre 2 e 50 caracteres.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Sobrenome é obrigatório.")
                .Must(HasRealCharacter).WithMessage("Não deve conter espaços vazios")
                .Length(2, 50).WithMessage("O sobrenome deve conter entre 2 e 50 caracteres.");
        }

        private static bool HasRealCharacter(string value)
        {
            if (value == null) return false;
            return value.Any(c => !char.IsWhiteSpace(c));
        }
    }
}
