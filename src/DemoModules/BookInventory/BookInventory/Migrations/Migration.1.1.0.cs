using CoolCat.Core.Contracts;
using CoolCat.Core.DomainModel;

namespace BookInventory.Migrations
{
    public class Migration_1_1_0 : BaseMigration
    {
        private static readonly CoolCat.Core.DomainModel.Version _version = new CoolCat.Core.DomainModel.Version("1.1.0");

        public override string UpScripts => @"ALTER TABLE Book ADD COLUMN Status BIT NOT NULL DEFAULT 0";

        public override string DownScripts => @"ALTER TABLE Book DROP COLUMN Status";

        public Migration_1_1_0(IDbHelper dbHelper) : base(dbHelper, _version)
        {

        }
    }
}
