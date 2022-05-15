using System.Data;
using Microsoft.AspNetCore.Identity;

namespace MyCourse.Models.Entities;

public class ApplicationUser : IdentityUser
{
    [PersonalData]
    public string FullName { get; set; }
    [PersonalData]
    public DateTimeOffset EcommerceConsent { get; set; }
    [PersonalData]
    public DateTimeOffset? NewsletterConsent { get; set; }
    public virtual ICollection<Course> AuthoredCourses { get; set; }
    public virtual ICollection<Course> SubscribedCourses { get; set; }

    public static ApplicationUser FromDataRow(DataRow userRow)
    {
        ApplicationUser applicationUser = new()
        {
            Id = Convert.ToString(userRow["Id"]),
            UserName = Convert.ToString(userRow["UserName"]),
            NormalizedUserName = Convert.ToString(userRow["NormalizedUserName"]),
            Email = Convert.ToString(userRow["Email"]),
            NormalizedEmail = Convert.ToString(userRow["NormalizedEmail"]),
            EmailConfirmed = Convert.ToBoolean(userRow["EmailConfirmed"]),
            PasswordHash = Convert.ToString(userRow["PasswordHash"]),
            SecurityStamp = Convert.ToString(userRow["SecurityStamp"]),
            ConcurrencyStamp = Convert.ToString(userRow["ConcurrencyStamp"]),
            PhoneNumber = Convert.ToString(userRow["PhoneNumber"]),
            PhoneNumberConfirmed = Convert.ToBoolean(userRow["PhoneNumberConfirmed"]),
            TwoFactorEnabled = Convert.ToBoolean(userRow["TwoFactorEnabled"]),
            LockoutEnd = (userRow["LockoutEnd"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(userRow["LockoutEnd"])),
            LockoutEnabled = Convert.ToBoolean(userRow["LockoutEnabled"]),
            AccessFailedCount = Convert.ToInt32(userRow["AccessFailedCount"]),
            FullName = Convert.ToString(userRow["FullName"])
        };
        return applicationUser;
    }
}
