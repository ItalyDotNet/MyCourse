using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MyCourse.Models.Entities;
using MyCourse.Models.Enums;

namespace MyCourse.Models.Services.Infrastructure
{
    public partial class MyCourseDbContext : IdentityDbContext<ApplicationUser>
    {

        public MyCourseDbContext(DbContextOptions<MyCourseDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Lesson> Lessons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("Courses"); //Superfluo se la tabella si chiama come la proprietà che espone il DbSet
                entity.HasKey(course => course.Id); //Superfluo se la proprietà si chiama Id oppure CourseId
                //entity.HasKey(course => new { course.Id, course.Author }); //Per chiavi primarie composite (è importante rispettare l'ordine dei campi)

                entity.HasIndex(course => course.Title).IsUnique();
                entity.Property(course => course.RowVersion).IsRowVersion();
                entity.Property(course => course.Status).HasConversion<string>();

                //Mapping per gli owned types
                entity.OwnsOne(course => course.CurrentPrice, builder =>
                {
                    builder.Property(money => money.Currency)
                           .HasConversion<string>()
                           .HasColumnName("CurrentPrice_Currency"); //Superfluo perché le nostre colonne seguono già la convenzione di nomi
                    builder.Property(money => money.Amount)
                           .HasColumnName("CurrentPrice_Amount")//Superfluo perché le nostre colonne seguono già la convenzione di nomi
                           .HasConversion<float>(); //Questo indica al meccanismo delle migration che la colonna della tabella dovrà essere creata di tipo numerico
                });

                entity.OwnsOne(course => course.FullPrice, builder =>
                {
                    builder.Property(money => money.Currency)
                           .HasConversion<string>();
                    builder.Property(money => money.Amount)
                           .HasConversion<float>(); //Questo indica al meccanismo delle migration che la colonna della tabella dovrà essere creata di tipo numerico
                });

                //Mapping per le relazioni
                entity.HasOne(course => course.AuthorUser)
                      .WithMany(user => user.AuthoredCourses)
                      .HasForeignKey(course => course.AuthorId);

                entity.HasMany(course => course.Lessons)
                      .WithOne(lesson => lesson.Course)
                      .HasForeignKey(lesson => lesson.CourseId); //Superflua se la proprietà si chiama CourseId

                entity.HasMany(course => course.SubscribedUsers)
                      .WithMany(user => user.SubscribedCourses)
                      .UsingEntity<Subscription>(
                            entity => entity.HasOne(subscription => subscription.User).WithMany().HasForeignKey(courseStudent => courseStudent.UserId),
                            entity => entity.HasOne(subscription => subscription.Course).WithMany().HasForeignKey(courseStudent => courseStudent.CourseId),
                            entity =>
                            {
                                entity.ToTable("Subscriptions");
                                entity.OwnsOne(subscription => subscription.Paid, builder =>
                                {
                                    builder.Property(money => money.Currency)
                                           .HasConversion<string>();
                                    builder.Property(money => money.Amount)
                                           .HasConversion<float>();
                                });
                            }
                );

                //Global Query Filter
                entity.HasQueryFilter(course => course.Status != CourseStatus.Deleted);

                #region Mapping generato automaticamente dal tool di reverse engineering
                /*
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Author)
                    .IsRequired()
                    .HasColumnType("TEXT (100)");

                entity.Property(e => e.CurrentPriceAmount)
                    .IsRequired()
                    .HasColumnName("CurrentPrice_Amount")
                    .HasColumnType("NUMERIC")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.CurrentPriceCurrency)
                    .IsRequired()
                    .HasColumnName("CurrentPrice_Currency")
                    .HasColumnType("TEXT (3)")
                    .HasDefaultValueSql("'EUR'");

                entity.Property(e => e.Description).HasColumnType("TEXT (10000)");

                entity.Property(e => e.Email).HasColumnType("TEXT (100)");

                entity.Property(e => e.FullPriceAmount)
                    .IsRequired()
                    .HasColumnName("FullPrice_Amount")
                    .HasColumnType("NUMERIC")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.FullPriceCurrency)
                    .IsRequired()
                    .HasColumnName("FullPrice_Currency")
                    .HasColumnType("TEXT (3)")
                    .HasDefaultValueSql("'EUR'");

                entity.Property(e => e.ImagePath).HasColumnType("TEXT (100)");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnType("TEXT (100)");
                    */
                #endregion
            });

            modelBuilder.Entity<Lesson>(entity =>
            {
                entity.Property(lesson => lesson.RowVersion).IsRowVersion();
                entity.Property(lesson => lesson.Order).HasDefaultValue(1000).ValueGeneratedNever();

                #region Mapping generato automaticamente dal tool di reverse engineering
                /*
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description).HasColumnType("TEXT (10000)");

                entity.Property(e => e.Duration)
                    .IsRequired()
                    .HasColumnType("TEXT (8)")
                    .HasDefaultValueSql("'00:00:00'");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnType("TEXT (100)");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Lessons)
                    .HasForeignKey(d => d.CourseId);
                */
                #endregion
            });
        }
    }
}
