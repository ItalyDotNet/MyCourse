using FluentValidation;

namespace MyCourse.Models.Validators
{
    public static class ValidatorExtensions
    {
        //Reusable property validators
        //https://docs.fluentvalidation.net/en/latest/custom-validators.html#reusable-property-validators
        public static IRuleBuilderOptions<T, TElement> Remote<T, TElement>(this IRuleBuilder<T, TElement> ruleBuilder, string url, string additionalFields, string errorText = "") {
            return ruleBuilder.SetValidator(new RemotePropertyValidator(url, additionalFields, errorText));
        }
    }
}