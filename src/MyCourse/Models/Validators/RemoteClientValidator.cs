using System.Linq;
using FluentValidation.AspNetCore;
using FluentValidation.Internal;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MyCourse.Models.Validators
{   
    public class RemoteClientValidator : ClientValidatorBase
    {
        public RemoteClientValidator(PropertyRule rule, IPropertyValidator validator) : base(rule, validator)
        {
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            var validator = (RemotePropertyValidator) Validator;
            var additionalFields = string.Join(",", (new[] { context.ModelMetadata.PropertyName }).Union(validator.AdditionalFields).Select(field => $"*.{field}"));
            MergeAttribute(context.Attributes, "data-val-remote", validator.ErrorText);
            MergeAttribute(context.Attributes, "data-val-remote-url", validator.Url);
            MergeAttribute(context.Attributes, "data-val-remote-additionalfields", additionalFields);
        }
    }
}