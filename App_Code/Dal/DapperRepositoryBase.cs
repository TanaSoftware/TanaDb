using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using System.Data;

namespace Tor.Dal
{
    public abstract class DapperRepositoryBase

    {

        protected IDbConnection _connection = null;



        protected List<T> Query<T>(string storedProcName)

        {

            return Query<T>(storedProcName, null);

        }



        protected List<T> Query<T>(string storedProcName, object parameters)

        {

            return _connection.Query<T>(storedProcName, parameters, commandType: CommandType.StoredProcedure, buffered: false).ToList();

        }



        protected List<T> QuerySql<T>(string sql)

        {

            return _connection.Query<T>(sql, commandType: CommandType.Text, buffered: false).ToList();

        }



        protected IEnumerable<T> eQuerySql<T>(string sql)

        {

            return _connection.Query<T>(sql, commandType: CommandType.Text, buffered: false);

        }



        protected IEnumerable<T> QueryResult<T>(string sql, object obj)

        {

            return _connection.Query<T>(sql, obj, commandType: CommandType.Text, buffered: false);

        }



        public bool IsExist(string sql, Object obj)

        {



            return _connection.ExecuteScalar<bool>(sql, obj);

        }



        protected int ExecuteSql(string sql, Object obj)

        {

            return _connection.Execute(sql, param: obj, commandType: CommandType.Text);

        }



        protected void Execute(string storedProcName)

        {

            Execute(storedProcName, null);

        }



        protected void Execute(string storedProcName, object parameters)

        {

            _connection.Execute(storedProcName, parameters, commandType: CommandType.StoredProcedure);

        }



        protected void Execute(string storedProcName, object parameters, IDbTransaction transaction)

        {

            _connection.Execute(storedProcName, parameters, transaction: transaction, commandType: CommandType.StoredProcedure);

        }



        protected object ExecuteScalar(string storedProcName)

        {

            return ExecuteScalar(storedProcName, null);

        }



        protected object ExecuteScalar(string storedProcName, object parameters)

        {

            return _connection.ExecuteScalar(storedProcName, parameters, commandType: CommandType.StoredProcedure);

        }



        protected List<T> QueryTableView<T>(string query)

        {

            return _connection.Query<T>(query, buffered: false).ToList();

        }





        #region IDisposable



        public bool CanDispose { get; set; }



        public void Dispose(bool force)

        {

            this.CanDispose = true;

            Dispose();

        }



        public void Dispose()

        {

            if (_connection != null && this.CanDispose)

                _connection.Dispose();

        }

        #endregion

    }

}
