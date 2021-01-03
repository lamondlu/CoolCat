using CoolCat.Core.Contracts;
using CoolCat.Core.Models;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolCat.Core.Repository.MySql
{
    public class MySqlConnectionFactory : IDbConnectionFactory
    {
        private MySqlConnection _mySqlConnection = null;
        private ConnectionStringSetting _setting = null;

        public MySqlConnectionFactory(IOptions<ConnectionStringSetting> settingAccessor)
        {
            _setting = settingAccessor.Value;
        }

        public IDbConnection GetConnection()
        {
            _mySqlConnection = new MySqlConnection(_setting.ConnectionString);
            _mySqlConnection.Open();

            return _mySqlConnection;
        }
    }
}
