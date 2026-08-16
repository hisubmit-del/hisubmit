using FluentValidation;
using FluentValidation.Validators;
using System.Text.RegularExpressions;

namespace Hisubmit.Client.SharedModels.Validators.Extensions.SocialNetwork
{
    public partial class TwitterValidator<T>: PropertyValidator<T,string>,ITwitterValidator
    {
        private const string Pattern = "^([A-Za-z0-9_]+)$";
        private const string ErrorMessage = "Twitter Username is Invalid";

        public override string Name => "TwitterUsernameInvalid";


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

    public interface ITwitterValidator : IPropertyValidator
    {
        
    }
}
