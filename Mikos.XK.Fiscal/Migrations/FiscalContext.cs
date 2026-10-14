using Mikos.XK.Fiscal.Datastore.Dao;
using System.Data.Entity;
using MySql.Data.EntityFramework;

namespace Mikos.XK.Fiscal.Migrations
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class FiscalContext : DbContext
    {
        public DbSet<FiscalInvoice> FiscalInvoices { get; set; }

        public FiscalContext()
            : base(FiscalConnectionFactory.CreateFiscalConnection(), true)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<FiscalContext, XkConfiguration>());
        }
    }
}
