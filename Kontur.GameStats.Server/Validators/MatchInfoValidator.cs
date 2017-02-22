using FluentValidation;
using Kontur.GameStats.Server.Models.Info;

namespace Kontur.GameStats.Server.Validators
{
    internal class MatchInfoValidator : AbstractValidator<MatchInfo>
    {
        public MatchInfoValidator()
        {
            RuleFor(x => x.FragLimit > 0).NotNull();
            RuleFor(x => x.GameMode).NotNull();
            RuleFor(x => x.Map).NotNull();
            RuleFor(x => x.Scoreboard).NotNull();
            RuleFor(x => x.TimeElapsed > 0).NotNull();
            RuleFor(x => x.TimeLimit > 0).NotNull();
        }
    }
}
