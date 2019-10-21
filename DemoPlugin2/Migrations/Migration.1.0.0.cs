using Mystique.Core.DomainModel;
using Mystique.Core.Repositories;

namespace DemoPlugin2.Migrations
{
    public class Migration_1_0_0 : BaseMigration
    {
        private static readonly Version version = new Version("1.0.0");

        public Migration_1_0_0(PluginDbContext pluginDbContext, IUnitOfWork unitOfWork) : base(pluginDbContext, unitOfWork, version)
        {
        }

        public override string UpScripts => "CREATE TABLE IF NOT EXISTS Test3 (TestId UNIQUEIDENTIFIER NOT NULL);";

        public override string DownScripts => "DROP TABLE Test3";
    }
}
