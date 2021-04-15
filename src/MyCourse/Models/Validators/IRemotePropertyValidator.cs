using System.Collections.Generic;

namespace MyCourse.Models.Validators
{
    public interface IRemotePropertyValidator
    {
        string Url { get; }
        string ErrorText { get; }
        IEnumerable<string> AdditionalFields { get; }
    }
}