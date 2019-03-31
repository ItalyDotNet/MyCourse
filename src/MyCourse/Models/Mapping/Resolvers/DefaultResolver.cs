using System;
using System.Data;
using AutoMapper;
using MyCourse.Models.Enums;
using MyCourse.Models.ValueTypes;

namespace MyCourse.Models.Mapping.Resolvers
{
    public class DefaultResolver : IValueResolver<DataRow, object, object>
    {
        private readonly string memberName;
        public DefaultResolver(string memberName)
        {
            this.memberName = memberName;
        }
        public object Resolve(DataRow source, object destination, object destMember, ResolutionContext context)
        {
            return source[memberName];
        }
    }
}