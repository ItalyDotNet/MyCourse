using System;

namespace MyCourse.Models.Exceptions.Application
{
    public class UserUnknownException : Exception
    {
        public UserUnknownException() : base($"A known user is required for this operation")
        {
        }
    }
}