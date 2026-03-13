using Mikos.XK.Fiscal.Datastore.Dao;
using Mikos.XK.Fiscal.Datastore.Enums;
using Mikos.XK.Fiscal.Migrations;
using System.Linq;

namespace Mikos.XK.Fiscal.Datastore.Repository
{
    public class FiscalInvoiceRepository : Repository<FiscalInvoice>, IFiscalInvoiceRepository
    {
        public FiscalInvoiceRepository(FiscalContext context) : base(context)
        {
        }

        public FiscalInvoice GetByCheckNumber(int checkNumber)
        {
            return _context.FiscalInvoices.Where(invoice => invoice.CheckNumber == checkNumber)
                   .OrderByDescending(invoice => invoice.SyncDateTime)
                   .FirstOrDefault();
        }
        public FiscalInvoice GetByCheckGuid(string checkGuid)
        {
            return _context.FiscalInvoices.FirstOrDefault(invoice => invoice.ChkGUID.Equals(checkGuid.Trim()));
        }

        public FiscalInvoice GetByCheckGuidAndRequestType(string checkGuid, RequestType requestType)
        {
            return _context.FiscalInvoices.FirstOrDefault(invoice => invoice.ChkGUID.Equals(checkGuid.Trim()) && invoice.RequestType == requestType);
        }

        public FiscalInvoice GetByCheckGuidWithVoidCheck(string checkGuid, bool isVoid)
        {
            return _context.FiscalInvoices.FirstOrDefault(invoice => invoice.ChkGUID.Equals(checkGuid.Trim()) && invoice.Void == isVoid);
        }
        public FiscalInvoice GetVoidByCheckNumber(int checkNumber)
        {
            return _context.FiscalInvoices.Where(i => i.CheckNumber == checkNumber && i.Void)
                   .OrderByDescending(invoice => invoice.SyncDateTime)
                   .FirstOrDefault();
        }
    }
}
