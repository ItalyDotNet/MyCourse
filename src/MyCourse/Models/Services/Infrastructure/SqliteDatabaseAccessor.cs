using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyCourse.Models.Exceptions.Infrastructure;
using MyCourse.Models.Options;
using MyCourse.Models.ValueObjects;

namespace MyCourse.Models.Services.Infrastructure
{
    public class SqliteDatabaseAccessor : IDatabaseAccessor
    {
        private readonly ILogger<SqliteDatabaseAccessor> logger;
        private readonly IOptionsMonitor<ConnectionStringsOptions> connectionStringOptions;

        public SqliteDatabaseAccessor(ILogger<SqliteDatabaseAccessor> logger, IOptionsMonitor<ConnectionStringsOptions> connectionStringOptions)
        {
            this.logger = logger;
            this.connectionStringOptions = connectionStringOptions;
        }

        public async Task<int> CommandAsync(FormattableString formattableCommand, CancellationToken token)
        {
            try
            {
                using SqliteConnection conn = await GetOpenedConnection(token);
                using SqliteCommand cmd = GetCommand(formattableCommand, conn);
                int affectedRows = await cmd.ExecuteNonQueryAsync(token);
                return affectedRows;
            }
            catch (SqliteException exc) when (exc.SqliteErrorCode == 19)
            {
                throw new ConstraintViolationException(exc);
            }
        }

        public async Task<T> QueryScalarAsync<T>(FormattableString formattableQuery, CancellationToken token)
        {
            try
            {
                using SqliteConnection conn = await GetOpenedConnection(token);
                using SqliteCommand cmd = GetCommand(formattableQuery, conn);
                object result = await cmd.ExecuteScalarAsync();
                return (T)Convert.ChangeType(result, typeof(T));
            }
            catch (SqliteException exc) when (exc.SqliteErrorCode == 19)
            {
                throw new ConstraintViolationException(exc);
            }
        }

        public async Task<DataSet> QueryAsync(FormattableString formattableQuery, CancellationToken token)
        {
            logger.LogInformation(formattableQuery.Format, formattableQuery.GetArguments());

            using SqliteConnection conn = await GetOpenedConnection(token);
            using SqliteCommand cmd = GetCommand(formattableQuery, conn);

            //Inviamo la query al database e otteniamo un SqliteDataReader
            //per leggere i risultati

            try
            {
                using var reader = await cmd.ExecuteReaderAsync(token);
                DataSet dataSet = new();

                //Creiamo tanti DataTable per quante sono le tabelle
                //di risultati trovate dal SqliteDataReader
                do
                {
                    DataTable dataTable = new();
                    dataSet.Tables.Add(dataTable);
                    dataTable.Load(reader);
                } while (!reader.IsClosed);

                return dataSet;
            }
            catch (SqliteException exc) when (exc.SqliteErrorCode == 19)
            {
                throw new ConstraintViolationException(exc);
            }

        }

        private static SqliteCommand GetCommand(FormattableString formattableQuery, SqliteConnection conn)
        {
            //Creiamo dei SqliteParameter a partire dalla FormattableString
            var queryArguments = formattableQuery.GetArguments();
            List<SqliteParameter> sqliteParameters = new();
            for (var i = 0; i < queryArguments.Length; i++)
            {
                if (queryArguments[i] is Sql)
                {
                    continue;
                }
                SqliteParameter parameter = new(name: i.ToString(), value: queryArguments[i] ?? DBNull.Value);
                sqliteParameters.Add(parameter);
                queryArguments[i] = "@" + i;
            }
            string query = formattableQuery.ToString();

            SqliteCommand cmd = new(query, conn);
            //Aggiungiamo i SqliteParameters al SqliteCommand
            cmd.Parameters.AddRange(sqliteParameters);
            return cmd;
        }

        private async Task<SqliteConnection> GetOpenedConnection(CancellationToken token)
        {
            //Colleghiamoci al database Sqlite, inviamo la query e leggiamo i risultati
            int retries = 3;
            while (true)
            {
                try
                {
                    retries--;
                    SqliteConnection conn = new(connectionStringOptions.CurrentValue.Default);
                    await conn.OpenAsync(token);
                    return conn;
                }
                catch (Exception) when (retries > 0)
                {
                    await Task.Delay(1000);
                }
            }
        }
    }
}