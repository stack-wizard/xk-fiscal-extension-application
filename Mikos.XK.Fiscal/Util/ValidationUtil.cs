using Micros.Ops;
using Mikos.XK.Fiscal.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mikos.XK.Fiscal.Util
{
    public class ValidationUtil
    {
        public static bool ValidateBuyerData(BuyerEntryBoxData buyerInfo, OpsContext opsContext)
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrEmpty(buyerInfo.Identification))
            {
                errors.Add("Buyer data does not contain the identification!");
            }

            if (string.IsNullOrEmpty(buyerInfo.Address))
            {
                errors.Add("Buyer data does not contain the address!");
            }

            if (string.IsNullOrEmpty(buyerInfo.Name))
            {
                errors.Add("Buyer data does not contain the name!");
            }

            if (errors.Any())
            {
                opsContext.ShowError(BuyerInfoValidationErrorMessage(errors));
                return false;
            }
            else
            {
                return true;
            }
        }

        private static string BuyerInfoValidationErrorMessage(List<string> errors)
        {
            StringBuilder sb = new StringBuilder("Buyer data contains the following errors:");
            foreach (string error in errors)
            {
                sb.Append($"\n{error}");
            }
            return sb.ToString();
        }
    }
}
