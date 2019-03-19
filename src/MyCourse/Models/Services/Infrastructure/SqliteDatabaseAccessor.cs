using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace MyCourse.Models.Services.Infrastructure
{
    public class SqliteDatabaseAccessor : IDatabaseAccessor
    {
        public async Task<DataSet> QueryAsync(FormattableString formattableQuery)
        {   
            //Creiamo dei SqliteParameter a partire dalla FormattableString
            var queryArguments = formattableQuery.GetArguments();
            var sqliteParameters = new List<SqliteParameter>();
            for (var i = 0; i < queryArguments.Length; i++)
            {
                var parameter = new SqliteParameter(i.ToString(), queryArguments[i]);
                sqliteParameters.Add(parameter);
                queryArguments[i] = "@" + i;
            }
            string query = formattableQuery.ToString();

            //Colleghiamoci al database Sqlite, inviamo la query e leggiamo i risultati
            using(var conn = new SqliteConnection("Data Source=Data/MyCourse.db"))
            {
                await conn.OpenAsync();
                using (var cmd = new SqliteCommand(query, conn))
                {
                    //Aggiungiamo i SqliteParameters al SqliteCommand
                    cmd.Parameters.AddRange(sqliteParameters);

                    //Inviamo la query al database e otteniamo un SqliteDataReader
                    //per leggere i risultati
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var dataSet = new DataSet();
                        
                        //TODO: La riga qui sotto va rimossa quando la issue sarà risolta
                        //https://github.com/aspnet/EntityFrameworkCore/issues/14963
                        dataSet.EnforceConstraints = false;

                        //Creiamo tanti DataTable per quante sono le tabelle
                        //di risultati trovate dal SqliteDataReader
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