using DynamicPlugins.Core.Contracts;
using DynamicPlugins.Core.DomainModel;
using DynamicPlugins.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoPlugin1.Migrations
{
    public class Migration_1_0_0 : BaseMigration
    {
        private static DynamicPlugins.Core.DomainModel.Version _version = new DynamicPlugins.Core.DomainModel.Version("1.0.0");
        private static string _upScripts = @"CREATE TABLE [dbo].[Test](
                        TestId[uniqueidentifier] NOT NULL,
                    );";
        private static string _downScripts = @"DROP TABLE[dbo].[Test]";

        public Migration_1_0_0(DbHelper dbHelper) : base(dbHelper, _version)
        {

        }

        public DynamicPlugins.Core.DomainModel.Version Version
        {
            get
            {
                return _version;
            }
        }

        public override void MigrationDown(Guid pluginId)
        {
            SQL(_downScripts);

            base.WriteDownScripts(pluginId);
        }

        public override void MigrationUp(Guid pluginId)
        {
            SQL(_upScripts);

            base.WriteUpScripts(pluginId, _upScripts, _downScripts);
        }
    }
}
