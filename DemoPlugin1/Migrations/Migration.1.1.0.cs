using Mystique.Core.DomainModel;
using Mystique.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoPlugin1.Migrations
{
    public class Migration_1_1_0 : BaseMigration
    {
        private static Mystique.Core.DomainModel.Version _version = new Mystique.Core.DomainModel.Version("1.1.0");

        public override string UpScripts
        {
            get
            {
                return @"CREATE TABLE [dbo].[Test2](
                        TestId[uniqueidentifier] NOT NULL,
                    );";
            }
        }

        public override string DownScripts
        {
            get
            {
                return @"DROP TABLE [dbo].[Test2]";
            }
        }

        public Migration_1_1_0(DbHelper dbHelper) : base(dbHelper, _version)
        {

        }
    }
}
