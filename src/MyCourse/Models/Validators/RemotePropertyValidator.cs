using System;
using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;

namespace MyCourse.Models.Validators
{
    public class RemotePropertyValidator : PropertyValidator
    {
        public RemotePropertyValidator(string url, string additionalFields, string errorText = "") : base(errorText)
        {
            Url = url;
            AdditionalFields = (additionalFields ?? "").Split(",", StringSplitOptions.RemoveEmptyEntries);
            ErrorText = errorText;
        }

        public string Url { get; }
        public string ErrorText { get; }
        public IEnumerable<string> AdditionalFields { get; }


        public override bool ShouldValidateAsync(ValidationContext context)
        {
            return false;
        }


        public override IEnumerable<ValidationFailure> Validate(PropertyValidatorContext context)
        {
            return base.Validate(context);
        }


        protected override bool IsValid(PropertyValidatorContext context)
        {
            return true;
        }
    }
}