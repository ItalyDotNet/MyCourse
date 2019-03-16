using System.Data;
using Microsoft.Data.Sqlite;

namespace MyCourse.Models.Services.Infrastructure
{
    public class SqliteDatabaseAccessor : IDatabaseAccessor
    {
        public DataSet Query(string query)
        {   
            using(var conn = new SqliteConnection("Data Source=Data/MyCourse.db"))
            {
                conn.Open();
                using (var cmd = new SqliteCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        var dataSet = new DataSet();
                        
                        //TODO: La riga qui sotto va rimossa quando la issue sarà risolta
                        //https://github.com/aspnet/EntityFrameworkCore/issues/14963
                        dataSet.EnforceConstraints = false;

                        do 
                        {
                            var dataTable = new DataTable();
                            dataSet.Tables.Add(dataTable);
                            dataTable.Load(reader);
                        } while (!reader.IsClosed);

                        return dataSet;
                    }
                }
            }
        }
    }
}