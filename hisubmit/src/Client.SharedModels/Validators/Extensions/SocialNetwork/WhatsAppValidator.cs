using FluentValidation;
using FluentValidation.Validators;
using System.Text.RegularExpressions;

namespace Hisubmit.Client.SharedModels.Validators.Extensions.SocialNetwork
{
    public partial class WhatsAppValidator<T> : PropertyValidator<T, string>, IWhatsAppValidator
    {
        public override string Name => "WhatsAppNumberInvalid";

        private const string Pattern= @"^\+(?:[0-9]●?){6,14}[0-9]$";
        private const string ErrorMessage = "Whatsapp number is invalid ";
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

    public interface IWhatsAppValidator : IPropertyValidator
    {

    }
}
