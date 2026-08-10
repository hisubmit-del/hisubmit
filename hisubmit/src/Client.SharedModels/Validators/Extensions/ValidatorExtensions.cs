using FluentValidation;
using FluentValidation.Validators;
using Hisubmit.Client.SharedModels.Validators.Extensions.SocialNetwork;
using Hisubmit.Client.SharedModels.Validators.Features.Projects;

namespace Hisubmit.Client.SharedModels.Validators.Extensions
{
    public static class ValidatorExtensions
    {
        public static IRuleBuilderOptions<T, string> MustBeJson<T>(this IRuleBuilder<T, string> ruleBuilder, IPropertyValidator<T, string> validator) where T : class
        {
            return ruleBuilder.SetValidator(validator);
        }

        public static IRuleBuilderOptions<T, string> InstagramUsername<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new InstagramValidator<T>());
        }
        public static IRuleBuilderOptions<T, string> TelegramUsername<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new TelegramValidator<T>());
        }
        public static IRuleBuilderOptions<T, string> TwitterUsername<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new TwitterValidator<T>());
        }
        public static IRuleBuilderOptions<T, string> FacebookUsername<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new FacebookValidator<T>());
        }
        public static IRuleBuilderOptions<T, string> YoutubeChannelName<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new YoutubeValidator<T>());
        }
        public static IRuleBuilderOptions<T, string> SiteUrl<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new SiteUrlValidator<T>());
        }
        public static IRuleBuilderOptions<T, string> WhatsAppNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new WhatsAppValidator<T>());
        }
    }   
}