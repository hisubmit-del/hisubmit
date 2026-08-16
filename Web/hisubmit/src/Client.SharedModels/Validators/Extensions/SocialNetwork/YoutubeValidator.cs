using FluentValidation;
using FluentValidation.Validators;
using System.Text.RegularExpressions;

namespace Hisubmit.Client.SharedModels.Validators.Extensions.SocialNetwork
{
    public partial class YoutubeValidator<T>:PropertyValidator<T,string>,IYoutubeValidator
    {
        private const string Pattern = "^([A-Za-z0-9_]+)$";
        private const string ErrorMessage = "Youtube channel name is Invalid";

        public override string Name => "YoutubeChannelNameInvalid";


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

    public interface IYoutubeValidator : IPropertyValidator
    {

    }
}
