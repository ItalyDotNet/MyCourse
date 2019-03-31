using System;
using System.Data;
using AutoMapper;
using MyCourse.Models.Enums;
using MyCourse.Models.ValueTypes;

namespace MyCourse.Models.Mapping.Resolvers
{
    public class TimeSpanResolver : IValueResolver<DataRow, object, TimeSpan>
    {
        private readonly string memberName;
        public TimeSpanResolver(string memberName)
        {
            this.memberName = memberName;
        }
        public TimeSpan Resolve(DataRow source, object destination, TimeSpan destMember, ResolutionContext context)
        {
            return TimeSpan.Parse((string)source[memberName]);
        }
    }
}