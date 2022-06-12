using System.Data;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using MyCourse.Models.Exceptions.Application;
using MyCourse.Models.Exceptions.Infrastructure;
using MyCourse.Models.InputModels.Courses;
using MyCourse.Models.Services.Infrastructure;
using MyCourse.Models.ViewModels.Courses;
using MyCourse.Models.ViewModels.Lessons;
using MyCourse.Controllers;
using Ganss.XSS;

namespace MyCourse.Models.Services.Application.Courses;

public class AdoNetCourseService : ICourseService
{
    private readonly ILogger<AdoNetCourseService> logger;
    private readonly IDatabaseAccessor db;
    private readonly IPaymentGateway paymentGateway;
    private readonly IOptionsMonitor<CoursesOptions> coursesOptions;
    private readonly LinkGenerator linkGenerator;
    private readonly ITransactionLogger transactionLogger;
    private readonly IImagePersister imagePersister;
    private readonly IEmailClient emailClient;
    private readonly IHttpContextAccessor httpContextAccessor;
    public AdoNetCourseService(IDatabaseAccessor db,
                               IPaymentGateway paymentGateway,
                               IImagePersister imagePersister,
                               IHttpContextAccessor httpContextAccessor,
                               IEmailClient emailClient,
                               IOptionsMonitor<CoursesOptions> coursesOptions,
                               LinkGenerator linkGenerator,
                               ITransactionLogger transactionLogger,
                               ILogger<AdoNetCourseService> logger)
    {
        this.db = db;
        this.paymentGateway = paymentGateway;
        this.imagePersister = imagePersister;
        this.coursesOptions = coursesOptions;
        this.linkGenerator = linkGenerator;
        this.transactionLogger = transactionLogger;
        this.logger = logger;
        this.emailClient = emailClient;
        this.httpContextAccessor = httpContextAccessor;
    }

    public async Task<CourseDetailViewModel> GetCourseAsync(int id)
    {
        logger.LogInformation("Course {id} requested", id);

        FormattableString query = $@"SELECT Id, Title, Description, ImagePath, Author, Rating, FullPrice_Amount, FullPrice_Currency, CurrentPrice_Amount, CurrentPrice_Currency FROM Courses WHERE Id={id} AND Status<>{nameof(CourseStatus.Deleted)}
            ; SELECT Id, Title, Description, Duration FROM Lessons WHERE CourseId={id} ORDER BY [Order], Id";

        DataSet dataSet = await db.QueryAsync(query);

        //Course
        var courseTable = dataSet.Tables[0];
        if (courseTable.Rows.Count != 1)
        {
            logger.LogWarning("Course {id} not found", id);
            throw new CourseNotFoundException(id);
        }
        var courseRow = courseTable.Rows[0];
        var courseDetailViewModel = CourseDetailViewModel.FromDataRow(courseRow);

        //Course lessons
        var lessonDataTable = dataSet.Tables[1];

        foreach (DataRow lessonRow in lessonDataTable.Rows)
        {
            LessonViewModel lessonViewModel = LessonViewModel.FromDataRow(lessonRow);
            courseDetailViewModel.Lessons.Add(lessonViewModel);
        }
        return courseDetailViewModel;
    }

