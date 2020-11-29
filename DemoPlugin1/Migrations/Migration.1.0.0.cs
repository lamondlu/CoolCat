using Mystique.Core.Contracts;
using Mystique.Core.DomainModel;
using Mystique.Core.Helpers;

namespace DemoPlugin1.Migrations
{
    public class Migration_1_0_0 : BaseMigration
    {
        private static readonly Mystique.Core.DomainModel.Version _version = new Mystique.Core.DomainModel.Version("1.0.0");

        public override string UpScripts => @"CREATE TABLE `test`  (
  `TestId` char(36) NOT NULL,
  PRIMARY KEY (`TestId`) 
) ENGINE = InnoDB";

        public override string DownScripts => @"DROP TABLE `test`";

        public Migration_1_0_0(IDbHelper dbHelper) : base(dbHelper, _version)
        {

        }
    }
}
