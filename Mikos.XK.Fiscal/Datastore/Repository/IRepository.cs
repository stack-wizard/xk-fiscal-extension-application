using Mikos.XK.Fiscal.Datastore.Dao;
using Mikos.XK.Fiscal.Datastore.Enums;
using System.Collections.Generic;

namespace Mikos.XK.Fiscal.Datastore.Repository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T GetById(long id);
        void Insert(T entity);
        void Update(T entity);
        void Delete(long id);
        void Save();
    }

    public interface IFiscalInvoiceRepository : IRepository<FiscalInvoice>
    {
        FiscalInvoice GetByCheckNumber(int checkNumber);

        FiscalInvoice GetVoidByCheckNumber(int checkNumber);

        FiscalInvoice GetByCheckGuid(string checkGuid);
        FiscalInvoice GetByCheckGuidAndRequestType(string checkGuid, RequestType requestType);

        FiscalInvoice GetByCheckGuidWithVoidCheck(string checkGuid, bool isVoid);
    }
}
