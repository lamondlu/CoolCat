using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace DynamicPlugins.Core.Helpers
{
    public class DbHelper
    {
        private string _connectionString = string.Empty;

        public DbHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// 离线查询，返回DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="par"></param>
        /// <returns></returns>
        public DataTable ExecuteTable(string sql, params SqlParameter[] par)
        {
            using (SqlDataAdapter sda = new SqlDataAdapter(sql, _connectionString))
            {
                if (par != null && par.Length > 0)
                {
                    sda.SelectCommand.Parameters.AddRange(par);
                }
                DataTable dt = new DataTable();
                sda.Fill(dt);
                return dt;
            }

        }
        /// <summary>
        /// 查询首行首列，返回object
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="par"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, params SqlParameter[] par)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand com = new SqlCommand(sql, con))
                {
                    if (par != null && par.Length > 0)
                    {
                        com.Parameters.AddRange(par);
                    }
                    if (con.State != ConnectionState.Open)
                    {
                        con.Open();
                    }
                    return com.ExecuteScalar();
                }
            }
        }
        /// <summary>
        /// 在线查询，返回SqlDataReader，存储过程
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="par"></param>
        /// <returns></returns>
        public SqlDataReader ExecuteReader1(string procname, params SqlParameter[] par)
        {
            SqlConnection con = new SqlConnection(_connectionString);

            using (SqlCommand com = new SqlCommand(procname, con))
            {
                com.CommandType = CommandType.StoredProcedure;
                if (par != null && par.Length > 0)
                {
                    com.Parameters.AddRange(par);
                }
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                return com.ExecuteReader(CommandBehavior.CloseConnection);

            }
        }
        /// <summary>
        /// 在线查询，返回SqlDataReader
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="par"></param>
        /// <returns></returns>
        public SqlDataReader ExecuteReader(string procname, params SqlParameter[] par)
        {
            SqlConnection con = new SqlConnection(_connectionString);

            using (SqlCommand com = new SqlCommand(procname, con))
            {

                if (par != null && par.Length > 0)
                {
                    com.Parameters.AddRange(par);
                }
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                return com.ExecuteReader(CommandBehavior.CloseConnection);

            }
        }
        /// <summary>
        /// 增删改方法
        /// </summary>
        /// <param name="sql"></param>
        public int ExecuteNonQuery(string sql, params SqlParameter[] par)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand com = new SqlCommand(sql, con))
                {
                    if (par != null && par.Length > 0)
                    {
                        com.Parameters.AddRange(par);
                    }
                    if (con.State != ConnectionState.Open)
                    {
                        con.Open();
                    }
                    return com.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 增删改方法，存储过程
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="par"></param>
        /// <returns></returns>
        public int ExecuteNonQueryProc(string sql, params SqlParameter[] par)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand com = new SqlCommand(sql, con))
                {
                    com.CommandTimeout = 60;
                    com.CommandType = CommandType.StoredProcedure;
                    if (par != null && par.Length > 0)
                    {
                        com.Parameters.AddRange(par);
                    }
                    if (con.State != ConnectionState.Open)
                    {
                        con.Open();
                    }
                    return com.ExecuteNonQuery();
                }
            }
        }

    }
}
