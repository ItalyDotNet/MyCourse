using System;

namespace MyCourse.Models.Exceptions.Application
{
    public class SendException : Exception
    {
        public SendException() : base($"Couldn't send the message")
        {
        }
    }
}