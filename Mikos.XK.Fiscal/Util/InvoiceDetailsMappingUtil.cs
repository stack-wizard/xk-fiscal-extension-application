using Micros.PosCore.Extensibility.Ops;
using System;
using System.Collections.Generic;
using System.Linq;
using Mikos.XK.Fiscal.Dtos;
using Mikos.XK.Fiscal.Model;
using System.Runtime.CompilerServices;
using System.Net.Sockets;

namespace Mikos.XK.Fiscal.Util
{
    public class InvoiceDetailsMappingUtil
    {
        public static ItemsPlaceholderObject MapItem(
            ItemsPlaceholderObject tempItemsList,
            CheckDetailItem item,
            Dictionary<long, long> itemTrxNoAgaintsMenuItemNo,
            List<TaxMapping> taxClassSettings,
            bool isVoid,
            string currency,
            string comboMealRefTrxNo)
        {
            var itemDiscount = false;
            int discountedItemDetailLink = 0;
            string referentTrxNo = null;


            var propertyInfos = item.GetType().GetProperty("TaxData");
            var taxData = propertyInfos?.GetValue(item) as OpsDetailTaxData;

            var itemDetails = (int)item.DetailType == 1 ? item as OpsMenuItemDetail : null;
            var discountDetails = (int)item.DetailType == 2 ? item as OpsDiscountDetail : null;
            if (discountDetails != null)
            {
                var menuitemDetailLink = discountDetails.MenuItemDetailLinks.ToList();
                if (discountDetails.ItmDsc)
                {
                    discountedItemDetailLink = menuitemDetailLink.FirstOrDefault();
                    referentTrxNo = itemTrxNoAgaintsMenuItemNo[discountedItemDetailLink].ToString();
                    itemDiscount = true;
                }
            }


            var miObjectNumber = (int)item.DetailType == 1 ? item.GetType().GetProperty("MiObjNum") : null;
            var dscntID = (int)item.DetailType == 2 ? item.GetType().GetProperty("DscntID") : null;
            var itemTrxNo = miObjectNumber != null ? miObjectNumber.GetValue(item) : null;
            var discountTrxNo = dscntID != null ? dscntID.GetValue(item) : null;
            string trxNo = itemTrxNo == null ? ((int)discountTrxNo).ToString() : ((int)(itemTrxNo)).ToString();

            var taxRateDetails = RetrieveTaxRateDetails(item, taxClassSettings);
            decimal taxRate = Decimal.Parse(taxRateDetails.Percent);
            bool taxExempt = IsTaxExempt(item);


            bool itemHasDiscount = ItemHasDiscount(trxNo, tempItemsList.postings);
            if (itemHasDiscount)
            {
                trxNo = trxNo + "99";
            }

            decimal grossAmount = isVoid ? decimal.Negate(Math.Round(item.Total, 2)) : Math.Round(item.Total, 2);
            decimal netAmount = isVoid ? decimal.Negate(item.Total - taxData.Total) : (item.Total - taxData.Total);

            decimal quantity = itemDetails != null ? itemDetails.SalesCount : discountDetails.SalesCount;
            if (!itemDiscount)
            {
                Posting posting = new Posting()
                {
                    TrxNo = trxNo + item.DetailLink.ToString(),
                    TrxCode = trxNo + item.DetailLink.ToString(),
                    TrxDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    TrxType = itemDetails != null ? "C" : "DSC%" + discountDetails.Percentage.ToString(),
                    UnitPrice = item.Total / quantity,
                    Quantity = ((double)quantity),
                    Currency = currency,
                    TaxInclusive = taxRate != 0,
                    ExchangeRate = 1.0,
                    TrxDateTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                    LocalTrxDateTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                    NetAmount = netAmount,
                    GrossAmount = grossAmount,
                    GuestAccountDebit = grossAmount,
                    Reference = discountDetails != null ? string.Join(",", discountDetails.MenuItemDetailLinks.ToList()) : comboMealRefTrxNo,
                    Generates = MapGenerates(taxData.Total, taxRate, quantity, item.DetailLink.ToString())
                };
                tempItemsList.postings.Add(posting);

                TrxInfo existingTrxInfo = tempItemsList.TrxInfo.Where(trxInfo => trxInfo.Code.Equals(trxNo)).FirstOrDefault();
                if (existingTrxInfo == null && itemDetails != null)
                {
                    tempItemsList.TrxInfo.Add(new TrxInfo()
                    {
                        HotelCode = "POS",
                        Group = "RO",
                        SubGroup = "I",
                        Code = trxNo,
                        TrxType = "C",
                        Description = item.Name,
                        Articles = { },
                        TranslatedDescriptions = { },
                        TrxCodeType = "L"
                    });
                }
            }
            else
            {
                tempItemsList.postings.Add(new Posting()
                {
                    TrxNo = trxNo,
                    TrxCode = trxNo,
                    TrxDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    TrxType = itemDetails != null ? "C" : "DSC%" + discountDetails.Percentage.ToString(),
                    UnitPrice = item.Total / quantity,
                    Quantity = ((double)quantity),
                    Currency = currency,
                    TaxInclusive = taxData.NonTaxable ? false : true,
                    ExchangeRate = (double)(discountDetails.Percentage * 100.0m),
                    TrxDateTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                    LocalTrxDateTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                    NetAmount = netAmount,
                    GrossAmount = grossAmount,
                    GuestAccountDebit = grossAmount,
                    TranActionId = 37276,
                    FinDmlSeqNo = 34096,
                    Generates = null,
                    Reference = discountDetails != null ? string.Join(",", discountDetails.MenuItemDetailLinks.ToList()) : referentTrxNo
                });
            }

            if (itemDetails != null)
            {
                tempItemsList.revenueBucketInfos.Add(new RevenueBucketInfo()
                {
                    BucketCode = trxNo.ToString() + item.DetailLink.ToString(),
                    BucketType = "FLIP_TRX_BY_GRP",
                    BucketValue = trxNo + item.DetailLink.ToString() + "|" + taxRateDetails.Code,
                    Description = item.Name,
                    BucketCodeTotalGross = grossAmount,
                    TrxCode = new List<string>()
                    {
                        trxNo + item.DetailLink.ToString()
                    }
                });
            }

            if (itemDetails != null)
            {
                itemTrxNoAgaintsMenuItemNo.Add(item.DetailLink, long.Parse(trxNo));
            }

            return tempItemsList;
        }
        public static ItemsPlaceholderObject MapServiceCharge(
            ItemsPlaceholderObject tempItemsList,
            CheckDetailItem item,
            List<TaxMapping> taxClassSettings,
            bool isVoid,
            string currency)
        {
            ServiceChargeDetail serviceChargeDetail = item as OpsServiceChargeDetail;
            var propertyInfos = item.GetType().GetProperty("TaxData");
            var taxData = propertyInfos?.GetValue(item) as OpsDetailTaxData;

            var taxRateDetails = RetrieveTaxRateDetails(item, taxClassSettings);
            decimal taxRate = Decimal.Parse(taxRateDetails.Percent);
            bool taxExempt = IsTaxExempt(item);

            string bucketValue = taxRateDetails.Code + "001|" + taxRateDetails.Code;
            string bucketDescription = $"Shërbime {taxRateDetails.Percent}%";
            var serviceChargeTrxNo = serviceChargeDetail.ObjectNumber.ToString();
            var existingServiceCharge = tempItemsList.postings.Find(posting => ItemAlreadyAdded(item, posting, serviceChargeTrxNo));
            var existingVatCategory = tempItemsList.revenueBucketInfos.Find(revenueItem => revenueItem.BucketValue == bucketValue);

            decimal quantity = serviceChargeDetail != null ? (decimal)serviceChargeDetail.SalesCount : 0;
            decimal grossAmount = isVoid ? decimal.Negate(Math.Round(item.Total, 2)) : Math.Round(item.Total, 2);
            decimal netAmount = isVoid ? decimal.Negate(item.Total - taxData.Total) : (item.Total - taxData.Total);


            if (existingServiceCharge == null)
            {
                tempItemsList.postings.Add(new Posting()
                {
                    TrxNo = serviceChargeTrxNo,
                    TrxCode = serviceChargeTrxNo,
                    TrxDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    TrxType = "C",
                    UnitPrice = serviceChargeDetail != null ? item.Total / ((decimal)serviceChargeDetail.SalesCount) : 0.0m,
                    Quantity = (double)quantity,
                    Currency = currency,
                    TaxInclusive = taxData.NonTaxable ? false : true,
                    ExchangeRate = 1.0,
                    TrxDateTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                    LocalTrxDateTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                    NetAmount = netAmount,
                    GrossAmount = grossAmount,
                    GuestAccountDebit = grossAmount,
                    TranActionId = 37276,
                    FinDmlSeqNo = 34096,
                    Generates = MapGenerates(taxData.Total, taxRate, quantity, item.DetailLink.ToString())
                });
            }
            else
            {
                existingServiceCharge.Quantity += serviceChargeDetail != null ? (double)serviceChargeDetail.SalesCount : 0.0;
                existingServiceCharge.NetAmount += netAmount;
                existingServiceCharge.GrossAmount += grossAmount;
                existingServiceCharge.GuestAccountDebit += grossAmount;
                existingServiceCharge.Generates.Generate[0].UnitPrice += taxData.Total;
                existingServiceCharge.Generates.Generate[0].NetAmount += taxData.Total;
            }

            if (existingVatCategory == null)
            {
                tempItemsList.revenueBucketInfos.Add(new RevenueBucketInfo()
                {
                    BucketCode = "ITEM_GROUP",
                    BucketType = "FLIP_TRX_BY_GRP",
                    BucketValue = bucketValue,
                    Description = bucketDescription,//CyrillicsUtil.TranslateToMacedonianCyrillic(item.Name),
                    BucketCodeTotalGross = grossAmount,
                    TrxCode = new List<string>()
                    {
                        serviceChargeTrxNo.ToString()
                    }
                });
            }
            else
            {
                var bucketGrossAmount = existingVatCategory.BucketCodeTotalGross;
                existingVatCategory.BucketCodeTotalGross = Math.Round(bucketGrossAmount + grossAmount);
                string existingTrxCode = existingVatCategory.TrxCode.Where(trxCode => trxCode.Equals(serviceChargeTrxNo.ToString())).FirstOrDefault();
                if (string.IsNullOrEmpty(existingTrxCode))
                {
                    existingVatCategory.TrxCode.Add(serviceChargeTrxNo.ToString());
                }
            }

            TrxInfo existingTrxInfo = tempItemsList.TrxInfo.Where(trxInfo => trxInfo.Code.Equals(serviceChargeTrxNo)).FirstOrDefault();
            if (existingTrxInfo == null)
            {
                tempItemsList.TrxInfo.Add(new TrxInfo()
                {
                    HotelCode = "POS",
                    Group = "RO",
                    SubGroup = "SC",
                    Code = serviceChargeTrxNo,
                    TrxType = "C",
                    Description = bucketDescription,
                    Articles = { },
                    TranslatedDescriptions = { },
                    TrxCodeType = "L"
                });
            }
            return tempItemsList;
        }

