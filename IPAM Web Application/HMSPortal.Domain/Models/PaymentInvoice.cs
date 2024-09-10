using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Domain.Models
{
    public class PaymentInvoice : BaseIndividual
    {
		public Guid PatientId { get; set; }
		[ForeignKey("PatientId")]
		public Patient? Patient { get; set; }

		public Guid DoctorId { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string? Phone { get; set; }
        public decimal? UnitCost { get; set; }
        public decimal Tax { get; set; }
        public decimal GrandTotal { get; set; }
        public string Note { get; set; }
        public int? TotalDays { get; set; }
        public int? InvoiceNumber { get; set; }
    }
}
