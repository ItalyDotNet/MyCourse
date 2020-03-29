using System;
using System.Collections.Generic;
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

        protected override bool IsValid(PropertyValidatorContext context)
        {
            return true;
        }
    }
}