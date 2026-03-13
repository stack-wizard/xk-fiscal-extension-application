using Micros.Ops;
using Mikos.XK.Fiscal.Dtos;
using Mikos.XK.Fiscal.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mikos.XK.Fiscal.Util
{
    public class RequestMappingUtil
    {
        public static DocumentInfo MapDocumentInfo(OpsContext opsContext, CisConfiguration cisConfiguration, string currentDateTimeFormat, bool isVoid)
        {
            return new DocumentInfo()
            {
                HotelCode = cisConfiguration.PropertyCode,
                BillNo = isVoid ? (int.MaxValue - opsContext.CheckNumber).ToString() : opsContext.CheckNumber.ToString(),
                FolioType = "FISCAL",
                TerminalId = opsContext.Check.Guid,
                ProgramName = "0",
                FiscalFolioId = "0",
                OperaFiscalBillNo = null,
                Application = "Simphony",
                PropertyTaxNumber = cisConfiguration.PropertyTaxNumber,
                BankName = "",
                BankCode = "",
                BusinessDate = DateTime.Now.ToString("yyyy-MM-dd"),
                BusinessDateTime = currentDateTimeFormat,
                CountryCode = cisConfiguration.SellerCountry,
                CountryName = cisConfiguration.SellerCountry,
                Command = "INVOICE",
                FiscalTimeoutPeriod = "30",
                LastSupportingDocumentInfo = null
            };
        }



        public static FiscalTerminalInfo MapFiscalTerminalInfo(CisConfiguration cisConfiguration)
        {
            return new FiscalTerminalInfo
            {
                TerminalID = cisConfiguration.PrinterId
            };
        }

        public static FolioInfo MapFolioInfo(OpsContext opsContext, CisConfiguration cisConfiguration, ItemsPlaceholderObject itemsList, string currentDateTimeFormat, bool isVoid)
        {
            return new FolioInfo()
            {
                FolioHeaderInfo = new FolioHeaderInfo()
                {
                    BillGenerationDate = currentDateTimeFormat,
                    FolioType = "FISCAL",
                    CreditBill = false,
                    FolioNo = isVoid ? (int.MaxValue - opsContext.CheckNumber).ToString() : opsContext.CheckNumber.ToString(),
                    BillNo = isVoid ? (int.MaxValue - opsContext.CheckNumber).ToString() : opsContext.CheckNumber.ToString(),
                    InvoiceCurrencyCode = cisConfiguration.SellerCurrencyCode,
                    InvoiceCurrencyRate = "1",
                    Window = "4",
                    CashierNumber = "",//this.OpsContext.WorkstationNumber.ToString(),
                    FiscalFolioStatus = "OK",
                    LocalBillGenerationDate = currentDateTimeFormat,
                    CollectingAgentTaxes = null,
                    FolioTypeUniqueCode = "0",
                    AssociatedFiscalTerminalInfo = null,
                    AssociatedFolioInfo = null
                },
                Postings = itemsList.postings,
                RevenueBucketInfo = itemsList.revenueBucketInfos,
                TotalInfo = itemsList.totalInfo,
                TrxInfo = itemsList.TrxInfo
            };
        }

        public static HotelInfo MapHotelInfo(CisConfiguration cisConfiguration, string currentDateTimeFormat)
        {
            return new HotelInfo()
            {
                HotelCode = cisConfiguration.PropertyCode,
                HotelName = cisConfiguration.SellerName,
                LegalOwner = cisConfiguration.SellerOwner,
                Address = new Addresses()
                {
                    Address = cisConfiguration.SellerAddr,
                    City = cisConfiguration.SellerCity,
                    Country = cisConfiguration.SellerCountry

                },
                LocalCurrency = cisConfiguration.SellerCurrencyCode,
                Decimals = "2",
                TimeZoneRegion = "Europe/Tirane",
                PhoneNo = "+000 000 0000 00",
                Email = "info@symp.rks",
                WebPage = "www.symp.rks",
                ExchangeRates = { },
                PropertyDateTime = currentDateTimeFormat
            };
        }

        public static ReservationInfo MapReservationInfo(OpsContext opsContext)
        {
            ReservationInfo reservationInfo = new ReservationInfo()
            {
                ResvNameID = opsContext.Check.Guid
            };

            return reservationInfo;
        }

        public static FiscalFolioUserInfo MapFiscalFolioUserInfo(OpsContext opsContext)
        {
            return new FiscalFolioUserInfo()
            {
                AppUser = opsContext.TransEmployeeFullName,
                AppUserId = opsContext.TransEmployeeID.ToString(),
                CashierId = opsContext.TransEmployeeID.ToString()
            };
        }

        public static decimal CalculateZeroTaxTotal(List<Posting> postings)
        {
            return postings.Where(posting => posting.TrxType == "C" && posting.GuestAccountDebit != 0).Where(posting => posting.Generates.Generate[0].TaxRate == 0.0).Sum(posting => posting.GrossAmount);
        }
        public static UserDefinedFields MapUserDefinedFields(OpsContext opsContext, string fiscalPrinter)
        {
            if (!string.IsNullOrEmpty(fiscalPrinter))
            {
                return new UserDefinedFields()
                {
                    CharacterUDFs = new List<CharacterUDF>()
            {
                new CharacterUDF()
                {
                    UDF = new List<UDF>()
                    {
                        new UDF()
                        {
                            Name = "FISCAL_PRINTER",
                            Value = fiscalPrinter
                        }
                    }
                }
            }
                };
            }
            else
            {
                return null;
            }
        }
    }
}
