
using System;

using System.Collections.Generic;

using System.Data.SqlClient;

using System.Linq;


namespace Tor.Dal
{
    public class DataBaseRetriever

    {

        string ConnectionString = "";

        public DataBaseRetriever(string conString)

        {

            ConnectionString = conString;

        }







        public bool IsExist(string sql, object obj, int dbType)

        {

            switch (dbType)

            {

                case 1:

                    {

                        SqlServerProvider sqlServer = new SqlServerProvider(ConnectionString);

                        return sqlServer.IsExistRow(sql, obj);

                    }



            }

            return false;

        }

        public IEnumerable<T> GetData<T>(string sql, int dbType)

        {



            switch (dbType)

            {

                case 1:

                    {

                        SqlServerProvider sqlServer = new SqlServerProvider(ConnectionString);

                        return sqlServer.eGetData<T>(sql);

                    }



            }

            return null;

        }



        public IEnumerable<T> QueryData<T>(string sql, int dbType, object obj)

        {



            switch (dbType)

            {

                case 1:

                    {

                        SqlServerProvider sqlServer = new SqlServerProvider(ConnectionString);

                        return sqlServer.QueryData<T>(sql, obj);

                    }



            }

            return null;

        }



        public int Execute(string sql, int dbType, Object obj = null)

        {



            switch (dbType)

            {

                case 1:

                    {

                        SqlServerProvider sqlServer = new SqlServerProvider(ConnectionString);

                        return sqlServer.executeSql(sql, obj);

                    }



            }

            return 0;

        }





    }


}
