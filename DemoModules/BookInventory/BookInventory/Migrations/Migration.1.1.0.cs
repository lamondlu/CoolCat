using Mystique.Core.Contracts;
using Mystique.Core.DomainModel;
using Mystique.Core.Helpers;

namespace BookInventory.Migrations
{
    public class Migration_1_1_0 : BaseMigration
    {
        private static readonly Mystique.Core.DomainModel.Version _version = new Mystique.Core.DomainModel.Version("1.1.0");

        public override string UpScripts => @"ALTER TABLE Book ADD COLUMN Status BIT NOT NULL DEFAULT 0";

        public override string DownScripts => @"ALTER TABLE Book DROP COLUMN Status";

        public Migration_1_1_0(IDbHelper dbHelper) : base(dbHelper, _version)
        {

        }
    }
}
