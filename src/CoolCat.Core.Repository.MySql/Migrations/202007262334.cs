using FluentMigrator;

namespace CoolCat.Core.Repository.MySql.Migrations
{
    [Migration(202007262334)]
    public class AddSiteSetting : Migration
    {
        public override void Up()
        {
            Insert.IntoTable("SiteSettings").Row(new { Key = "SiteCSS", Value = "" });
            Insert.IntoTable("SiteSettings").Row(new { Key = "SiteTemplateId", Value = "" });
        }

        public override void Down()
        {
            Delete.FromTable("SiteSettings").Row(new {Key = "SiteCSS"});
            Delete.FromTable("SiteSettings").Row(new {Key = "SiteTemplateId"});
        }
    }
}