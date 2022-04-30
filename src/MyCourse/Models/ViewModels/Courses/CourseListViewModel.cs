using MyCourse.Models.InputModels.Courses;

namespace MyCourse.Models.ViewModels.Courses;

public class CourseListViewModel : IPaginationInfo
{
    public ListViewModel<CourseViewModel> Courses { get; set; }
    public CourseListInputModel Input { get; set; }


    #region Implementazione IPaginationInfo
    int IPaginationInfo.CurrentPage => Input.Page;

    int IPaginationInfo.TotalResults => Courses.TotalCount;

    int IPaginationInfo.ResultsPerPage => Input.Limit;

    string IPaginationInfo.Search => Input.Search;

    string IPaginationInfo.OrderBy => Input.OrderBy;

    bool IPaginationInfo.Ascending => Input.Ascending;
    #endregion
}
