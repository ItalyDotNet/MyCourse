using MyCourse.Models.Exceptions.Application;

namespace MyCourse.Models.Entities;

public partial class Course
{
    public Course(string title, string author, string authorId)
    {
        ChangeTitle(title);
        ChangeAuthor(author, authorId);
        Lessons = new HashSet<Lesson>();
        CurrentPrice = new Money(Currency.EUR, 0);
        FullPrice = new Money(Currency.EUR, 0);
        ImagePath = "/Courses/default.png";
    }

    public int Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public string ImagePath { get; private set; }
    public string Author { get; set; }
    public string Email { get; private set; }
    public double Rating { get; private set; }
    public Money FullPrice { get; private set; }
    public Money CurrentPrice { get; private set; }
    public string RowVersion { get; private set; }
    public CourseStatus Status { get; private set; }
    public string AuthorId { get; set; }
    public virtual ICollection<Lesson> Lessons { get; private set; }
    public virtual ApplicationUser AuthorUser { get; set; }
    public virtual ICollection<ApplicationUser> SubscribedUsers { get; private set; }

    public void Puslish()
    {
        ChangeStatus(CourseStatus.Published);
    }

    public void Unpublish()
    {
        ChangeStatus(CourseStatus.Draft);
    }

    public void Delete()
    {
        if (SubscribedUsers.Any())
        {
            throw new CourseDeletionException(Id);
        }

        ChangeStatus(CourseStatus.Deleted);
    }

    public void ChangeAuthor(string newAuthor, string newAuthorId)
    {
        EnsureNotDeleted();

        if (string.IsNullOrWhiteSpace(newAuthor))
        {
            throw new ArgumentException("The author must have a name");
        }

        if (string.IsNullOrWhiteSpace(newAuthorId))
        {
            throw new ArgumentException("The author must have a id");
        }

        Author = newAuthor;
        AuthorId = newAuthorId;
    }

    public void ChangeTitle(string newTitle)
    {
        EnsureNotDeleted();

        if (string.IsNullOrWhiteSpace(newTitle))
        {
            throw new ArgumentException("The course must have a title");
        }

        Title = newTitle;
    }

    public void ChangePrices(Money newFullPrice, Money newCurrentPrice)
    {
        EnsureNotDeleted();

        if (newFullPrice == null || newCurrentPrice == null)
        {
            throw new ArgumentException("Prices can't be null");
        }
        if (newFullPrice.Currency != newCurrentPrice.Currency)
        {
            throw new ArgumentException("Currencies don't match");
        }
        if (newFullPrice.Amount < newCurrentPrice.Amount)
        {
            throw new ArgumentException("Full price can't be less than the current price");
        }
        FullPrice = newFullPrice;
        CurrentPrice = newCurrentPrice;
    }

    public void ChangeEmail(string newEmail)
    {
        EnsureNotDeleted();

        if (string.IsNullOrEmpty(newEmail))
        {
            throw new ArgumentException("Email can't be empty");
        }
        Email = newEmail;
    }

    public void ChangeDescription(string newDescription)
    {
        EnsureNotDeleted();

        if (newDescription != null)
        {
            if (newDescription.Length < 20)
            {
                throw new Exception("Description is too short");
            }
            else if (newDescription.Length > 4000)
            {
                throw new Exception("Description is too long");
            }
        }
        Description = newDescription;
    }

    public void ChangeImagePath(string imagePath)
    {
        EnsureNotDeleted();

        ImagePath = imagePath;
    }

    public void ChangeRating(double? rating)
    {
        EnsureNotDeleted();

        if (rating == null)
        {
            return;
        }

        Rating = rating ?? 0;
    }

    private void EnsureNotDeleted()
    {
        if (Status == CourseStatus.Deleted)
        {
            throw new InvalidOperationException("Course is deleted and cannot be modified");
        }
    }

    private void ChangeStatus(CourseStatus status)
    {
        EnsureNotDeleted();
        Status = status;
    }
}
