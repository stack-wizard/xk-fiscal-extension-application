namespace Mikos.XK.Fiscal.Model
{
    public class PaymentMethod
    {
        public int ObjectNumber { get; set; }

        public string Type { get; set; }
        public string Code { get; set; }

        public decimal Total { get; set; }
    }
}
