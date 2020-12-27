using CoolCat.Core.Contracts;
using CoolCat.Core.Models;
using CoolCat.Core.Repositories;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace CoolCat.Core.Repository.MySql
{
    public class DbHelper : IDbHelper
    {
        private readonly string connectionString = string.Empty;

        public DbHelper(IOptions<ConnectionStringSetting> setting)
        {
            this.connectionString = setting.Value.ConnectionString;
        }

        public void ExecuteNonQuery(List<Command> commands)
        {
            using (MySqlConnection Connection = new MySqlConnection(connectionString))
            {
                Connection.Open();
                MySqlTransaction trans = Connection.BeginTransaction();

                try
                {
                    foreach (Command query in commands)
                    {

                        MySqlCommand cmd = new MySqlCommand(query.Sql, Connection)
                        {
                            Transaction = trans
                        };

                        cmd.Parameters.AddRange(query.Parameters.ToArray());
                        if (Connection.State != ConnectionState.Open)
                        {
                            Connection.Open();
                        }

                        cmd.ExecuteNonQuery();

                    }

                    trans.Commit();
                    commands.Clear();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public void ExecuteNonQuery(Dictionary<string, List<MySqlParameter>> queries)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlTransaction trans = connection.BeginTransaction();

                try
                {
                    foreach (KeyValuePair<string, List<MySqlParameter>> query in queries)
                    {
                        MySqlCommand cmd = new MySqlCommand(query.Key, connection)
                        {
                            Transaction = trans
                        };
                        cmd.Parameters.AddRange(query.Value.ToArray());
                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
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

        public void SQL(string sql)
        {
            ExecuteNonQuery(sql);
        }

        public int ExecuteNonQuery(string safeSql)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlTransaction trans = connection.BeginTransaction();
                try
                {
                    MySqlCommand cmd = new MySqlCommand(safeSql, connection)
                    {
                        Transaction = trans
                    };

                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
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

        public int ExecuteNonQuery(string sql, MySqlParameter[] values)
        {
            using (MySqlConnection Connection = new MySqlConnection(connectionString))
            {
                Connection.Open();
                MySqlTransaction trans = Connection.BeginTransaction();
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sql, Connection)
                    {
                        Transaction = trans
                    };
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
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                MySqlCommand cmd = new MySqlCommand(safeSql, connection);
                int result = Convert.ToInt32(cmd.ExecuteScalar());
                return result;
            }
        }

        public int ExecuteScalar(string sql, MySqlParameter[] values)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                MySqlCommand cmd = new MySqlCommand(sql, connection);
                cmd.Parameters.AddRange(values);
                int result = Convert.ToInt32(cmd.ExecuteScalar());
                return result;
            }
        }

        public object ExecuteScalarWithObjReturn(string sql, MySqlParameter[] values)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                MySqlCommand cmd = new MySqlCommand(sql, connection);
                cmd.Parameters.AddRange(values);
                return cmd.ExecuteScalar();
            }
        }

        public DataTable ExecuteDataTable(CommandType type, string safeSql, params MySqlParameter[] values)
        {
            using (MySqlConnection Connection = new MySqlConnection(connectionString))
            {
                if (Connection.State != ConnectionState.Open)
                {
                    Connection.Open();
                }

                DataSet ds = new DataSet();
                MySqlCommand cmd = new MySqlCommand(safeSql, Connection)
                {
                    CommandType = type
                };
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(ds);
                return ds.Tables[0];
            }
        }

        public DataTable ExecuteDataTable(string safeSql)
        {
            using (MySqlConnection Connection = new MySqlConnection(connectionString))
            {
                if (Connection.State != ConnectionState.Open)
                {
                    Connection.Open();
                }

                DataSet ds = new DataSet();
                MySqlCommand cmd = new MySqlCommand(safeSql, Connection);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
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

        public DataTable ExecuteDataTable(string sql, params MySqlParameter[] values)
        {
            using (MySqlConnection Connection = new MySqlConnection(connectionString))
            {
                if (Connection.State != ConnectionState.Open)
                {
                    Connection.Open();
                }

                DataSet ds = new DataSet();
                MySqlCommand cmd = new MySqlCommand(sql, Connection)
                {
                    CommandTimeout = 0
                };
                cmd.Parameters.AddRange(values);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(ds);
                return ds.Tables[0];
            }
        }

        public DataSet GetDataSet(string safeSql, string tabName, params MySqlParameter[] values)
        {
            using (MySqlConnection Connection = new MySqlConnection(connectionString))
            {
                if (Connection.State != ConnectionState.Open)
                {
                    Connection.Open();
                }

                DataSet ds = new DataSet();
                MySqlCommand cmd = new MySqlCommand(safeSql, Connection);

                if (values != null)
                {
                    cmd.Parameters.AddRange(values);
                }

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
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
