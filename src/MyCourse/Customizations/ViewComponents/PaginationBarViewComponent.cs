using Microsoft.AspNetCore.Mvc;
using MyCourse.Models.ViewModels;

namespace MyCourse.Customizations.ViewComponents
{
    public class PaginationBarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(CourseListViewModel model)
        {
            //Il numero di pagina corrente
            //Il numero di risultati totali
            //Il numero di risultati per pagina
            return View(model);
        }
    }
}