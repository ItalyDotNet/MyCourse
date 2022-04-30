using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MyCourse.Models.Services.Infrastructure;

public partial class MyCourseDbContext : IdentityDbContext<ApplicationUser>
{

    public MyCourseDbContext(DbContextOptions<MyCourseDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Course> Courses { get; set; }
    public virtual DbSet<Lesson> Lessons { get; set; }
    public virtual DbSet<Subscription> Subscriptions { get; set; }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Pre-convention model configuration goes here
        configurationBuilder.Properties<Currency>().HaveConversion<string>();
        configurationBuilder.Properties<decimal>().HaveConversion<float>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //Mapping per gli owned types
        modelBuilder.Owned<Money>();

        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("Courses"); //Superfluo se la tabella si chiama come la proprietà che espone il DbSet
            entity.HasKey(course => course.Id); //Superfluo se la proprietà si chiama Id oppure CourseId
                                                //entity.HasKey(course => new { course.Id, course.Author }); //Per chiavi primarie composite (è importante rispettare l'ordine dei campi)

            entity.HasIndex(course => course.Title).IsUnique();
            entity.Property(course => course.RowVersion).IsRowVersion();
            entity.Property(course => course.Status).HasConversion<string>();

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
                        entity =>
                        {
                            entity.HasQueryFilter(subscription => subscription.Course.Status != CourseStatus.Deleted);
                            entity.ToTable("Subscriptions");
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
            entity.HasQueryFilter(lesson => lesson.Course.Status != CourseStatus.Deleted);

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
