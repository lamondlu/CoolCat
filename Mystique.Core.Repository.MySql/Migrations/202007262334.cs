using FluentMigrator;

namespace Mystique.Core.Repository.MySql.Migrations
{
    [Migration(202007262334)]
    public class AddSiteSetting : Migration
    {
        public override void Up()
        {
            Insert.IntoTable("GlobalSettings").Row(new { Key = "SiteCSS", Value = "" });
            Insert.IntoTable("GlobalSettings").Row(new { Key = "SiteTemplateId", Value = "" });
        }

        public override void Down()
        {
            Delete.FromTable("GlobalSettings").Row(new {Key = "SiteCSS"});
            Delete.FromTable("GlobalSettings").Row(new {Key = "SiteTemplateId"});
        }
    }
}