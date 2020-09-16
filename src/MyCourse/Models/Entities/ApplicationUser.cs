using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace MyCourse.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public virtual ICollection<Course> AuthoredCourses { get; set; }
    }
}