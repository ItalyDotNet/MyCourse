using System;
using System.Data;
using AutoMapper;

namespace MyCourse.Models.Mapping.Resolvers
{
    public class TimeSpanResolver : IMemberValueResolver<DataRow, object, string, TimeSpan>
    {
        public TimeSpan Resolve(DataRow source, object destination, string sourceMember, TimeSpan destMember, ResolutionContext context)
        {
            return TimeSpan.Parse((string)source[sourceMember]);
        }

        private static Lazy<TimeSpanResolver> instance = new Lazy<TimeSpanResolver>(() => new TimeSpanResolver());
        public static TimeSpanResolver Instance => instance.Value;
    }
}