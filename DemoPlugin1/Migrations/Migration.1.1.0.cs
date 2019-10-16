using Mystique.Core.DomainModel;
using Mystique.Core.Repositories;

namespace DemoPlugin1.Migrations
{
    public class Migration_1_1_0 : BaseMigration
    {
        private static readonly Version version = new Version("1.1.0");

        public Migration_1_1_0(PluginDbContext pluginDbContext, IUnitOfWork unitOfWork) : base(pluginDbContext, unitOfWork, version)
        {
        }

        public override string UpScripts => "CREATE TABLE IF NOT EXISTS Test2 (TestId UNIQUEIDENTIFIER NOT NULL);";

        public override string DownScripts => "DROP TABLE Test2";
    }
}