    public async Task<ListViewModel<CourseViewModel>> GetCoursesAsync(CourseListInputModel model)
    {
        string orderby = model.OrderBy == "CurrentPrice" ? "CurrentPrice_Amount" : model.OrderBy;
        string direction = model.Ascending ? "ASC" : "DESC";

        FormattableString query = $@"SELECT Id, Title, ImagePath, Author, Rating, FullPrice_Amount, FullPrice_Currency, CurrentPrice_Amount, CurrentPrice_Currency FROM Courses WHERE Title LIKE {"%" + model.Search + "%"} AND Status<>{nameof(CourseStatus.Deleted)} ORDER BY {(Sql)orderby} {(Sql)direction} LIMIT {model.Limit} OFFSET {model.Offset}; 
            SELECT COUNT(*) FROM Courses WHERE Title LIKE {"%" + model.Search + "%"} AND Status<>{nameof(CourseStatus.Deleted)}";
        DataSet dataSet = await db.QueryAsync(query);
        DataTable dataTable = dataSet.Tables[0];
        List<CourseViewModel> courseList = new();
        foreach (DataRow courseRow in dataTable.Rows)
        {
            CourseViewModel courseViewModel = CourseViewModel.FromDataRow(courseRow);
            courseList.Add(courseViewModel);
        }

        ListViewModel<CourseViewModel> result = new()
        {
            Results = courseList,
            TotalCount = Convert.ToInt32(dataSet.Tables[1].Rows[0][0])
        };

        return result;
    }

    public async Task<List<CourseDetailViewModel>> GetCoursesByAuthorAsync(string authorId)
    {
        FormattableString query = $@"SELECT Id FROM Courses WHERE AuthorId={authorId} AND Status<>{nameof(CourseStatus.Deleted)}";
        DataSet dataSet = await db.QueryAsync(query);
        DataTable dataTable = dataSet.Tables[0];
        List<CourseDetailViewModel> courseList = new();
        foreach (DataRow courseRow in dataTable.Rows)
        {
            CourseDetailViewModel course = await GetCourseAsync(courseRow.Field<int>("Id"));
            courseList.Add(course);
        }

        return courseList;
    }

    public async Task<CourseEditInputModel> GetCourseForEditingAsync(int id)
    {
        FormattableString query = $@"SELECT Id, Title, Description, ImagePath, Email, FullPrice_Amount, FullPrice_Currency, CurrentPrice_Amount, CurrentPrice_Currency, RowVersion FROM Courses WHERE Id={id} AND Status<>{nameof(CourseStatus.Deleted)}";

        DataSet dataSet = await db.QueryAsync(query);

        DataTable courseTable = dataSet.Tables[0];
        if (courseTable.Rows.Count != 1)
        {
            logger.LogWarning("Course {id} not found", id);
            throw new CourseNotFoundException(id);
        }
        DataRow courseRow = courseTable.Rows[0];
        CourseEditInputModel courseEditInputModel = CourseEditInputModel.FromDataRow(courseRow);
        return courseEditInputModel;
    }

    public async Task<List<CourseViewModel>> GetBestRatingCoursesAsync()
    {
        CourseListInputModel inputModel = new(
            search: "",
            page: 1,
            orderby: "Rating",
            ascending: false,
            limit: coursesOptions.CurrentValue.InHome,
            orderOptions: coursesOptions.CurrentValue.Order);

        ListViewModel<CourseViewModel> result = await GetCoursesAsync(inputModel);
        return result.Results;
    }

    public async Task<List<CourseViewModel>> GetMostRecentCoursesAsync()
    {
        CourseListInputModel inputModel = new(
            search: "",
            page: 1,
            orderby: "Id",
            ascending: false,
            limit: coursesOptions.CurrentValue.InHome,
            orderOptions: coursesOptions.CurrentValue.Order);

        ListViewModel<CourseViewModel> result = await GetCoursesAsync(inputModel);
        return result.Results;
    }

