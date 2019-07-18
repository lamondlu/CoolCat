using DynamicPlugins.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace DynamicPlugins.Core.Helpers
{
    public class DbHelper
    {
        private string connectionString = string.Empty;

        public DbHelper(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void ExecuteNoQuery(List<Command> queries)
        {
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();
                SqlTransaction trans = Connection.BeginTransaction();

                try
                {
                    foreach (var query in queries)
                    {
                        SqlCommand cmd = new SqlCommand(query.Sql, Connection);
                        cmd.Transaction = trans;
                        cmd.Parameters.AddRange(query.Parameters.ToArray());
                        if (Connection.State != ConnectionState.Open)
                        {
                            Connection.Open();
                        }

                        cmd.ExecuteNonQuery();
                    }

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public void ExecuteNoQuery(Dictionary<string, List<SqlParameter>> queries)
        {
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();
                SqlTransaction trans = Connection.BeginTransaction();

                try
                {
                    foreach (var query in queries)
                    {
                        SqlCommand cmd = new SqlCommand(query.Key, Connection);
                        cmd.Transaction = trans;
                        cmd.Parameters.AddRange(query.Value.ToArray());
                        if (Connection.State != ConnectionState.Open)
                        {
                            Connection.Open();
                        }

                        cmd.ExecuteNonQuery();
                    }

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public int ExecuteNonQuery(string safeSql)
        {
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();
                SqlTransaction trans = Connection.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand(safeSql, Connection);
                    cmd.Transaction = trans;

                    if (Connection.State != ConnectionState.Open)
                    {
                        Connection.Open();
                    }
                    int result = cmd.ExecuteNonQuery();
                    trans.Commit();
                    return result;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    Console.WriteLine(ex.ToString());
                    return 0;
                }
            }
        }

        public int ExecuteNonQuery(string sql, SqlParameter[] values)
        {
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();
                SqlTransaction trans = Connection.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand(sql, Connection);
                    cmd.Transaction = trans;
                    cmd.Parameters.AddRange(values);
                    if (Connection.State != ConnectionState.Open)
                    {
                        Connection.Open();
                    }
                    int result = cmd.ExecuteNonQuery();
                    trans.Commit();
                    return result;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    Console.WriteLine(ex.ToString());
                    return 0;
                }
            }
        }

        public int ExecuteScalar(string safeSql)
        {
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                SqlCommand cmd = new SqlCommand(safeSql, Connection);
                int result = Convert.ToInt32(cmd.ExecuteScalar());
                return result;
            }
        }

        public int ExecuteScalar(string sql, SqlParameter[] values)
        {
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                SqlCommand cmd = new SqlCommand(sql, Connection);
                cmd.Parameters.AddRange(values);
                int result = Convert.ToInt32(cmd.ExecuteScalar());
                return result;
            }
        }

        public SqlDataReader ExecuteReader(string safeSql, SqlConnection Connection)
        {
            if (Connection.State != ConnectionState.Open)
                Connection.Open();
            SqlCommand cmd = new SqlCommand(safeSql, Connection);
            SqlDataReader reader = cmd.ExecuteReader();
            return reader;
        }

        public SqlDataReader ExecuteReader(string sql, SqlParameter[] values, SqlConnection Connection)
        {
            if (Connection.State != ConnectionState.Open)
                Connection.Open();
            SqlCommand cmd = new SqlCommand(sql, Connection);
            cmd.Parameters.AddRange(values);
            SqlDataReader reader = cmd.ExecuteReader();
            return reader;
        }

        public DataTable ExecuteDataTable(CommandType type, string safeSql, params SqlParameter[] values)
        {
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand(safeSql, Connection);
                cmd.CommandType = type;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                return ds.Tables[0];
            }
        }

        public DataTable ExecuteDataTable(string safeSql)
        {
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand(safeSql, Connection);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                try
                {
                    da.Fill(ds);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                return ds.Tables[0];
            }
        }

        public DataTable ExecuteDataTable(string sql, params SqlParameter[] values)
        {
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand(sql, Connection);
                cmd.CommandTimeout = 0;
                cmd.Parameters.AddRange(values);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                return ds.Tables[0];
            }
        }

        public DataSet GetDataSet(string safeSql, string tabName, params SqlParameter[] values)
        {
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand(safeSql, Connection);

                if (values != null)
                    cmd.Parameters.AddRange(values);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                try
                {
                    da.Fill(ds, tabName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                return ds;
            }
        }
    }
}
