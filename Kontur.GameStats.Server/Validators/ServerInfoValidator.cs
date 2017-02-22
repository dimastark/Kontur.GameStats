using FluentValidation;
using Kontur.GameStats.Server.Models.Info;

namespace Kontur.GameStats.Server.Validators
{
    public class ServerInfoValidator : AbstractValidator<ServerInfo>
    {
        public ServerInfoValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.GameModes).NotNull();
        }
    }
}
