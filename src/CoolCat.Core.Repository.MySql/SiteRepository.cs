using CoolCat.Core.Consts;
using CoolCat.Core.Contracts;
using CoolCat.Core.DTOs;
using CoolCat.Core.Repositories;
using CoolCat.Core.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;

namespace CoolCat.Core.Repository.MySql
{
    public class SiteRepository : ISiteRepository
    {
        private readonly IDbConnection _dbConnection = null;
        private readonly List<Command> _commands = null;

        public SiteRepository(IDbConnection dbConnection, List<Command> commands)
        {
            _dbConnection = dbConnection;
            _commands = commands;
        }

        public SiteSettingsViewModel GetSiteSettings()
        {
            var sql = "SELECT * FROM SiteSettings";

            var keyPairs = _dbConnection.Query<KeyValueViewModel>(sql);

            var result = new SiteSettingsViewModel();

            if (keyPairs.Any(p => p.Key == SiteSettingKeyConst.SiteCSS))
            {
                result.SiteCSS = keyPairs.First(p => p.Key == SiteSettingKeyConst.SiteCSS).Value;
            }

            if (keyPairs.Any(p => p.Key == SiteSettingKeyConst.SiteTemplateId))
            {
                var val = keyPairs.First(p => p.Key == SiteSettingKeyConst.SiteTemplateId).Value;

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

            _dbConnection.Execute(sql, new { val = dto.SiteCSS, key = SiteSettingKeyConst.SiteCSS });
            _dbConnection.Execute(sql, new { val = dto.SiteTemplateId?.ToString(), key = SiteSettingKeyConst.SiteTemplateId });
        }
    }
}
