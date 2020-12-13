using FluentMigrator;

namespace CoolCat.Core.Repository.MySql.Migrations
{
    [Tags("System")]
    [Migration(202007202310)]
    public class AddGlobalSettings : Migration
    {
        public override void Up()
        {
            Create.Table("SiteSettings")
              .WithColumn("Key").AsString().PrimaryKey()
              .WithColumn("Value").AsCustom("text").NotNullable();

            Insert.IntoTable("SiteSettings").Row(new { Key = "SYSTEM_INSTALLED", Value = "0" });
        }

        public override void Down()
        {
            Delete.Table("SiteSettings");
        }
    }
}