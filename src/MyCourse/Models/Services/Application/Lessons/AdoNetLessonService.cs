using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyCourse.Models.Exceptions.Application;
using MyCourse.Models.InputModels.Lessons;
using MyCourse.Models.Services.Infrastructure;
using MyCourse.Models.ViewModels.Lessons;

namespace MyCourse.Models.Services.Application.Lessons
{
    public class AdoNetLessonService : ILessonService
    {
        private readonly ILogger<AdoNetLessonService> logger;
        private readonly IDatabaseAccessor db;

        public AdoNetLessonService(ILogger<AdoNetLessonService> logger, IDatabaseAccessor db)
        {
            this.logger = logger;
            this.db = db;
        }
        public async Task<LessonDetailViewModel> CreateLessonAsync(LessonCreateInputModel inputModel)
        {
            int lessonId = await db.QueryScalarAsync<int>($@"INSERT INTO Lessons (Title, CourseId, Duration) VALUES ({inputModel.Title}, {inputModel.CourseId}, '00:00:00');
                                                 SELECT last_insert_rowid();");

            LessonDetailViewModel lesson = await GetLessonAsync(lessonId);
            return lesson;
        }

        public async Task<LessonDetailViewModel> EditLessonAsync(LessonEditInputModel inputModel)
        {
            int affectedRows = await db.CommandAsync($"UPDATE Lessons SET Title={inputModel.Title}, Description={inputModel.Description}, Duration={inputModel.Duration:HH':'mm':'ss}, [Order]={inputModel.Order} WHERE Id={inputModel.Id} AND RowVersion={inputModel.RowVersion}");
            if (affectedRows == 0)
            {
                bool lessonExists = await db.QueryScalarAsync<bool>($"SELECT COUNT(*) FROM Lessons WHERE Id={inputModel.Id}");
                if (lessonExists)
                {
                    throw new OptimisticConcurrencyException();
                }
                else
                {
                    throw new LessonNotFoundException(inputModel.Id);
                }
            }
            LessonDetailViewModel lesson = await GetLessonAsync(inputModel.Id);
            return lesson;
        }

        public async Task<LessonDetailViewModel> GetLessonAsync(int id)
        {
            FormattableString query = $@"SELECT Id, CourseId, Title, Description, Duration FROM Lessons WHERE ID={id}";

            DataSet dataSet = await db.QueryAsync(query);

            //Course
            var lessonTable = dataSet.Tables[0];
            if (lessonTable.Rows.Count != 1)
            {
                logger.LogWarning("Lesson {id} not found", id);
                throw new LessonNotFoundException(id);
            }
            var lessonRow = lessonTable.Rows[0];
            var lessonDetailViewModel = LessonDetailViewModel.FromDataRow(lessonRow);
            return lessonDetailViewModel;
        }

        public async Task<LessonEditInputModel> GetLessonForEditingAsync(int id)
        {
            FormattableString query = $@"SELECT Id, CourseId, Title, Description, Duration, RowVersion, [Order] FROM Lessons WHERE ID={id}";

            DataSet dataSet = await db.QueryAsync(query);

            //Course
            var lessonTable = dataSet.Tables[0];
            if (lessonTable.Rows.Count != 1)
            {
                logger.LogWarning("Lesson {id} not found", id);
                throw new LessonNotFoundException(id);
            }
            var lessonRow = lessonTable.Rows[0];
            var lessonEditInputModel = LessonEditInputModel.FromDataRow(lessonRow);
            return lessonEditInputModel;
        }

        public async Task DeleteLessonAsync(LessonDeleteInputModel inputModel)
        {
            int affectedRows = await db.CommandAsync($"DELETE FROM Lessons WHERE Id={inputModel.Id}");
            if (affectedRows == 0)
            {
                throw new LessonNotFoundException(inputModel.Id);
            }
        }
    }
}