        public static ItemsPlaceholderObject MapTenderMedia(
            ItemsPlaceholderObject tempItemsList,
            CheckDetailItem item,
            List<PaymentMethod> tenderMediaSettings,
            bool isVoid,
            string currency)
        {
            OpsTenderMediaDetail opsTenderMediaDetail = item as OpsTenderMediaDetail;

            var type = tenderMediaSettings.Where((PaymentMethod paymentSetting) => opsTenderMediaDetail.ObjectNumber == paymentSetting.ObjectNumber);
            PaymentMethod selectedPaymentMethod = type.Count() != 0 ? type.FirstOrDefault() : new PaymentMethod { Type = "CASH", Code = "0" };

            var existingTenderMedia = tempItemsList.revenueBucketInfos.Find(rbi => TenderMediaAlreadyAdded(opsTenderMediaDetail.ObjectNumber, rbi));
            decimal paymentAmount = isVoid ? decimal.Negate(Math.Round(item.Total, 2)) : Math.Round(item.Total, 2);
            decimal unitPrice = isVoid ? decimal.Negate(Math.Round(item.Total)) : Math.Round(item.Total);
            if (existingTenderMedia == null)
            {
                tempItemsList.postings.Add(new Posting()
                {
                    TrxCode = opsTenderMediaDetail.ObjectNumber.ToString(),
                    TrxDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    TrxType = "FC",
                    UnitPrice = unitPrice,
                    Quantity = 1.0,
                    Currency = currency,
                    TaxInclusive = false,
                    ExchangeRate = 1.0,
                    TrxDateTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                    LocalTrxDateTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                    GrossAmount = paymentAmount,
                    GuestAccountDebit = paymentAmount,
                    Generates = null
                });
                tempItemsList.revenueBucketInfos.Add(new RevenueBucketInfo()
                {
                    BucketCode = opsTenderMediaDetail.ObjectNumber.ToString(),
                    BucketType = "FLIP_PAY_SUBTYPE",
                    BucketValue = "PAY_3",
                    Description = selectedPaymentMethod.Code,
                    BucketCodeTotalGross = paymentAmount,
                    TrxCode = new List<string>()
                        {
                            opsTenderMediaDetail.ObjectNumber.ToString()
                        }
                });
            }
            else
            {
                existingTenderMedia.BucketCodeTotalGross += paymentAmount;
            }
            return tempItemsList;
        }

