using CoolCat.Core.Contracts;
using CoolCat.Core.DomainModel;

namespace DemoPlugin1.Migrations
{
    public class Migration_1_1_0 : BaseMigration
    {
        private static readonly CoolCat.Core.DomainModel.Version _version = new CoolCat.Core.DomainModel.Version("1.1.0");


        public override string UpScripts => @"CREATE TABLE `test2`  (
  `TestId` char(36) NOT NULL,
  PRIMARY KEY (`TestId`) 
) ENGINE = InnoDB";

        public override string DownScripts => @"DROP TABLE `test2`";

        public Migration_1_1_0(IDbConnectionFactory dbConnectionFactory) : base(_version, dbConnectionFactory)
        {

        }
    }
}
