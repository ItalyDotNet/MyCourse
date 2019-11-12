using System.Data;
using AutoMapper;
using MyCourse.Models.Mapping.Resolvers;
using MyCourse.Models.ViewModels;

namespace MyCourse.Models.Mapping
{
    public class AdoNetCourseProfile : Profile
    {
        public AdoNetCourseProfile()
        {
            CreateMap<DataRow, CourseViewModel>()
                .ForMember(viewModel => viewModel.Id, config => config.MapFrom(IdResolver.Instance))
                .ForMember(viewModel => viewModel.CurrentPrice, config => config.MapFrom(MoneyResolver.Instance, dataRow => "CurrentPrice"))
                .ForMember(viewModel => viewModel.FullPrice, config => config.MapFrom(MoneyResolver.Instance, dataRow => "FullPrice"))
                .ForAllOtherMembers(config => config.MapFrom(DefaultResolver.Instance, dataRow => config.DestinationMember.Name));

            CreateMap<DataRow, CourseDetailViewModel>()
                //.IncludeBase<DataRow, CourseViewModel>()
                .ForMember(viewModel => viewModel.Id, config => config.MapFrom(IdResolver.Instance))
                .ForMember(viewModel => viewModel.CurrentPrice, config => config.MapFrom(MoneyResolver.Instance, dataRow => "CurrentPrice"))
                .ForMember(viewModel => viewModel.FullPrice, config => config.MapFrom(MoneyResolver.Instance, dataRow => "FullPrice"))
                .ForMember(viewModel => viewModel.Lessons, config => config.Ignore())
                .ForMember(viewModel => viewModel.TotalCourseDuration, config => config.Ignore())
                .ForAllOtherMembers(config => config.MapFrom(DefaultResolver.Instance, dataRow => config.DestinationMember.Name));

            CreateMap<DataRow, LessonViewModel>()
                .ForMember(viewModel => viewModel.Id, config => config.MapFrom(IdResolver.Instance))
                .ForMember(viewModel => viewModel.Duration, config => config.MapFrom(TimeSpanResolver.Instance, dataRow => "Duration"))
                .ForAllOtherMembers(config => config.MapFrom(DefaultResolver.Instance, dataRow => config.DestinationMember.Name));
        }
    }
}