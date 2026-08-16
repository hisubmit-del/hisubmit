using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Validators;

namespace Hisubmit.Client.SharedModels.Validators.Extensions.SocialNetwork;

public partial class FacebookValidator<T> : PropertyValidator<T, string>, IFaceBookValidator
{
    private const string Pattern = "^([A-Za-z0-9_]+)$";
    private const string ErrorMessage = "Facebook Username is Invalid";
    public override string Name => "FacebookUsernameInvalid";

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

public interface IFaceBookValidator : IPropertyValidator
{

}