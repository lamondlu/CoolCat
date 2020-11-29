using Mystique.Core.Contracts;
using Mystique.Core.DomainModel;
using Mystique.Core.Helpers;

namespace DemoPlugin2.Migrations
{
    public class Migration_1_0_0 : BaseMigration
    {
        private static readonly Mystique.Core.DomainModel.Version _version = new Mystique.Core.DomainModel.Version("1.0.0");

        public Migration_1_0_0(IDbHelper dbHelper) : base(dbHelper, _version)
        {

        }

        public override string UpScripts => @"CREATE TABLE `test3`  (
  `TestId` char(36) NOT NULL,
  PRIMARY KEY (`TestId`) 
) ENGINE = InnoDB";

        public override string DownScripts => @"DROP TABLE `test3`";
    }
}
