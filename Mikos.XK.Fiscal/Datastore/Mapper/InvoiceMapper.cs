using Micros.Ops;
using Micros.PosCore.Common.Classes;
using Mikos.XK.Fiscal.Dtos;
using Mikos.XK.Fiscal.Model;
using Mikos.XK.Fiscal.Util;
using Mikos.XK.Fiscal.Datastore.Dao;
using Mikos.XK.Fiscal.Datastore.Enums;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Linq;
using static Micros.Ops.OpsAskYesNoCancelRequest;
using static Mikos.XK.Fiscal.Application;

namespace Mikos.XK.Fiscal.Datastore.Mapper
{
    public class InvoiceMapper
    {
        public static FiscalInvoice ToFiscalInvoice(
            OpsContext opsContext,
            FiscalData fiscalData,
            FiscalResponseData result,
            FiscalInvoice fiscalInvoice,
            FiscalRequestData request,
            RequestType requestType,
            bool isVoid,
            CisConfiguration cisConfiguration,
            string employeeVatId)
        {
            if (fiscalInvoice == null)
            {
                FiscalInvoice toSave = new FiscalInvoice()
                {
                    RequestType = requestType,
                    TaxNumber = cisConfiguration.PropertyTaxNumber,
                    CheckNumber = opsContext.CheckNumber,
                    EmployeeVatId = employeeVatId,
                    SpecialId = fiscalData.FiscalBillNo,
                    ChkClosedDateTime = DateTime.Now,
                    RvcNumber = opsContext.RvcNumber.ToString(),
                    WorkstationNumber = opsContext.WorkstationNumber.ToString(),
                    Queued = string.IsNullOrEmpty(fiscalData.FiscalBillNo),
                    Error = string.IsNullOrEmpty(fiscalData.FiscalBillNo),
                    UpdateDateTime = DateTime.Now,
                    ErrorDesc = string.IsNullOrEmpty(fiscalData.FiscalBillNo) ? formatErrorDescription(result.StatusMessages.Messages[0].Description) : null,
                    ResponseMessage = result.StatusMessages.Messages[0].Description,
                    PaymentTotal = Math.Round(request.FolioInfo.TotalInfo.GrossAmount, 2),
                    ChkGUID = opsContext.Check.Guid,
                    PaymentMethod = DeterminePaymentMethodFromRequest(request),
                    Base64Request = ConvertToBase64(request),
                    Void = isVoid
                };

                if (!string.IsNullOrEmpty(fiscalData.FiscalBillNo))
                {
                    toSave.SyncDateTime = DateTime.Now;
                }

                if (result != null)
                {
                    toSave.ErrorDesc = string.IsNullOrEmpty(fiscalData.FiscalBillNo) ? result.StatusMessages.Messages[0].Description : null;
                    toSave.ResponseMessage = result.StatusMessages.Messages[0].Description;
                }
                else
                {
                    toSave.ErrorDesc = "Request not sent to fiscal service.";
                    toSave.ResponseMessage = "No response.";
                }

                return toSave;
            }
            else
            {
                if (!string.IsNullOrEmpty(fiscalData.FiscalBillNo))
                {
                    fiscalInvoice.SyncDateTime = DateTime.Now;
                }
                fiscalInvoice.SpecialId = fiscalData.FiscalBillNo;
                fiscalInvoice.UpdateDateTime = DateTime.Now;
                fiscalInvoice.Queued = string.IsNullOrEmpty(fiscalData.FiscalBillNo);
                fiscalInvoice.Error = string.IsNullOrEmpty(fiscalData.FiscalBillNo);
                fiscalInvoice.ErrorDesc = string.IsNullOrEmpty(fiscalData.FiscalBillNo) ? formatErrorDescription(result.StatusMessages.Messages[0].Description) : null;
                fiscalInvoice.ResponseMessage = result.StatusMessages.Messages[0].Description;
                fiscalInvoice.EmployeeVatId = employeeVatId;
                fiscalInvoice.PaymentTotal = isVoid ? decimal.Negate(Math.Round(opsContext.Check.Payment)) : Math.Round(opsContext.Check.Payment);
                fiscalInvoice.Base64Request = ConvertToBase64(request);

                return fiscalInvoice;
            }
        }

