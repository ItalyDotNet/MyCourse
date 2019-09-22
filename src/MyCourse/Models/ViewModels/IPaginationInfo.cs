namespace MyCourse.Models.ViewModels
{
    public interface IPaginationInfo
    {
        int CurrentPage { get; }
        int TotalResults { get; }
        int ResultsPerPage { get; }
        
        string Search { get; }
        string OrderBy { get; }
        bool Ascending { get; }
    }
}