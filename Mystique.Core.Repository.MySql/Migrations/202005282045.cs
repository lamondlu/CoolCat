using System;
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
              .WithColumn("Version").AsString().NotNullable()
              .WithColumn("Enable").AsInt16().NotNullable();

            
        }

        public override void Down()
        {
            Delete.Table("Plugins");
        }
    }
}