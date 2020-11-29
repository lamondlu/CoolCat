using Mystique.Core.Contracts;
using Mystique.Core.DomainModel;
using Mystique.Core.Helpers;

namespace BookInventory.Migrations
{
    public class Migration_1_0_0 : BaseMigration
    {
        private static readonly Mystique.Core.DomainModel.Version _version = new Mystique.Core.DomainModel.Version("1.0.0");

        public override string UpScripts => @"CREATE TABLE `book`(
  `BookId` char(36) NOT NULL,
  `BookName` varchar(255) NULL DEFAULT NULL,
  `DateIssued` datetime(0) NULL DEFAULT NULL,
  `ISBN` varchar(255)NULL DEFAULT NULL,
  `Description` text NULL,
  PRIMARY KEY (`BookId`) USING BTREE
)";

        public override string DownScripts => @"DROP TABLE Book";

        public Migration_1_0_0(IDbHelper dbHelper) : base(dbHelper, _version)
        {

        }
    }
}
