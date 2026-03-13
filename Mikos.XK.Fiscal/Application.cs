using Micros.Ops;
using Micros.Ops.Extensibility;
using Micros.PosCore.Extensibility;
using Micros.PosCore.Extensibility.Ops;
using Mikos.XK.Fiscal.Datastore.Dao;
using Mikos.XK.Fiscal.Datastore.Enums;
using Mikos.XK.Fiscal.Datastore.Mapper;
using Mikos.XK.Fiscal.Dtos;
using Mikos.XK.Fiscal.Migrations;
using Mikos.XK.Fiscal.Model;
using Mikos.XK.Fiscal.Services;
using Mikos.XK.Fiscal.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mikos.XK.Fiscal
{
    public class Application : OpsExtensibilityApplication
    {
        public enum TransactionEmployeeType
        {
            PID,
            PIN
        }

        public enum RetransmitType
        {
            Undefined = -1,
            FiscalInvoice,
            Corrective
        }

        private const string racunkoAPI = "/invoices/fiscalize";
        private string racunkoUrlBase;
        private string fiscalPrinter;
        private FiscalData fiscalData;
        private PrinterSettings printerSettings;
        private FiscalDataService fiscalDataService;
        /// <summary>
        /// Extension application constructor
        /// </summary>
        /// <param name="context">the execution context for the application</param>
        public Application(IExecutionContext context)
            : base(context)
        {
            //Add initialization code and hook up event handlers here
            this.OpsFinalTenderEvent += Application_FinalTenderEvent;
            this.OpsVoidClosedCheckEventPreview += Application_VoidClosedCheckEventPreview;
            this.OpsWorkstationDownEvent += Application_WorkstationDownEvent;
            this.fiscalDataService = new FiscalDataService();
            racunkoUrlBase = FiscalConfigUtil.ReadServiceUrlBase(this.OpsContext, base.DataStore, ApplicationName);
            fiscalPrinter = FiscalConfigUtil.ReadFiscalPrinter(this.OpsContext, base.DataStore, ApplicationName);
            printerSettings = FiscalConfigUtil.ReadPrinterSettings(this.OpsContext, base.DataStore, ApplicationName);
        }

        private EventProcessingInstruction Application_FinalTenderEvent(object sender, OpsTmedEventArgs args)
        {
            try
            {
                if (!this.OpsContext.TrainingModeEnabled)
                {
                    FiscalRequestData invoice = FillFiscalRequestInitData(false);
                    fiscalData = InitializeFiscalData();

                    if (invoice != null)
                    {
                        var result = System.Threading.Tasks.Task.Run(async () => await sendInvoiceToRacunko(invoice)).Result;
                        ProcessResult(result, invoice, 0, false, false);
                    }
                }
            }
            catch (Exception ex)
            {
                this.OpsContext.ShowMessage(ex.Message);
            }

            return EventProcessingInstruction.Continue;
        }

        private EventProcessingInstruction Application_VoidClosedCheckEventPreview(object sender, OpsVoidCheckEventArgs args)
        {
            try
            {
                if (!this.OpsContext.TrainingModeEnabled)
                {
                    FiscalRequestData invoice = FillFiscalRequestInitData(true);
                    fiscalData = InitializeFiscalData();

                    if (invoice != null)
                    {
                        var result = System.Threading.Tasks.Task.Run(async () => await sendInvoiceToRacunko(invoice)).Result;
                        ProcessResult(result, invoice, 0, true, false);
                    }
                }

            }
            catch (System.Exception ex)
            {
                this.OpsContext.ShowMessage(ex.Message);
            }

            return EventProcessingInstruction.Continue;
        }

        private EventProcessingInstruction Application_WorkstationDownEvent(object sender, OpsWorkstationDownEventArgs args)
        {
            try
            {
                if (!this.OpsContext.TrainingModeEnabled)
                {
                    if (FiscalConfigUtil.RefiscalizeOnShutdown(this.OpsContext, base.DataStore, ApplicationName))
                    {
                        bool hasLeftoverInvoices = false;
                        long count = 0;
                        using (FiscalContext fiscalContext = new FiscalContext())
                        {
                            List<FiscalInvoice> list = fiscalContext.FiscalInvoices.Where((FiscalInvoice m) => (m.Queued || m.Error) && m.Base64Request != null).ToList();
                            if (list.Count > 0)
                            {
                                hasLeftoverInvoices = true;
                                count = list.Count;
                            }
                        }

                        if (hasLeftoverInvoices)
                        {
                            var refiscalize = this.OpsContext.AskQuestion($"Workstation is shutting down. \nThere are {count} leftover invoices to fiscalize.\nThis process can take up to a couple of minutes.");
                            if (refiscalize)
                            {
                                var result = System.Threading.Tasks.Task.Run(async () => await Refiscalize("shutdown"));
                                result.Wait();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                this.OpsContext.ShowMessage(ex.Message);
            }

            return EventProcessingInstruction.Continue;
        }

        private FiscalData InitializeFiscalData()
        {
            return new FiscalData()
            {
                FiscalBillGenerationDateTime = "",
                FiscalBillNo = ""
            };
        }

        private void ProcessResult(FiscalResponseData result, FiscalRequestData request, long entryId, bool isVoid, bool isShutdown)
        {
            if (result != null)
            {
                if (result.StatusMessages.Messages[0].Type.Equals("Error") && !isShutdown)
                {
                    base.OpsContext.ShowError(ShowMessages(result.StatusMessages.Messages));
                }

                var Doc1NoValue = result?.FiscalOutputs?.Output?.Find(qRCodeBase => qRCodeBase.Name == "DOCUMENT_NO_1")?.Value;

                fiscalData = new FiscalData
                {
                    FiscalBillGenerationDateTime = result.FiscalBillGenerationDateTime,
                    FiscalBillNo = Doc1NoValue?.ToString()
                };

                if (IsEligibleToBeStored(result))
                {
                    CisConfiguration cisConfiguration = FiscalConfigUtil.ReadCisConfiguration(this.OpsContext, base.DataStore, ApplicationName);
                    fiscalDataService.SaveInvoiceResponseToDb(result, base.OpsContext, fiscalData, cisConfiguration, request, DetermineRequestType(request), entryId, EmployeeUtil.GetEmployeeId(this.OpsContext, base.DataStore, ApplicationName), isVoid);
                }
            }
        }

        private bool IsEligibleToBeStored(FiscalResponseData result)
        {
            return !result.StatusMessages.Messages[0].Description.Contains("Invoice does not require fiscalization.");
        }

        private RequestType DetermineRequestType(FiscalRequestData request)
        {
            if (request.FolioInfo.TotalInfo.GrossAmount < 0)
            {
                return RequestType.REFUND;
            }

            return RequestType.NORMAL;
        }

        private string ShowMessages(List<Message> Messages)
        {
            StringBuilder sb = new StringBuilder();

            foreach (Message message in Messages)
            {
                sb.AppendLine(message.Description.Replace("hotel", "property"));
            }

            return sb.ToString();
        }

        private FiscalRequestData FillFiscalRequestInitData(bool isVoid)
        {
            FiscalRequestData invoice = new FiscalRequestData();

            CisConfiguration cisConfiguration = FiscalConfigUtil.ReadCisConfiguration(this.OpsContext, base.DataStore, ApplicationName);

            if (string.IsNullOrEmpty(cisConfiguration.PropertyTaxNumber))
            {
                this.OpsContext.ShowError("Property tax number is invalid. Please, contact the customer support.");
            }

            ItemsPlaceholderObject itemsList = fillInvoiceItemData(cisConfiguration.SellerCurrencyCode, isVoid);

            if (itemsList == null)
            {
                return null; //this invoice does not require fiscalization
            }

            string currentDateTimeFormat = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

            invoice.DocumentInfo = RequestMappingUtil.MapDocumentInfo(this.OpsContext, cisConfiguration, currentDateTimeFormat, isVoid);
            invoice.UserDefinedFields = RequestMappingUtil.MapUserDefinedFields(this.OpsContext, fiscalPrinter);
            invoice.FiscalTerminalInfo = RequestMappingUtil.MapFiscalTerminalInfo(cisConfiguration);
            invoice.FolioInfo = RequestMappingUtil.MapFolioInfo(this.OpsContext, cisConfiguration, itemsList, currentDateTimeFormat, isVoid);
            invoice.HotelInfo = RequestMappingUtil.MapHotelInfo(cisConfiguration, currentDateTimeFormat);
            invoice.ReservationInfo = RequestMappingUtil.MapReservationInfo(this.OpsContext);
            invoice.FiscalFolioUserInfo = RequestMappingUtil.MapFiscalFolioUserInfo(this.OpsContext);

            if (!string.IsNullOrEmpty(fiscalPrinter))
            {
                if (fiscalPrinter.Equals("tremol", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (invoice?.FolioInfo?.TotalInfo?.GrossAmount < 0)
                    {
                        OpsCommandUtil.TransactionCancel(this.OpsContext);
                        return null;
                    }
                }
            }

            return invoice;
        }

        private ItemsPlaceholderObject fillInvoiceItemData(string currency, bool isVoid)
        {
            ItemsPlaceholderObject tempItemsList = new ItemsPlaceholderObject()
            {
                postings = new List<Posting>(),
                revenueBucketInfos = new List<RevenueBucketInfo>(),
                TrxInfo = new List<TrxInfo>(),
                totalInfo = new TotalInfo()
            };
            List<PaymentMethod> tenderMediaSettings = FiscalConfigUtil.GetTenderMediaSettings(this.OpsContext, base.DataStore, ApplicationName);
            List<TaxMapping> taxClassSettings = FiscalConfigUtil.GetTaxClassSettings(this.OpsContext, base.DataStore, ApplicationName);
            Dictionary<int, int> itemTrxNoAgaintsMenuItemNo = new Dictionary<int, int>();

            foreach (Micros.PosCore.Extensibility.Ops.CheckDetailItem item in OpsContext.CheckDetail.Where((Micros.PosCore.Extensibility.Ops.CheckDetailItem c) => !c.LineNumVoid))
            {
                if (((int)item.DetailType == 1) || ((int)item.DetailType == 2))
                {
                    tempItemsList = InvoiceDetailsMappingUtil.MapItem(
                        tempItemsList,
                        item,
                        itemTrxNoAgaintsMenuItemNo,
                        taxClassSettings,
                        isVoid,
                        currency,
                        null
                    );

                    var condimentsProperty = item.GetType().GetProperty("Condiments");
                    var condiments = condimentsProperty?.GetValue(item) as Micros.PosCore.Extensibility.Ops.OpsExtensibilityDetailArray<Micros.PosCore.Extensibility.Ops.MenuItemDetail>;

                    if (condiments != null)
                    {
                        var miObjectNumber = (int)item.DetailType == 1 ? item.GetType().GetProperty("MiObjNum") : null;
                        var condimentTrxNo = miObjectNumber.GetValue(item);
                        condiments.ToList().ForEach(c =>
                        {
                            if (c.Total > 0.0m)
                            {
                                tempItemsList = InvoiceDetailsMappingUtil.MapItem(
                                    tempItemsList,
                                    c,
                                    itemTrxNoAgaintsMenuItemNo,
                                    taxClassSettings,
                                    isVoid,
                                    currency,
                                    condimentTrxNo.ToString()
                                );
                            }
                            return;
                        });
                    }

                    var comboSidesProperty = item.GetType().GetProperty("ComboSides");
                    var comboSides = comboSidesProperty?.GetValue(item) as Micros.PosCore.Extensibility.Ops.OpsExtensibilityDetailArray<Micros.PosCore.Extensibility.Ops.MenuItemDetail>;

                    if (comboSides != null)
                    {
                        var miObjectNumber = (int)item.DetailType == 1 ? item.GetType().GetProperty("MiObjNum") : null;
                        var comboMealTrxNo = miObjectNumber.GetValue(item);

                        comboSides.ToList().ForEach(cs =>
                        {
                            if (cs.Total > 0.0m)
                            {
                                tempItemsList = InvoiceDetailsMappingUtil.MapItem(
                                    tempItemsList,
                                    cs,
                                    itemTrxNoAgaintsMenuItemNo,
                                    taxClassSettings,
                                    isVoid,
                                    currency,
                                    comboMealTrxNo.ToString()
                                );
                            }

                            condimentsProperty = cs.GetType().GetProperty("Condiments");
                            condiments = condimentsProperty?.GetValue(cs) as Micros.PosCore.Extensibility.Ops.OpsExtensibilityDetailArray<Micros.PosCore.Extensibility.Ops.MenuItemDetail>;

                            if (condiments != null)
                            {
                                var csCondimentTrxNo = miObjectNumber.GetValue(item);
                                condiments.ToList().ForEach(c =>
                                {
                                    if (c.Total > 0.0m)
                                    {
                                        tempItemsList = InvoiceDetailsMappingUtil.MapItem(
                                            tempItemsList,
                                            c,
                                            itemTrxNoAgaintsMenuItemNo,
                                            taxClassSettings,
                                            isVoid,
                                            currency,
                                            csCondimentTrxNo.ToString()
                                        );
                                    }
                                    return;
                                });
                            }

                            return;
                        });
                    }
                }
                else if ((int)item.DetailType == 3)
                {
                    tempItemsList = InvoiceDetailsMappingUtil.MapServiceCharge(
                        tempItemsList,
                        item,
                        taxClassSettings,
                        isVoid,
                        currency
                    );
                }
                else if ((int)item.DetailType == 4)
                {
                    tempItemsList = InvoiceDetailsMappingUtil.MapTenderMedia(
                        tempItemsList,
                        item,
                        tenderMediaSettings,
                        isVoid,
                        currency
                    );
                }
            }

            if (isVoid)
            {
                tempItemsList.postings.Add(new Posting()
                {
                    TrxType = "FC",
                    Reference = "VOID",
                    TrxDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    TrxDateTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                    LocalTrxDateTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                });
            }

            tempItemsList.postings = InvoiceDetailsMappingUtil.ApplyDiscounts(tempItemsList.postings);
            tempItemsList.revenueBucketInfos = InvoiceDetailsMappingUtil.ApplyDiscountsToRbi(tempItemsList.postings, tempItemsList.revenueBucketInfos);
            tempItemsList.totalInfo = InvoiceDetailsMappingUtil.MapTotalInfo(tempItemsList.postings, TaxTotalsUtil.calculateTaxes(tempItemsList.postings, base.OpsContext));

            return tempItemsList;
        }

        //private EventProcessingInstruction Application_CustomReceiptEvent(object sender, OpsCustomReceiptEventArgs args)
        //{
        //    CisConfiguration cisConfiguration = FiscalConfigUtil.ReadCisConfiguration(this.OpsContext, base.DataStore, ApplicationName);

        //args.HeaderAction = (CustomPrintType)3;
        //args.TrailerAction = (CustomPrintType)3;

        //    args.HeaderAction = (CustomPrintType)4;
        //    args.TrailerAction = (CustomPrintType)4;

        //    args.CustomHeader = ReceiptPrintUtil.AddHeader(cisConfiguration, this.OpsContext);
        //    args.CustomTrailer = ReceiptPrintUtil.AddTrailer(fiscalData, args.CustomTrailer);

        //    return EventProcessingInstruction.Continue;
        //}

        private async Task<FiscalResponseData> sendInvoiceToRacunko(FiscalRequestData invoice)
        {
            var result = await ApiHelper.PostAsync(invoice, racunkoUrlBase + racunkoAPI);

            return result;
        }

        [ExtensibilityMethod]
        public async Task Refiscalize(object arg)
        {
            bool isShutdown = arg is string str && str == "shutdown";

            if (!isShutdown)
            {
                this.OpsContext.StartProgressRequest("Please wait", "Invoices are fiscalizing.", 0);
            }

            using (FiscalContext fiscalContext = new FiscalContext())
            {
                List<FiscalInvoice> invoices = fiscalContext.FiscalInvoices.Where((FiscalInvoice m) => (m.Queued || m.Error) && !m.Void && m.Base64Request != null).ToList();
                List<FiscalInvoice> correctiveInvoices = fiscalContext.FiscalInvoices.Where((FiscalInvoice m) => (m.Queued || m.Error) && m.Void && m.Base64Request != null).ToList();
                invoices.ForEach(invoice => {
                    FiscalRequestData request = InvoiceMapper.ConvertBase64ToFiscalRequestData(invoice.Base64Request);


                    var result = System.Threading.Tasks.Task.Run(async () => await sendInvoiceToRacunko(request)).Result;
                    ProcessResult(result, request, invoice.Id, invoice.Void, isShutdown);
                });
                correctiveInvoices.ForEach(invoice =>
                {
                    FiscalRequestData request = InvoiceMapper.ConvertBase64ToFiscalRequestData(invoice.Base64Request);


                    var result = System.Threading.Tasks.Task.Run(async () => await sendInvoiceToRacunko(request)).Result;
                    ProcessResult(result, request, invoice.Id, invoice.Void, isShutdown);
                });
            }

            if (!isShutdown)
            {
                this.OpsContext.EndProgressRequest();
                return;
            }
        }

        [ExtensibilityMethod]
        public void Retransmit(object arg)
        {
            try
            {
                switch (getTypeFromArgs(arg))
                {
                    case RetransmitType.Undefined:
                        base.OpsContext.ShowError($"Argument inconsistencies for Invoice Retransmit [{arg}]");
                        break;
                    case RetransmitType.FiscalInvoice:
                        retransmitFiscalInvoice();
                        break;
                    case RetransmitType.Corrective:
                        retransmitCorrectiveInvoices();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception ex)
            {
                base.OpsContext.EndProgressRequest();
                base.OpsContext.ShowError("An error occured while retransmitting the invoice:\n" + ex.Message + " \r\n " + ex.InnerException?.Message);
            }
        }

        private void retransmitFiscalInvoice()
        {
            using (FiscalContext fiscalContext = new FiscalContext())
            {
                IList<OpsSelectionEntry> list = (from fr in fiscalContext.FiscalInvoices.Where((FiscalInvoice m) => (m.Queued || m.Error) && !m.Void).ToList()
                                                 select new OpsSelectionEntry(fr.Id, $"CHK#: {fr.CheckNumber} | {fr.RvcNumber}-{fr.WorkstationNumber} | {fr.SyncDateTime} | {fr.ChkGUID}")).ToList();
                int? num = base.OpsContext.SelectionRequest("Retransmit invoices", "Select an invoice to retransmit:", list);
                if (!num.HasValue)
                {
                    return;
                }
                OpsSelectionEntry opsSelectionEntry = list[Convert.ToInt32(num)];
                long id = opsSelectionEntry.Number;
                FiscalInvoice fiscalInvoice = fiscalDataService.retrieveFiscalInvoiceById(opsSelectionEntry.Number, this.OpsContext);

                if (fiscalInvoice == null)
                {
                    return;
                }

                FiscalRequestData request = InvoiceMapper.ConvertBase64ToFiscalRequestData(fiscalInvoice.Base64Request);


                var result = System.Threading.Tasks.Task.Run(async () => await sendInvoiceToRacunko(request)).Result;
                ProcessResult(result, request, fiscalInvoice.Id, fiscalInvoice.Void, false);
            }
        }

        private void retransmitCorrectiveInvoices()
        {
            using (FiscalContext fiscalContext = new FiscalContext())
            {
                IList<OpsSelectionEntry> list = (from fr in fiscalContext.FiscalInvoices.Where((FiscalInvoice m) => (m.Queued || m.Error) && m.Void).ToList()
                                                 select new OpsSelectionEntry(fr.Id, $"CHK#: {fr.CheckNumber} | {fr.RvcNumber}-{fr.WorkstationNumber} | {fr.SyncDateTime} | {fr.ChkGUID}")).ToList();
                int? num = base.OpsContext.SelectionRequest("Retransmit corrective invoices", "Select a corrective invoice to retransmit:", list);
                if (!num.HasValue)
                {
                    return;
                }
                OpsSelectionEntry opsSelectionEntry = list[Convert.ToInt32(num)];
                long id = opsSelectionEntry.Number;
                FiscalInvoice fiscalInvoice = fiscalDataService.retrieveFiscalInvoiceById(opsSelectionEntry.Number, this.OpsContext);

                if (fiscalInvoice == null)
                {
                    return;
                }

                FiscalRequestData request = InvoiceMapper.ConvertBase64ToFiscalRequestData(fiscalInvoice.Base64Request);


                var result = System.Threading.Tasks.Task.Run(async () => await sendInvoiceToRacunko(request)).Result;
                ProcessResult(result, request, fiscalInvoice.Id, fiscalInvoice.Void, false);
            }
        }

        private RetransmitType getTypeFromArgs(object args)
        {
            RetransmitType result = RetransmitType.Undefined;
            if (!Enum.TryParse<RetransmitType>(args.ToString(), ignoreCase: true, out result))
            {
                throw new Exception($"Retransmit operation inconsistencies with argument [{args}]");
            }
            return result;
        }

        [ExtensibilityMethod]
        public void CorrectInvoice(object arg)
        {
            using (FiscalContext fiscalContext = new FiscalContext())
            {
                DateLimit dateLimit = GetDisplayLimit();
                IList<OpsSelectionEntry> list = (from fr in fiscalContext.FiscalInvoices.Where((FiscalInvoice m) => !m.Queued && !m.Error && !m.Voided && !m.Void && !string.IsNullOrEmpty(m.SpecialId) && (m.ChkClosedDateTime > dateLimit.limit && m.ChkClosedDateTime <= dateLimit.now)).ToList()
                                                 select new OpsSelectionEntry(fr.Id, $"CHK#: {fr.CheckNumber} | {fr.ChkClosedDateTime} | {fr.SpecialId} | {fr.RvcNumber}-{fr.WorkstationNumber} | {fr.SyncDateTime} | {fr.ChkGUID}")).ToList();
                int? num = base.OpsContext.SelectionRequest("Corrective invoice", "Select an invoice to correct:", list);
                if (!num.HasValue)
                {
                    return;
                }
                OpsSelectionEntry opsSelectionEntry = list[Convert.ToInt32(num)];
                long id = opsSelectionEntry.Number;
                FiscalInvoice fiscalInvoice = fiscalDataService.retrieveFiscalInvoiceById(opsSelectionEntry.Number, this.OpsContext);

                if (fiscalInvoice == null)
                {
                    return;
                }
                try
                {
                    if (fiscalInvoice.SpecialId != null)
                    {
                        if (!fiscalInvoice.SpecialId.Equals("0"))
                        {
                            OpsCommandUtil.VoidClosedCheck(base.OpsContext, fiscalInvoice.ChkGUID);
                            //OpsCommand voidCheckByGuid = new OpsCommand(OpsCommandType.VoidClosedCheckByGuid)
                            //{
                            //    Data = fiscalInvoice.ChkGUID.Trim()
                            //};
                            //base.OpsContext.ProcessCommand(voidCheckByGuid);
                            fiscalDataService.MarkInvoiceAsVoided(fiscalInvoice, this.OpsContext);
                        }
                    }

                }
                catch (Exception ex)
                {
                    this.OpsContext.ShowError($"Corrective invoice attempt has failed.\nError message: {ex.Message}");
                }
            }
        }

        private DateLimit GetDisplayLimit()
        {
            DateTime now = DateTime.Now;

            // Get the date 7 days ago
            DateTime sevenDaysAgo = now.AddDays(-7);

            // Set the time to 3:00 AM for that date
            DateTime limit = new DateTime(sevenDaysAgo.Year, sevenDaysAgo.Month, sevenDaysAgo.Day, 3, 0, 0);

            // Return both now and limit as a tuple
            return new DateLimit
            {
                now = now,
                limit = limit
            };
        }

        public override void Destroy()
        {
            base.OpsFinalTenderEvent -= Application_FinalTenderEvent;
            this.OpsVoidClosedCheckEventPreview -= Application_VoidClosedCheckEventPreview;
            this.OpsWorkstationDownEvent -= Application_WorkstationDownEvent;
            this.Destroy();
        }
    }
}
