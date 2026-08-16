using FluentValidation;
using FluentValidation.Validators;
using System.Text.RegularExpressions;

namespace Hisubmit.Client.SharedModels.Validators.Extensions.SocialNetwork
{
    public partial class SiteUrlValidator<T> : PropertyValidator<T, string>, ISiteUrlValidator
    {
        private const string Pattern = @"^" +
                                       @"(?:https?:\/\/)?" +  // پروتکل اختیاری
                                       @"(?:www\.)?" +       // www اختیاری
                                       @"[a-z0-9-A-Z-]+" +       // نام دامنه
                                       @"\.[a-z]{2,63}" +    // پسوند دامنه (الزامی)
                                       @"(?::\d+)?" +        // پورت اختیاری
                                       @"(?:/[\p{L}0-9-._~:/?#[\]@!$&'()*+,;%=]*)?" + // مسیر و پارامترهای اختیاری
                                       @"$";

        private const string ErrorMessage = "Website url is invalid";
        public override string Name => "WebsiteUrlIsInvalid";

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

    public interface ISiteUrlValidator : IPropertyValidator
    {

    }
}
