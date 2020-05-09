using Mystique.Core.DomainModel;
using Mystique.Core.Helpers;

namespace DemoPlugin1.Migrations
{
    public class Migration_1_1_0 : BaseMigration
    {
        private static readonly Mystique.Core.DomainModel.Version _version = new Mystique.Core.DomainModel.Version("1.1.0");

        public override string UpScripts => @"CREATE TABLE [dbo].[Test2](
                        TestId[uniqueidentifier] NOT NULL,
                    );";

        public override string DownScripts => @"DROP TABLE [dbo].[Test2]";

        public Migration_1_1_0(DbHelper dbHelper) : base(dbHelper, _version)
        {

        }
    }
}