        public static TotalInfo MapTotalInfo(List<Posting> postings, List<Tax> taxes)
        {
            var invoiceTotalNetAmount = CalcuclateInvoiceNet(postings);
            var totalGross = CalculateInvoiceGross(postings);

            return new TotalInfo()
            {
                NetAmount = invoiceTotalNetAmount,
                GrossAmount = totalGross,
                NonTaxableAmount = 0.0m,
                PaidOut = 0.0m,
                Taxes = new Taxes
                {
                    Tax = taxes
                }
            };
        }

        private static decimal CalculateInvoiceGross(List<Posting> postings)
        {
            return postings.Where(posting => posting.TrxType == "C" && posting.GuestAccountDebit != 0).Sum(posting => posting.GrossAmount);
        }

        private static decimal CalcuclateInvoiceNet(List<Posting> postings)
        {
            return postings.Where(posting => posting.TrxType == "C" && posting.GuestAccountDebit != 0).Sum(posting => posting.NetAmount);
        }

        private static bool ItemHasDiscount(String trxNo, List<Posting> postings)
        {
            foreach (Posting post in postings)
            {
                if (!string.IsNullOrEmpty(post.TrxType))
                {
                    if (post.TrxType.Equals("DSC"))
                    {
                        if (post.Reference.Equals(trxNo))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static bool ItemAlreadyAdded(CheckDetailItem item, Posting posting, string trxNo)
        {
            if (posting.TrxNo.Equals(trxNo))
            {
                decimal salesCount = item.SalesCount;
                if (salesCount == 0.0m)
                {
                    salesCount = item.SalesCount;
                }

                if (Math.Sign(posting.Quantity) == Math.Sign(salesCount))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool TenderMediaAlreadyAdded(int tmdObjectNumber, RevenueBucketInfo rbi)
        {
            if (rbi.BucketCode.Equals(tmdObjectNumber.ToString()))
            {
                return true;
            }
            return false;
        }

        private static Generates MapGenerates(decimal taxAmount, decimal taxRate, decimal quantity, string reference)
        {
            Generates generates = new Generates();

            generates.Generate = new List<Generate>();

            Generate Generate = new Generate
            {
                UnitPrice = taxAmount,
                NetAmount = taxAmount,
                TaxRate = (double)taxRate,
                TaxInclusive = true,
                TrxDateTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                LocalTrxDateTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                Quantity = quantity,
                Reference = reference
            };

            generates.Generate.Add(Generate);

            return generates;
        }



        private static TaxMapping RetrieveTaxRateDetails(CheckDetailItem item, List<TaxMapping> taxClassSettings)
        {
            var taxRatesProperty = item.GetType().GetProperty("TaxRates");
            var taxRates = taxRatesProperty?.GetValue(item) as Boolean[];

            int index = Array.FindIndex(taxRates, b => b);

            var type = taxClassSettings.Where((TaxMapping taxMapping) => (index + 1) == taxMapping.ObjectNumber);
            return type.Count() != 0 ? type.FirstOrDefault() : new TaxMapping { Percent = "0", Code = "3"};
        }

        private static bool IsTaxExempt(CheckDetailItem item)
        {
            var taxRatesProperty = item.GetType().GetProperty("TaxRates");
            var taxRates = taxRatesProperty?.GetValue(item) as Boolean[];

            return !taxRates.FirstOrDefault(tr => tr == true);
        }

        internal static List<Posting> ApplyDiscounts(List<Posting> postings)
        {
            var itemPostings = postings.Where(p => p.TrxType == "C").ToList();
            var discountPostings = postings.Where(p => p.TrxType.StartsWith("DSC") && !string.IsNullOrEmpty(p.Reference)).ToList();
            var finalResult = new List<Posting>();

            foreach (var discount in discountPostings)
            {
                var referencedIds = discount.Reference.Split(',').Select(long.Parse).ToList();
                var referencedItems = itemPostings.Where(p => referencedIds.Contains(long.Parse(p.Generates.Generate[0].Reference))).ToList();

                decimal totalGross = referencedItems.Sum(p => p.GrossAmount);
                if (totalGross == 0) continue;

                decimal totalDiscountAmount = totalGross * (1.00m - decimal.Parse(discount.TrxType.Split('%')[1]));
                totalDiscountAmount = Math.Round(totalDiscountAmount, 2);

                decimal distributed = 0m;

                for (int i = 0; i < referencedItems.Count; i++)
                {
                    var item = referencedItems[i];
                    var proportion = item.GrossAmount / totalGross;
                    decimal thisDiscount = Math.Round(totalDiscountAmount * proportion, 2);

                    // Residual correction on last item
                    if (i == referencedItems.Count - 1)
                    {
                        thisDiscount = totalDiscountAmount - distributed;
                    }

                    distributed += thisDiscount;

                    decimal multiplier = 1.00m - (item.GrossAmount - thisDiscount) / item.GrossAmount;

                    item.GrossAmount = applyDiscount(item.GrossAmount, multiplier);
                    item.NetAmount = applyDiscount(item.NetAmount, multiplier);
                    item.GuestAccountDebit = applyDiscount(item.GuestAccountDebit, multiplier);
                    item.UnitPrice = applyDiscount(item.UnitPrice, multiplier);
                }
            }

            finalResult.AddRange(itemPostings.Where(p => p.TrxType == "C" || p.TrxType == "FC"));
            var finalCharge = postings.FirstOrDefault(p => p.TrxType == "FC");
            if (finalCharge != null) finalResult.Add(finalCharge);

            return finalResult;
        }

        private static decimal applyDiscount(decimal baseAmount, decimal multiplier)
        {
            decimal result = baseAmount * multiplier;

            int firstDecimalDigit = (int)(result * 10) % 10;
            int thirdDecimalDigit = (int)(result * 1000) % 10;

            // Check if even or odd
            if (firstDecimalDigit % 2 == 0 && thirdDecimalDigit == 5)
            {
                return Math.Floor(result * 100) / 100;
            }
            else if (firstDecimalDigit % 2 != 0 && thirdDecimalDigit == 5)
            {
                return Math.Ceiling(result * 100) / 100;
            }
            else
            {
                return Math.Round(result, 2);
            }
        }

        private static string formatTrxNo(string trxNo)
        {
            if (int.TryParse(trxNo, out int number))
            {
                if (number >= 7000)
                {
                    number %= 1000; // keep only the last 3 digits
                }
            }
            else
            {
                number = 100;
            }

            return number.ToString();
        }

        public static List<RevenueBucketInfo> ApplyDiscountsToRbi(List<Posting> postings, List<RevenueBucketInfo> revenueBucketInfos)
        {
            foreach (var rbi in revenueBucketInfos)
            {
                if (rbi.BucketType.Equals("FLIP_TRX_BY_GRP"))
                {
                    decimal totalGross = postings
                        .Where(p => rbi.TrxCode.Contains(p.TrxCode))
                        .Sum(p => p.GrossAmount);

                    rbi.BucketCodeTotalGross = totalGross;
                }
            }

            return revenueBucketInfos;
        }
    }
}
