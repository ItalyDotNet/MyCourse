using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Identity;

namespace MyCourse.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public virtual ICollection<Course> AuthoredCourses { get; set; }

        public static ApplicationUser FromDataRow(DataRow courseRow)
        {
            var applicationUser = new ApplicationUser
            {
                Id = Convert.ToString(courseRow["Id"]),
                UserName = Convert.ToString(courseRow["UserName"]),
                NormalizedUserName = Convert.ToString(courseRow["NormalizedUserName"]),
                Email = Convert.ToString(courseRow["Email"]),
                NormalizedEmail = Convert.ToString(courseRow["NormalizedEmail"]),
                EmailConfirmed = Convert.ToBoolean(courseRow["EmailConfirmed"]),
                PasswordHash = Convert.ToString(courseRow["PasswordHash"]),
                SecurityStamp = Convert.ToString(courseRow["SecurityStamp"]),
                ConcurrencyStamp = Convert.ToString(courseRow["ConcurrencyStamp"]),
                PhoneNumber = Convert.ToString(courseRow["PhoneNumber"]),
                PhoneNumberConfirmed = Convert.ToBoolean(courseRow["PhoneNumberConfirmed"]),
                TwoFactorEnabled = Convert.ToBoolean(courseRow["TwoFactorEnabled"]),
                LockoutEnd = (courseRow["LockoutEnd"] == DBNull.Value ? (DateTime?) null : Convert.ToDateTime(courseRow["LockoutEnd"])),
                LockoutEnabled = Convert.ToBoolean(courseRow["LockoutEnabled"]),
                AccessFailedCount = Convert.ToInt32(courseRow["AccessFailedCount"]),
                FullName = Convert.ToString(courseRow["FullName"])
            };
            return applicationUser;
        }
    }
}