using System.Data.Entity.Migrations;
using MySql.Data.EntityFramework;

namespace Mikos.XK.Fiscal.Migrations
{
    internal sealed class XkConfiguration : DbMigrationsConfiguration<FiscalContext>
    {
        public XkConfiguration()
        {
            base.AutomaticMigrationsEnabled = true;
            base.AutomaticMigrationDataLossAllowed = true;
            SetSqlGenerator("MySql.Data.MySqlClient", new MySqlMigrationSqlGenerator());
        }

        protected override void Seed(FiscalContext context)
        {
        }
    }
}
