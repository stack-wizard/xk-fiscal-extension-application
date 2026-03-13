using Micros.Ops;
using Mikos.XK.Fiscal.Dtos;
using System.Collections.Generic;

namespace Mikos.XK.Fiscal.Util
{
    public class TaxTotalsUtil
    {
        public static List<Tax> calculateTaxes(List<Posting> postings, OpsContext opsContext)
        {
            SimTaxCalc(opsContext);
            List<Tax> taxList = new List<Tax>();

            decimal ZeroTaxTotal = RequestMappingUtil.CalculateZeroTaxTotal(postings);

            if (Mikos.XK.Fiscal.TaxesByTaxRates.Tax1Vat != 0.0m)
                taxList.Add(new Tax(1, Mikos.XK.Fiscal.TaxesByTaxRates.Tax1Vat, Mikos.XK.Fiscal.TaxesByTaxRates.Tax1Net, Mikos.XK.Fiscal.TaxesByTaxRates.Tax1Name, ""));
            if (Mikos.XK.Fiscal.TaxesByTaxRates.Tax2Vat != 0.0m)
                taxList.Add(new Tax(2, Mikos.XK.Fiscal.TaxesByTaxRates.Tax2Vat, Mikos.XK.Fiscal.TaxesByTaxRates.Tax2Net, Mikos.XK.Fiscal.TaxesByTaxRates.Tax2Name, ""));
            if (Mikos.XK.Fiscal.TaxesByTaxRates.Tax3Vat != 0.0m)
                taxList.Add(new Tax(3, Mikos.XK.Fiscal.TaxesByTaxRates.Tax3Vat, Mikos.XK.Fiscal.TaxesByTaxRates.Tax3Net, Mikos.XK.Fiscal.TaxesByTaxRates.Tax3Name, ""));

            if (ZeroTaxTotal != 0.0m)
            {
                Mikos.XK.Fiscal.TaxesByTaxRates.Tax4Net = ZeroTaxTotal;
                taxList.Add(new Tax(4, Mikos.XK.Fiscal.TaxesByTaxRates.Tax4Vat, ZeroTaxTotal, Mikos.XK.Fiscal.TaxesByTaxRates.Tax4Name, ""));
            }


            return taxList;
        }
        private static void SimTaxCalc(OpsContext opsContext)
        {
            OpsCommand command = new OpsCommand
            {
                Command = OpsCommandType.SimInquire,
                Arguments = "Mikos.XK.Fiscal:GetTaxTotals"
            };

            opsContext.ProcessCommand(command);
        }

        public static void ReceiveTaxTotals(string tax1Name, decimal tax1Net, decimal tax1Vat, string tax2Name, decimal tax2Net, decimal tax2Vat, string tax3Name, decimal tax3Net, decimal tax3Vat, string tax4Name, decimal tax4Net, decimal tax4Vat)
        {
            Mikos.XK.Fiscal.TaxesByTaxRates.Tax1Net = 0m;
            Mikos.XK.Fiscal.TaxesByTaxRates.Tax1Vat = 0m;
            Mikos.XK.Fiscal.TaxesByTaxRates.Tax2Net = 0m;
            Mikos.XK.Fiscal.TaxesByTaxRates.Tax2Vat = 0m;
            Mikos.XK.Fiscal.TaxesByTaxRates.Tax3Net = 0m;
            Mikos.XK.Fiscal.TaxesByTaxRates.Tax3Vat = 0m;
            if (!string.IsNullOrEmpty(tax1Name))
            {
                Mikos.XK.Fiscal.TaxesByTaxRates.Tax1Name = tax1Name;
                Mikos.XK.Fiscal.TaxesByTaxRates.Tax1Net = tax1Net;
                Mikos.XK.Fiscal.TaxesByTaxRates.Tax1Vat = tax1Vat;
            }
            if (!string.IsNullOrEmpty(tax2Name))
            {
                Mikos.XK.Fiscal.TaxesByTaxRates.Tax2Name = tax2Name;
                Mikos.XK.Fiscal.TaxesByTaxRates.Tax2Net = tax2Net;
                Mikos.XK.Fiscal.TaxesByTaxRates.Tax2Vat = tax2Vat;
            }
            if (!string.IsNullOrEmpty(tax3Name))
            {
                Mikos.XK.Fiscal.TaxesByTaxRates.Tax3Name = tax3Name;
                Mikos.XK.Fiscal.TaxesByTaxRates.Tax3Net = tax3Net;
                Mikos.XK.Fiscal.TaxesByTaxRates.Tax3Vat = tax3Vat;
            }
            if (!string.IsNullOrEmpty(tax4Name))
            {
                Mikos.XK.Fiscal.TaxesByTaxRates.Tax4Name = tax4Name;
                Mikos.XK.Fiscal.TaxesByTaxRates.Tax4Net = tax4Net;
                Mikos.XK.Fiscal.TaxesByTaxRates.Tax4Vat = tax4Vat;
            }
            if (tax1Vat != 0.0m || tax2Vat != 0.0m || tax3Vat != 0.0m || tax4Net != 0.0m)
            {
                Mikos.XK.Fiscal.TaxesByTaxRates.hasTaxes = true;
            }
            else
            {
                Mikos.XK.Fiscal.TaxesByTaxRates.hasTaxes = false;
            }
        }
    }
}
