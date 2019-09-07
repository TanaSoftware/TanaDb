using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Tor.Dal
{
    public class SqlServerProvider : DapperRepositoryBase

    {

        public SqlServerProvider(string conString)

        {

            _connection = new SqlConnection();

            _connection.ConnectionString = conString;

        }

        public bool IsExistRow(string sql, Object obj)

        {

            return IsExist(sql, obj);

        }

        //public List<T> GetData<T>(string sql)

        //{

        // return QuerySql<T>(sql).ToList();

        //}



        public IEnumerable<T> eGetData<T>(string sql)

        {

            return eQuerySql<T>(sql);

        }



        public IEnumerable<T> QueryData<T>(string sql, Object obj)

        {

            return QueryResult<T>(sql, obj);

        }





        public int executeSql(string sql, Object obj = null)

        {

            return ExecuteSql(sql, obj);

        }

    }
}
