using MySql.Data.MySqlClient;
using Mystique.Core.Repositories;
using System.Collections.Generic;
using System.Data;

namespace Mystique.Core.Contracts
{
    public interface IDbHelper
    {
        void ExecuteNonQuery(List<Command> commands);

        void ExecuteNonQuery(Dictionary<string, List<MySqlParameter>> queries);

        void SQL(string sql);

        int ExecuteNonQuery(string safeSql);

        int ExecuteNonQuery(string sql, MySqlParameter[] values);

        int ExecuteScalar(string safeSql);

        int ExecuteScalar(string sql, MySqlParameter[] values);

        object ExecuteScalarWithObjReturn(string sql, MySqlParameter[] values);

        DataTable ExecuteDataTable(CommandType type, string safeSql, params MySqlParameter[] values);

        DataTable ExecuteDataTable(string safeSql);

        DataTable ExecuteDataTable(string sql, params MySqlParameter[] values);

        DataSet GetDataSet(string safeSql, string tabName, params MySqlParameter[] values);
    }
}
