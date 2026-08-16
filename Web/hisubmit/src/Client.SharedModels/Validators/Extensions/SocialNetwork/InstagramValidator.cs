using FluentValidation.Validators;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Hisubmit.Client.SharedModels.Validators.Extensions.SocialNetwork;

public partial class InstagramValidator<T> : PropertyValidator<T, string>, IInstagramValidator
{
    private const string Pattern = "^([A-Za-z0-9_]+)$";
    private const string ErrorMessage = "Telegram Username is Invalid";
    public override string Name => "TelegramUsernameInvalid";

    public override bool IsValid(ValidationContext<T> context, string value)
    {
        return string.IsNullOrWhiteSpace(value) || MyRegex().IsMatch(value);
    }
    protected override string GetDefaultMessageTemplate(string errorCode)
    {
        return ErrorMessage;
    }

    [GeneratedRegex(Pattern)]
    private static partial Regex MyRegex();
}

public interface IInstagramValidator : IPropertyValidator
{

}