        internal static FiscalInvoice ToFiscalInvoiceToSync(FiscalInvoice fiscalInvoice, FiscalResponseData response)
        {
            var doc1NoValue = response?.FiscalOutputs?.Output?.Find(qRCodeBase => qRCodeBase.Name == "DOCUMENT_NO_1")?.Value;

            FiscalData fiscalData = new FiscalData
            {
                FiscalBillNo = doc1NoValue?.ToString(),
                FiscalBillGenerationDateTime = response.FiscalBillGenerationDateTime
            };

            if (fiscalInvoice == null)
            {
                FiscalInvoice toSave = new FiscalInvoice()
                {
                    SpecialId = fiscalData.FiscalBillNo,
                    Queued = string.IsNullOrEmpty(fiscalData.FiscalBillNo),
                    Error = string.IsNullOrEmpty(fiscalData.FiscalBillNo),
                    UpdateDateTime = DateTime.Now,
                    ChkGUID = response.chkGuid,
                };

                if (!string.IsNullOrEmpty(fiscalData.FiscalBillNo))
                {
                    toSave.SyncDateTime = DateTime.Now;
                }

                if (response != null)
                {
                    toSave.ErrorDesc = string.IsNullOrEmpty(fiscalData.FiscalBillNo) ? response.StatusMessages.Messages[0].Description : null;
                    toSave.ResponseMessage = response.StatusMessages.Messages[0].Description;
                }
                else
                {
                    toSave.ErrorDesc = "Request not sent to fiscal service.";
                    toSave.ResponseMessage = "No response.";
                }

                return toSave;
            }
            else
            {
                if (!string.IsNullOrEmpty(fiscalData.FiscalBillNo))
                {
                    fiscalInvoice.SyncDateTime = DateTime.Now;
                }
                fiscalInvoice.SpecialId = fiscalData.FiscalBillNo;
                fiscalInvoice.UpdateDateTime = DateTime.Now;
                fiscalInvoice.Queued = string.IsNullOrEmpty(fiscalData.FiscalBillNo);
                fiscalInvoice.Error = string.IsNullOrEmpty(fiscalData.FiscalBillNo);
                fiscalInvoice.ErrorDesc = string.IsNullOrEmpty(fiscalData.FiscalBillNo) ? response.StatusMessages.Messages[0].Description : null;
                fiscalInvoice.ResponseMessage = response.StatusMessages.Messages[0].Description;

                return fiscalInvoice;
            }
        }

        private static string formatErrorDescription(string errorDescription)
        {
            if (!string.IsNullOrEmpty(errorDescription))
            {
                if (errorDescription.Length > 100)
                    return errorDescription.Substring(0, 100);
            }
            return errorDescription;
        }

        private static string ConvertToBase64(FiscalRequestData request)
        {
            var json = JsonConvert.SerializeObject(request);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            return Convert.ToBase64String(bytes);

        }

        public static FiscalRequestData ConvertBase64ToFiscalRequestData(string base64String)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64String);
            var json = Encoding.UTF8.GetString(base64EncodedBytes);
            return JsonConvert.DeserializeObject<FiscalRequestData>(json);
        }

        private static PaymentType DeterminePaymentMethodFromRequest(FiscalRequestData request)
        {
            PaymentType result = PaymentType.None;

            if (request.FolioInfo.RevenueBucketInfo != null)
            {
                string bucketValue = request.FolioInfo.RevenueBucketInfo.Where(rbi => rbi.BucketType.Equals("FLIP_PAY_SUBTYPE")).FirstOrDefault().BucketValue;
                if (Enum.TryParse<PaymentType>(bucketValue, ignoreCase: true, out result))
                {
                    return result;
                }
            }

            return result;
        }

        public static FiscalInvoice CopyFiscalInvoice(FiscalInvoice fiscalInvoice)
        {
            return new FiscalInvoice
            {
                Id = fiscalInvoice.Id,
                RequestType = fiscalInvoice.RequestType,
                SpecialId = fiscalInvoice.SpecialId,
                TaxNumber = fiscalInvoice.TaxNumber,
                ChkClosedDateTime = fiscalInvoice.ChkClosedDateTime,
                RvcNumber = fiscalInvoice.RvcNumber,
                WorkstationNumber = fiscalInvoice.WorkstationNumber,
                CheckNumber = fiscalInvoice.CheckNumber,
                EmployeeVatId = fiscalInvoice.EmployeeVatId,
                Queued = fiscalInvoice.Queued,
                SyncDateTime = fiscalInvoice.SyncDateTime,
                Error = fiscalInvoice.Error,
                Void = true,
                ErrorCode = fiscalInvoice.ErrorCode,
                ErrorDesc = fiscalInvoice.ErrorDesc,
                PaymentMethod = fiscalInvoice.PaymentMethod,
                PaymentTotal = fiscalInvoice.PaymentTotal,
                InsertDateTime = fiscalInvoice.InsertDateTime,
                UpdateDateTime = fiscalInvoice.UpdateDateTime,
                ChkGUID = fiscalInvoice.ChkGUID,
                ResponseMessage = fiscalInvoice.ResponseMessage,
                Base64Request = fiscalInvoice.Base64Request
            };
        }
    }
}
