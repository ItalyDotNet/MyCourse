using System;
using System.Data;
using AutoMapper;
using MyCourse.Models.Enums;
using MyCourse.Models.ValueTypes;

namespace MyCourse.Models.Mapping.Resolvers
{
    public class IdResolver : IValueResolver<DataRow, object, int>
    {
        public int Resolve(DataRow source, object destination, int destMember, ResolutionContext context)
        {
            return Convert.ToInt32(source["Id"]);
        }

        private static Lazy<IdResolver> instance = new Lazy<IdResolver>(() => new IdResolver());
        public static IdResolver Instance => instance.Value;
    }
}