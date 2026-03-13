using System.Data.Entity.Migrations;

namespace Mikos.XK.Fiscal.Migrations
{
    internal sealed class XkConfiguration : DbMigrationsConfiguration<FiscalContext>
    {
        public XkConfiguration()
        {
            base.AutomaticMigrationsEnabled = true;
            base.AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(FiscalContext context)
        {
        }
    }
}
