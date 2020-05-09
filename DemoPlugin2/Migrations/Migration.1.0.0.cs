using Mystique.Core.DomainModel;
using Mystique.Core.Helpers;

namespace DemoPlugin2.Migrations
{
    public class Migration_1_0_0 : BaseMigration
    {
        private static readonly Mystique.Core.DomainModel.Version _version = new Mystique.Core.DomainModel.Version("1.0.0");

        public Migration_1_0_0(DbHelper dbHelper) : base(dbHelper, _version)
        {

        }

        public override string UpScripts => @"CREATE TABLE [dbo].[Test3](
                        TestId[uniqueidentifier] NOT NULL,
                    );";

        public override string DownScripts => @"DROP TABLE [dbo].[Test3]";
    }
}
