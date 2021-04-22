using System;
using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Validators;

namespace MyCourse.Models.Validators
{
    public class RemotePropertyValidator<T, TProperty> : PropertyValidator<T, TProperty>, IRemotePropertyValidator
    {
        public RemotePropertyValidator(string url, string additionalFields, string errorText = "")
        {
            Url = url;
            AdditionalFields = (additionalFields ?? "").Split(",", StringSplitOptions.RemoveEmptyEntries);
            ErrorText = errorText;
        }

        public string Url { get; }
        public string ErrorText { get; }
        public IEnumerable<string> AdditionalFields { get; }
        public override string Name => "RemotePropertyValidator";

        public override bool IsValid(ValidationContext<T> context, TProperty value)
        {
            return true;
        }
    }
}