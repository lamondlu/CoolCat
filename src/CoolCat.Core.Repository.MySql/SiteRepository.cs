using MySql.Data.MySqlClient;
using CoolCat.Core.Consts;
using CoolCat.Core.Contracts;
using CoolCat.Core.DTOs;
using CoolCat.Core.Repositories;
using CoolCat.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CoolCat.Core.Repository.MySql
{
    public class SiteRepository : ISiteRepository
    {
        private readonly IDbHelper _dbHelper = null;
        private readonly List<Command> _commands = null;

        public SiteRepository(IDbHelper dbHelper, List<Command> commands)
        {
            _dbHelper = dbHelper;
            _commands = commands;
        }

        public SiteSettingsViewModel GetSiteSettings()
        {
            var sql = "SELECT * FROM SiteSettings";

            var rows = _dbHelper.ExecuteDataTable(sql).Rows.Cast<DataRow>().ToList();

            var result = new SiteSettingsViewModel();

            if (rows.Any(p => p["Key"].ToString() == SiteSettingKeyConst.SiteCSS))
            {
                result.SiteCSS = rows.FirstOrDefault(p => p["Key"].ToString() == SiteSettingKeyConst.SiteCSS)?.Field<string>("Value");
            }

            if (rows.Any(p => p["Key"].ToString() == SiteSettingKeyConst.SiteTemplateId))
            {
                var val = rows.FirstOrDefault(p => p["Key"].ToString() == SiteSettingKeyConst.SiteTemplateId)?.Field<string>("Value");

                Guid guidVal;
                if (!string.IsNullOrWhiteSpace(val) && Guid.TryParse(val, out guidVal))
                {
                    result.SiteTemplateId = guidVal;
                }
            }

            return result;
        }

        public void SaveSiteSettings(SiteSettingsDTO dto)
        {
            var sql = "UPDATE SiteSettings SET `Value`=@val WHERE `Key`=@key";

            _dbHelper.ExecuteNonQuery(sql, new List<MySqlParameter>() {
                new MySqlParameter{ ParameterName = "@val", MySqlDbType = MySqlDbType.VarChar, Value = dto.SiteCSS },
                 new MySqlParameter{ ParameterName = "@key", MySqlDbType = MySqlDbType.VarChar, Value = SiteSettingKeyConst.SiteCSS }
            }.ToArray());

            _dbHelper.ExecuteNonQuery(sql, new List<MySqlParameter>() {
                new MySqlParameter{ ParameterName = "@val", MySqlDbType = MySqlDbType.VarChar, Value = dto.SiteTemplateId?.ToString() },
                 new MySqlParameter{ ParameterName = "@key", MySqlDbType = MySqlDbType.VarChar, Value = SiteSettingKeyConst.SiteTemplateId }
            }.ToArray());
        }
    }
}
