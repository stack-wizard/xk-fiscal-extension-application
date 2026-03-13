using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Mikos.XK.Fiscal.Datastore.Enums;
using Mikos.XK.Fiscal.Model;

namespace Mikos.XK.Fiscal.Datastore.Dao
{
    public class FiscalInvoice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public RequestType RequestType { get; set; }
        [StringLength(100)]
        public string SpecialId { get; set; }

        [StringLength(13)]
        public string TaxNumber { get; set; }

        public DateTime? ChkClosedDateTime { get; set; }

        [StringLength(20)]
        public string RvcNumber { get; set; }

        [StringLength(20)]
        public string WorkstationNumber { get; set; }

        public long CheckNumber { get; set; }

        [StringLength(13)]
        public string EmployeeVatId { get; set; }

        [Required]
        [DefaultValue(1)]
        public bool Queued { get; set; } = true;

        public DateTime? SyncDateTime { get; set; }

        [Required]
        [DefaultValue(0)]
        public bool Error { get; set; } = false;

        [Required]
        public bool Void { get; set; } = false;

        public bool Voided { get; set; }

        [StringLength(10)]
        public string ErrorCode { get; set; }

        [StringLength(256)]
        public string ErrorDesc { get; set; }

        public PaymentType PaymentMethod { get; set; }

        public decimal PaymentTotal { get; set; }

        public DateTime? InsertDateTime { get; set; } = DateTime.Now;

        public DateTime? UpdateDateTime { get; set; }

        [StringLength(100)]
        public string ChkGUID { get; set; }
        [StringLength(1000)]
        public string ResponseMessage { get; set; }

        public string Base64Request { get; set; }
    }
}
