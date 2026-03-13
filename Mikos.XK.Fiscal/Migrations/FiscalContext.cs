using Mikos.XK.Fiscal.Datastore.Dao;
using System.Data.Entity;

namespace Mikos.XK.Fiscal.Migrations
{
    public class FiscalContext : DbContext
    {
        public DbSet<FiscalInvoice> FiscalInvoices { get; set; }

        public FiscalContext()
            : base("MIKOS_FISCAL_XK")
        {
            var created = base.Database.CreateIfNotExists();
            var exists = base.Database.Exists();
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<FiscalContext, XkConfiguration>());
        }
    }
}
