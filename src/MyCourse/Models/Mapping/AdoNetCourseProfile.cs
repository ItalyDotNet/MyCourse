using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using AutoMapper;
using MyCourse.Models.Enums;
using MyCourse.Models.ValueTypes;
using MyCourse.Models.ViewModels;

namespace MyCourse.Models.Mapping
{
    public class AdoNetCourseProfile : Profile
    {
        public AdoNetCourseProfile()
        {
            CreateMap<DataRow, CourseViewModel>()
                .ForMember(viewModel => viewModel.Id, config => config.MapFrom(new IdResolver()))
                .ForMember(viewModel => viewModel.CurrentPrice, config => config.MapFrom(new MoneyResolver("CurrentPrice")))
                .ForMember(viewModel => viewModel.FullPrice, config => config.MapFrom(new MoneyResolver("FullPrice")))
                .ForAllOtherMembers(config => config.MapFrom(new DefaultResolver(config.DestinationMember.Name)));

            CreateMap<DataRow, CourseDetailViewModel>()
                .ForMember(viewModel => viewModel.Id, config => config.MapFrom(new IdResolver()))
                .ForMember(viewModel => viewModel.CurrentPrice, config => config.MapFrom(new MoneyResolver("CurrentPrice")))
                .ForMember(viewModel => viewModel.FullPrice, config => config.MapFrom(new MoneyResolver("FullPrice")))
                .ForMember(viewModel => viewModel.Lessons, config => config.Ignore())
                .ForMember(viewModel => viewModel.TotalCourseDuration, config => config.Ignore())
                .ForAllOtherMembers(config => config.MapFrom(new DefaultResolver(config.DestinationMember.Name)));

            CreateMap<DataRow, LessonViewModel>()
                .ForMember(viewModel => viewModel.Id, config => config.MapFrom(new IdResolver()))
                .ForMember(viewModel => viewModel.Duration, config => config.MapFrom(new TimeSpanResolver("Duration")))
                .ForAllOtherMembers(config => config.MapFrom(new DefaultResolver(config.DestinationMember.Name)));
        }
    }

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

    public class TimeSpanResolver : IValueResolver<DataRow, object, TimeSpan>
    {
        private readonly string memberName;
        public TimeSpanResolver(string memberName)
        {
            this.memberName = memberName;
        }
        public TimeSpan Resolve(DataRow source, object destination, TimeSpan destMember, ResolutionContext context)
        {
            return TimeSpan.Parse((string) source[memberName]);
        }
    }

    public class IdResolver : IValueResolver<DataRow, object, int>
    {
        public int Resolve(DataRow source, object destination, int destMember, ResolutionContext context)
        {
            return Convert.ToInt32(source["Id"]);
        }
    }

    public class MoneyResolver : IValueResolver<DataRow, object, Money>
    {
        private readonly string prefix;
        public MoneyResolver(string prefix)
        {
            this.prefix = prefix;
        }
        public Money Resolve(DataRow source, object destination, Money destMember, ResolutionContext context)
        {
            string currencyColumnName = $"{prefix}_Currency";
            string amountColumnName = $"{prefix}_Amount";

            Currency currency = Enum.Parse<Currency>(System.Convert.ToString(source[currencyColumnName]));
            decimal amount = System.Convert.ToDecimal(source[amountColumnName]);

            return new Money(currency, amount);
        }
    }
}