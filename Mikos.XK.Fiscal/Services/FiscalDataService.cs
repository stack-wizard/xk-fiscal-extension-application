using Micros.Ops;
using System;
using Mikos.XK.Fiscal.Datastore.Dao;
using Mikos.XK.Fiscal.Datastore.Enums;
using Mikos.XK.Fiscal.Datastore.Mapper;
using Mikos.XK.Fiscal.Datastore.Repository;
using Mikos.XK.Fiscal.Dtos;
using Mikos.XK.Fiscal.Model;
using Mikos.XK.Fiscal.Migrations;

namespace Mikos.XK.Fiscal.Services
{
    public class FiscalDataService
    {
        public void SaveInvoiceResponseToDb(
            FiscalResponseData result,
            OpsContext opsContext,
            FiscalData fiscalData,
            CisConfiguration cisConfiguration,
            FiscalRequestData request,
            RequestType requestType,
            long invoiceId,
            string employeeVatId,
            bool isVoid)
        {
            try
            {
                using (FiscalContext fiscalContext = new FiscalContext())
                {
                    FiscalInvoiceRepository fiscalInvoiceRepository = new FiscalInvoiceRepository(fiscalContext);
                    bool update = false;
                    FiscalInvoice existingFiscalInvoice = null;
                    if (invoiceId != 0)
                    {
                        existingFiscalInvoice = fiscalInvoiceRepository.GetById(invoiceId);
                    }
                    else
                    {
                        existingFiscalInvoice = fiscalInvoiceRepository.GetByCheckGuidAndRequestType(request.ReservationInfo.ResvNameID, requestType);
                    }

                    if (existingFiscalInvoice != null)
                    {
                        update = true;
                    }
                    FiscalInvoice fiscalInvoice = InvoiceMapper.ToFiscalInvoice(opsContext, fiscalData, result, existingFiscalInvoice, request, requestType, isVoid, cisConfiguration, employeeVatId);

                    fiscalInvoice.TableNumber = opsContext.CheckTableName
                        ?? (opsContext.CheckTableNumber > 0
                            ? opsContext.CheckTableNumber.ToString()
                            : null);

                    if (update)
                    {
                        fiscalInvoiceRepository.Update(fiscalInvoice);
                    }
                    else
                    {
                        fiscalInvoiceRepository.Insert(fiscalInvoice);
                    }
                    fiscalInvoiceRepository.Save();
                }
            }
            catch (Exception ex)
            {
                opsContext.ShowError("Failed to save an invoice to local database: [" + ex.Message + "]");
            }
        }

        public FiscalInvoice retrieveFiscalInvoiceById(long chkId, OpsContext opsContext)
        {
            try
            {
                using (FiscalContext fiscalContext = new FiscalContext())
                {
                    FiscalInvoiceRepository fiscalInvoiceRepository = new FiscalInvoiceRepository(fiscalContext);
                    return fiscalInvoiceRepository.GetById(chkId);
                }
            }
            catch (Exception ex)
            {
                opsContext.ShowError($"Failed to retireve an invoice with ID: {chkId}. [ {ex.Message}]");
                return null;
            }
        }

        public void MarkInvoiceAsVoided(FiscalInvoice fiscalInvoice, OpsContext opsContext)
        {
            try
            {
                using (FiscalContext fiscalContext = new FiscalContext())
                {
                    FiscalInvoiceRepository fiscalInvoiceRepository = new FiscalInvoiceRepository(fiscalContext);
                    fiscalInvoice.Voided = true;
                    fiscalInvoiceRepository.Update(fiscalInvoice);
                    fiscalInvoiceRepository.Save();
                }
            }
            catch (Exception ex)
            {
                opsContext.ShowError($"Update of the invoice with ID: {fiscalInvoice.Id} has failed.\nError message: {ex.Message}");
            }
        }
    }
}