    public async Task<CourseDetailViewModel> CreateCourseAsync(CourseCreateInputModel inputModel)
    {
        string title = inputModel.Title;
        string author;
        string authorId;

        try
        {
            author = httpContextAccessor.HttpContext.User.FindFirst("FullName").Value;
            authorId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
        catch (NullReferenceException)
        {
            throw new UserUnknownException();
        }

        try
        {
            int courseId = await db.QueryScalarAsync<int>($@"INSERT INTO Courses (Title, Author, AuthorId, ImagePath, Rating, CurrentPrice_Currency, CurrentPrice_Amount, FullPrice_Currency, FullPrice_Amount, Status) VALUES ({title}, {author}, {authorId}, '/Courses/default.png', 0, 'EUR', 0, 'EUR', 0, {nameof(CourseStatus.Draft)});
                                                 SELECT last_insert_rowid();");

            CourseDetailViewModel course = await GetCourseAsync(courseId);
            return course;
        }
        catch (ConstraintViolationException exc)
        {
            throw new CourseTitleUnavailableException(inputModel.Title, exc);
        }
    }

    public async Task<bool> IsTitleAvailableAsync(string title, int id)
    {
        bool titleExists = await db.QueryScalarAsync<bool>($"SELECT COUNT(*) FROM Courses WHERE Title LIKE {title} AND id<>{id}");
        return !titleExists;
    }

    public async Task<CourseDetailViewModel> EditCourseAsync(CourseEditInputModel inputModel)
    {
        try
        {
            string imagePath = null;
            if (inputModel.Image != null)
            {
                imagePath = await imagePersister.SaveCourseImageAsync(inputModel.Id, inputModel.Image);
            }
            int affectedRows = await db.CommandAsync($"UPDATE Courses SET ImagePath=COALESCE({imagePath}, ImagePath), Title={inputModel.Title}, Description={inputModel.Description}, Email={inputModel.Email}, CurrentPrice_Currency={inputModel.CurrentPrice.Currency.ToString()}, CurrentPrice_Amount={inputModel.CurrentPrice.Amount}, FullPrice_Currency={inputModel.FullPrice.Currency.ToString()}, FullPrice_Amount={inputModel.FullPrice.Amount} WHERE Id={inputModel.Id} AND Status<>{nameof(CourseStatus.Deleted)} AND RowVersion={inputModel.RowVersion}");
            if (affectedRows == 0)
            {
                bool courseExists = await db.QueryScalarAsync<bool>($"SELECT COUNT(*) FROM Courses WHERE Id={inputModel.Id} AND Status<>{nameof(CourseStatus.Deleted)}");
                if (courseExists)
                {
                    throw new OptimisticConcurrencyException();
                }
                else
                {
                    throw new CourseNotFoundException(inputModel.Id);
                }
            }
        }
        catch (ConstraintViolationException exc)
        {
            throw new CourseTitleUnavailableException(inputModel.Title, exc);
        }
        catch (ImagePersistenceException exc)
        {
            throw new CourseImageInvalidException(inputModel.Id, exc);
        }

        CourseDetailViewModel course = await GetCourseAsync(inputModel.Id);
        return course;
    }

    public async Task DeleteCourseAsync(CourseDeleteInputModel inputModel)
    {
        int affectedRows = await db.CommandAsync($"UPDATE Courses SET Status={nameof(CourseStatus.Deleted)} WHERE Id={inputModel.Id} AND Status<>{nameof(CourseStatus.Deleted)}");
        if (affectedRows == 0)
        {
            throw new CourseNotFoundException(inputModel.Id);
        }
    }

    public async Task SendQuestionToCourseAuthorAsync(int id, string question)
    {
        // Recupero le informazioni del corso
        FormattableString query = $@"SELECT Title, Email FROM Courses WHERE Courses.Id={id}";
        DataSet dataSet = await db.QueryAsync(query);

        if (dataSet.Tables[0].Rows.Count == 0)
        {
            logger.LogWarning("Course {id} not found", id);
            throw new CourseNotFoundException(id);
        }

        string courseTitle = Convert.ToString(dataSet.Tables[0].Rows[0]["Title"]);
        string courseEmail = Convert.ToString(dataSet.Tables[0].Rows[0]["Email"]);

        // Recupero le informazioni dell'utente che vuole inviare la domanda
        string userFullName;
        string userEmail;

        try
        {
            userFullName = httpContextAccessor.HttpContext.User.FindFirst("FullName").Value;
            userEmail = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email).Value;
        }
        catch (NullReferenceException)
        {
            throw new UserUnknownException();
        }

        // Sanitizzo la domanda dell'utente
        question = new HtmlSanitizer(allowedTags: new string[0]).Sanitize(question);

        // Compongo il testo della domanda
        string subject = $@"Domanda per il tuo corso ""{courseTitle}""";
        string message = $@"<p>L'utente {userFullName} (<a href=""{userEmail}"">{userEmail}</a>)
                                ti ha inviato la seguente domanda:</p>
                                <p>{question}</p>";

        // Invio la domanda
        try
        {
            await emailClient.SendEmailAsync(courseEmail, userEmail, subject, message);
        }
        catch
        {
            throw new SendException();
        }
    }

    public Task<string> GetCourseAuthorIdAsync(int courseId)
    {
        return db.QueryScalarAsync<string>($"SELECT AuthorId FROM Courses WHERE Id={courseId}");
    }

    public Task<int> GetCourseCountByAuthorIdAsync(string authorId)
    {
        return db.QueryScalarAsync<int>($"SELECT COUNT(*) FROM Courses WHERE AuthorId={authorId}");
    }

    public async Task SubscribeCourseAsync(CourseSubscribeInputModel inputModel)
    {
        try
        {
            await db.CommandAsync($"INSERT INTO Subscriptions (UserId, CourseId, PaymentDate, PaymentType, Paid_Currency, Paid_Amount, TransactionId) VALUES ({inputModel.UserId}, {inputModel.CourseId}, {inputModel.PaymentDate}, {inputModel.PaymentType}, {inputModel.Paid.Currency.ToString()}, {inputModel.Paid.Amount}, {inputModel.TransactionId})");
        }
        catch (ConstraintViolationException)
        {
            throw new CourseSubscriptionException(inputModel.CourseId);
        }
        catch (Exception)
        {
            await transactionLogger.LogTransactionAsync(inputModel);
        }
    }

    public Task<bool> IsCourseSubscribedAsync(int courseId, string userId)
    {
        return db.QueryScalarAsync<bool>($"SELECT COUNT(*) FROM Subscriptions WHERE CourseId={courseId} AND UserId={userId}");
    }

    public async Task<string> GetPaymentUrlAsync(int courseId)
    {
        CourseDetailViewModel viewModel = await GetCourseAsync(courseId);

        CoursePayInputModel inputModel = new()
        {
            CourseId = courseId,
            UserId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier),
            Description = viewModel.Title,
            Price = viewModel.CurrentPrice,
            ReturnUrl = linkGenerator.GetUriByAction(httpContextAccessor.HttpContext,
                      action: nameof(CoursesController.Subscribe),
                      controller: "Courses",
                      values: new { id = courseId }),
            CancelUrl = linkGenerator.GetUriByAction(httpContextAccessor.HttpContext,
                      action: nameof(CoursesController.Detail),
                      controller: "Courses",
                      values: new { id = courseId })
        };

        return await paymentGateway.GetPaymentUrlAsync(inputModel);
    }

    public Task<CourseSubscribeInputModel> CapturePaymentAsync(int courseId, string token)
    {
        return paymentGateway.CapturePaymentAsync(token);
    }

    public async Task<int?> GetCourseVoteAsync(int id)
    {
        string userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        string vote = await db.QueryScalarAsync<string>($"SELECT Vote FROM Subscriptions WHERE CourseId={id} AND UserId={userId}");
        return string.IsNullOrEmpty(vote) ? null : Convert.ToInt32(vote);
    }

    public async Task VoteCourseAsync(CourseVoteInputModel inputModel)
    {
        if (inputModel.Vote < 1 || inputModel.Vote > 5)
        {
            throw new InvalidVoteException(inputModel.Vote);
        }

        string userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        int updatedRows = await db.CommandAsync($"UPDATE Subscriptions SET Vote={inputModel.Vote} WHERE CourseId={inputModel.Id} AND UserId={userId}");
        if (updatedRows == 0)
        {
            throw new CourseSubscriptionNotFoundException(inputModel.Id);
        }
    }
}
