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

        public override void Down()
        {
            var scripts = @"DROP TABLE [dbo].[Test]";

            SQL(scripts);
        }

        public override void Up()
        {
            var scripts = @"CREATE TABLE [dbo].[Test](
	                    TestId [uniqueidentifier] NOT NULL,
                    PRIMARY KEY CLUSTERED 
                    (
	                    TestId ASC
                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";

            SQL(scripts);
        }
    }
}
