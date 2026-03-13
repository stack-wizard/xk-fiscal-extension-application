using Micros.Ops;
using Micros.PosCore.Extensibility;
using Micros.PosCore.Extensibility.DataStore;
using Mikos.XK.Fiscal.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using static Mikos.XK.Fiscal.Application;

namespace Mikos.XK.Fiscal.Util
{
    public class FiscalConfigUtil
    {
        public static CisConfiguration ReadCisConfiguration(OpsContext opsContext, DataStoreClient dataStore, string ApplicationName)
        {
            try
            {
                IEnumerable<XElement> enumerable = from item in XDocument.Parse(dataStore.ReadExtensionApplicationContentTextByNameKey(opsContext.RvcID, ApplicationName, "FiscalConfig")).Descendants("cis")
                                                   select (item);

                //IEnumerable<XElement> enumerable = from item in XDocument.Load(dllLocationPath + "\\FiscalConfig.xml").Descendants("cis")
                //                                   select (item);
                CisConfiguration cisConfiguration = new CisConfiguration();
                foreach (XElement item in enumerable)
                {
                    cisConfiguration.PropertyCode = item?.Element("PropertyCode")?.Value;
                    if (string.IsNullOrEmpty(cisConfiguration.PropertyCode))
                    {
                        opsContext.ShowError("Error reading Fiscal Configuration from the Database. Please contact Support.");
                    }
                    cisConfiguration.PrinterId = item?.Element("PrinterId")?.Value;
                    if (string.IsNullOrEmpty(cisConfiguration.PrinterId))
                    {
                        opsContext.ShowError("Error reading Fiscal Configuration from the Database. Please contact Support.");
                    }
                    cisConfiguration.PropertyTaxNumber = item?.Element("PropertyTaxNumber")?.Value;
                    if (string.IsNullOrEmpty(cisConfiguration.PropertyTaxNumber))
                    {
                        opsContext.ShowError("Error reading Fiscal Configuration from the Database. Please contact Support.");
                    }
                    cisConfiguration.PropertyCode = item?.Element("PropertyCode")?.Value;
                    if (string.IsNullOrEmpty(cisConfiguration.PropertyCode))
                    {
                        opsContext.ShowError("Error reading Fiscal Configuration from the Database. Please contact Support.");
                    }
                    cisConfiguration.SellerName = item?.Element("SellerName")?.Value;
                    if (string.IsNullOrEmpty(cisConfiguration.SellerName))
                    {
                        opsContext.ShowError("Error reading Fiscal Configuration from the Database. Please contact Support.");
                    }
                    cisConfiguration.SellerOwner = item?.Element("SellerOwner")?.Value;
                    if (string.IsNullOrEmpty(cisConfiguration.SellerOwner))
                    {
                        opsContext.ShowError("Error reading Fiscal Configuration from the Database. Please contact Support.");
                    }
                    cisConfiguration.SellerAddr = item?.Element("SellerAddr")?.Value;
                    if (string.IsNullOrEmpty(cisConfiguration.SellerAddr))
                    {
                        opsContext.ShowError("Error reading Fiscal Configuration from the Database. Please contact Support.");
                    }
                    cisConfiguration.SellerCity = item?.Element("SellerCity")?.Value;
                    if (string.IsNullOrEmpty(cisConfiguration.SellerCity))
                    {
                        opsContext.ShowError("Error reading Fiscal Configuration from the Database. Please contact Support.");
                    }
                    cisConfiguration.SellerCountry = item?.Element("SellerCountry")?.Value;
                    if (string.IsNullOrEmpty(cisConfiguration.SellerCountry))
                    {
                        opsContext.ShowError("Error reading Fiscal Configuration from the Database. Please contact Support.");
                    }
                    cisConfiguration.SellerCurrencyCode = item?.Element("SellerCurrencyCode")?.Value;
                    if (string.IsNullOrEmpty(cisConfiguration.SellerCurrencyCode))
                    {
                        opsContext.ShowError("Error reading Fiscal Configuration from the Database. Please contact Support.");
                    }
                }
                return cisConfiguration;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string ReadServiceUrlBase(OpsContext opsContext, DataStoreClient dataStore, string ApplicationName)
        {
            try
            {
                IEnumerable<XElement> enumerable = from item in XDocument.Parse(dataStore.ReadExtensionApplicationContentTextByNameKey(opsContext.RvcID, ApplicationName, "FiscalConfig")).Descendants("service")
                                                   select (item);

                string result = "";
                foreach (XElement item in enumerable)
                {
                    result = item?.Element("url")?.Value;
                    if (string.IsNullOrEmpty(result))
                    {
                        opsContext.ShowError("Error reading Fiscal Service URL from the Database. Please contact Support.");
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static string ReadFiscalPrinter(OpsContext opsContext, DataStoreClient dataStore, string applicationName)
        {
            try
            {
                var configXml = XDocument.Parse(dataStore.ReadExtensionApplicationContentTextByNameKey(opsContext.RvcID, applicationName, "FiscalConfig"));
                var fiscalPrinterElement = configXml.Descendants("global").Elements("fiscalPrinter").FirstOrDefault();

                if (fiscalPrinterElement != null && !string.IsNullOrEmpty(fiscalPrinterElement.Value))
                {
                    return fiscalPrinterElement.Value;
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static PrinterSettings ReadPrinterSettings(OpsContext opsContext, DataStoreClient dataStore, string ApplicationName)
        {
            try
            {
                IEnumerable<XElement> enumerable = from item in XDocument.Parse(dataStore.ReadExtensionApplicationContentTextByNameKey(opsContext.RvcID, ApplicationName, "FiscalConfig")).Descendants("printer")
                                                   select (item);
                PrinterSettings printerSettings = null;
                foreach (XElement item in enumerable)
                {
                    printerSettings = new PrinterSettings();
                    XElement xElement = item.Element("encoding");
                    if (xElement != null)
                    {
                        printerSettings.encoding = xElement.Value;
                    }
                    XElement xElement2 = item.Element("lineSeparator");
                    if (xElement2 != null)
                    {
                        printerSettings.lineSeparator = xElement2.Value;
                    }
                    return printerSettings;
                }

                if (printerSettings == null)
                {
                    return new PrinterSettings
                    {
                        encoding = "1251",
                        lineSeparator = "\n",
                    };
                }

                return printerSettings;
            }
            catch (Exception ex)
            {
                return new PrinterSettings
                {
                    encoding = "1521",
                    lineSeparator = "\n",
                };
            }
        }

        public static string ReadFolderPath(OpsContext opsContext, DataStoreClient dataStore, string ApplicationName)
        {
            try
            {
                IEnumerable<XElement> enumerable = from item in XDocument.Parse(dataStore.ReadExtensionApplicationContentTextByNameKey(opsContext.RvcID, ApplicationName, "FiscalConfig")).Descendants("service")
                                                   select (item);

                string result = null;
                foreach (XElement item in enumerable)
                {
                    result = item?.Element("folder")?.Value;
                    if (string.IsNullOrEmpty(result))
                    {
                        opsContext.ShowError("Error reading Fiscal Service folder path from the Database. Please contact Support.");
                    }
                    else
                    {
                        if (!System.IO.Directory.Exists(result))
                        {
                            System.IO.Directory.CreateDirectory(result);
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                opsContext.ShowError($"Error reading Fiscal Service folder path from the Database.\nPlease contact the Support with the following message: {ex.Message}");
                return null;
            }
        }

        public static List<PaymentMethod> GetTenderMediaSettings(OpsContext opsContext, DataStoreClient dataStore, string ApplicationName)
        {
            try
            {
                var enumerable = from item in XDocument.Parse(dataStore.ReadExtensionApplicationContentTextByNameKey(opsContext.RvcID, ApplicationName, "FiscalConfig"))
                                 .Descendants("payment")
                                 select (item);

                //IEnumerable<XElement> enumerable = from item in XDocument.Load(dllLocationPath + "\\FiscalConfig.xml").Descendants("payment") select (item);
                List<PaymentMethod> list = new List<PaymentMethod>();
                foreach (XElement item in enumerable)
                {
                    PaymentMethod paymentMethod = new PaymentMethod();
                    XElement xElement = item.Element("type");
                    if (xElement != null)
                    {
                        paymentMethod.Type = xElement.Value;
                    }
                    XElement xElement2 = item.Element("number");
                    if (xElement2 != null)
                    {
                        paymentMethod.ObjectNumber = Convert.ToInt32(xElement2.Value);
                    }
                    XElement xElement3 = item.Element("code");
                    if (xElement3 != null)
                    {
                        paymentMethod.Code = xElement3.Value;
                    }
                    list.Add(paymentMethod);
                }
                return list;
            }
            catch (Exception ex)
            {
                opsContext.ShowError("Error reading Fiscal Payments Configuration from the Database. Please contact Support.\n" + ex.Message);
                return null;
            }
        }

        public static List<SalesItemizerDescription> GetSalesItemizerDescriptions(OpsContext opsContext, DataStoreClient dataStore, string ApplicationName)
        {
            try
            {
                var enumerable = from item in XDocument.Parse(dataStore.ReadExtensionApplicationContentTextByNameKey(opsContext.RvcID, ApplicationName, "FiscalConfig"))
                                 .Descendants("itemGroup")
                                 select (item);

                //IEnumerable<XElement> enumerable = from item in XDocument.Load(dllLocationPath + "\\FiscalConfig.xml").Descendants("payment") select (item);
                List<SalesItemizerDescription> list = new List<SalesItemizerDescription>();
                foreach (XElement item in enumerable)
                {
                    SalesItemizerDescription salesItemizerDescription = new SalesItemizerDescription();
                    XElement xElement = item.Element("salesItemizerIndex");
                    if (xElement != null)
                    {
                        salesItemizerDescription.SalesItemizerIndex = Convert.ToInt32(xElement.Value);
                    }
                    XElement xElement2 = item.Element("description");
                    if (xElement2 != null)
                    {
                        salesItemizerDescription.Description = xElement2.Value;
                    }

                    list.Add(salesItemizerDescription);
                }
                return list;
            }
            catch (Exception ex)
            {
                opsContext.ShowError("Error reading Sales Itemizer Descriptions from the Database. Please contact Support.\n" + ex.Message);
                return null;
            }
        }

        public static List<TaxMapping> GetTaxClassSettings(OpsContext opsContext, DataStoreClient dataStore, string ApplicationName)
        {
            try
            {
                var enumerable = from item in XDocument.Parse(dataStore.ReadExtensionApplicationContentTextByNameKey(opsContext.RvcID, ApplicationName, "FiscalConfig"))
                                 .Descendants("tax")
                                 select (item);

                //IEnumerable<XElement> enumerable = from item in XDocument.Load(dllLocationPath + "\\FiscalConfig.xml").Descendants("payment") select (item);
                List<TaxMapping> list = new List<TaxMapping>();
                foreach (XElement item in enumerable)
                {
                    TaxMapping taxMapping = new TaxMapping();
                    XElement xElement = item.Element("percent");
                    if (xElement != null)
                    {
                        taxMapping.Percent = xElement.Value;
                    }
                    XElement xElement2 = item.Element("number");
                    if (xElement2 != null)
                    {
                        taxMapping.ObjectNumber = Convert.ToInt32(xElement2.Value);
                    }
                    XElement xElement4 = item.Element("code");
                    if (xElement4 != null)
                    {
                        taxMapping.Code = xElement4.Value;
                    }
                    list.Add(taxMapping);
                }
                return list;
            }
            catch (Exception ex)
            {
                opsContext.ShowError("Error reading Tax Class Configuration from the Database. Please contact Support.\n" + ex.Message);
                return null;
            }
        }

        public static bool UsePayrollId(OpsContext opsContext, DataStoreClient dataStore, string ApplicationName)
        {
            try
            {
                using (IEnumerator<XElement> enumerator = (from item in XDocument.Parse(dataStore.ReadExtensionApplicationContentTextByNameKey(opsContext.RvcID, ApplicationName, "FiscalConfig")).Descendants("global")
                                                           select (item) into el
                                                           select el.Element("usepayrollid") into xElementPayrollId
                                                           where xElementPayrollId != null
                                                           select xElementPayrollId).GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        if (bool.TryParse(enumerator.Current.Value, out var result))
                        {
                            return result;
                        }
                        return true;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return true;
            }
        }

        public static bool RefiscalizeOnShutdown(OpsContext opsContext, DataStoreClient dataStore, string ApplicationName)
        {
            try
            {
                using (IEnumerator<XElement> enumerator = (from item in XDocument.Parse(dataStore.ReadExtensionApplicationContentTextByNameKey(opsContext.RvcID, ApplicationName, "FiscalConfig")).Descendants("global")
                                                           select (item) into el
                                                           select el.Element("refiscalizeOnShutdown") into xElementRefiscalize
                                                           where xElementRefiscalize != null
                                                           select xElementRefiscalize).GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        if (bool.TryParse(enumerator.Current.Value, out var result))
                        {
                            return result;
                        }
                        return false;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static TransactionEmployeeType getTransactionType(OpsContext opsContext, DataStoreClient dataStore, string ApplicationName)
        {
            TransactionEmployeeType result = TransactionEmployeeType.PID;
            try
            {
                using (IEnumerator<XElement> enumerator = (from item in XDocument.Parse(dataStore.ReadExtensionApplicationContentTextByNameKey(opsContext.RvcID, ApplicationName, "FiscalConfig")).Descendants("global")
                                                           select (item) into el
                                                           select el.Element("transactiontype") into xTransactionType
                                                           where xTransactionType != null
                                                           select xTransactionType).GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        if (Enum.TryParse<TransactionEmployeeType>(enumerator.Current.Value, ignoreCase: true, out result))
                        {
                            return result;
                        }
                        return TransactionEmployeeType.PID;
                    }
                }
                return TransactionEmployeeType.PID;
            }
            catch (Exception ex)
            {
                return result;
            }
        }
    }
}
