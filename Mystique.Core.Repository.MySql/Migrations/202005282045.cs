using FluentMigrator;

namespace Mystique.Core.Repository.MySql.Migrations
{
    [Migration(202005282045)]
    public class InitialDB : Migration
    {
        public override void Up()
        {
            Create.Table("Plugins")
              .WithColumn("PluginId").AsGuid().PrimaryKey()
              .WithColumn("UniqueKey").AsString().NotNullable()
              .WithColumn("Name").AsString().NotNullable()
              .WithColumn("DisplayName").AsString().NotNullable()
              .WithColumn("Version").AsString().NotNullable()
              .WithColumn("Enable").AsInt16().NotNullable();

            Create.Table("PluginMigrations")
                .WithColumn("PluginId").AsGuid().PrimaryKey().ForeignKey("FK_PluginMigrations_PluginId_Plugins_PluginId", "Plugins", "PluginId")
                .WithColumn("Version").AsString().NotNullable()
                .WithColumn("Up").AsCustom("text")
                .WithColumn("Down").AsCustom("text");


        }

        public override void Down()
        {
            Delete.Table("PluginMigrations");
            Delete.ForeignKey("FK_PluginMigrations_PluginId_Plugins_PluginId");
            Delete.Table("Plugins");

        }
    }
}