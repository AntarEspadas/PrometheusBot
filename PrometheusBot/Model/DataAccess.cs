using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using MySql.Data.MySqlClient;

namespace PrometheusBot.Model
{
    public class DataAccess
    {
        public string ConnectionString { get; set; }

        public DataAccessSession Session => new(ConnectionString);

        public DataAccess(string connectinoString)
        {
            ConnectionString = connectinoString;
        }
        public List<T> LoadData<T, U>(string sql, U parameters)
        {
            using var dataSession = Session;
            return dataSession.LoadData<T, U>(sql, parameters);
        }

        public void SaveData<T>(string sql, T parameters)
        {
            using var dataSession = Session;
            dataSession.SaveData(sql, parameters);
        }
    }

    public class DataAccessSession : IDisposable
    {

        private IDbConnection _connection;

        public DataAccessSession(string connectionString)
        {
            _connection = new MySqlConnection(connectionString);
        }
        public List<T> LoadData<T, U>(string sql, U parameters)
        {
            List<T> rows = _connection.Query<T>(sql, parameters).ToList();
            return rows;
        }

        public void SaveData<T>(string sql, T parameters)
        {
            _connection.Execute(sql, parameters);
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
