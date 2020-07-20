using FluentMigrator;

namespace Mystique.Core.Repository.MySql.Migrations
{
    [Migration(202007202310)]
    public class AddGlobalSettings : Migration
    {
        public override void Up()
        {
            Create.Table("GlobalSettings")
              .WithColumn("Key").AsString().PrimaryKey()
              .WithColumn("Value").AsString().NotNullable();

            Insert.IntoTable("GlobalSettings").Row(new { Key = "SYSTEM_INSTALLED", Value = "0" });
        }

        public override void Down()
        {
            Delete.Table("GlobalSettings");
        }
    }
}