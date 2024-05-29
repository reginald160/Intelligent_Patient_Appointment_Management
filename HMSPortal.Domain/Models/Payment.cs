using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Domain.Models
{
	public class Payment : BaseIndividual
	{
		[Required]
		public Guid PatientId { get; set; }
		[ForeignKey("PatientId")]
		public Patient? Patient { get; set; } = null; // Ensure this navigation property exists

		[Required]
		public Guid DoctorId { get; set; }
		[ForeignKey("DoctorId")]
		public Doctor? Doctor { get; set; } = null; // Ensure this navigation property exists

		public Department Department { get; set; }
		public PaymentType PaymentType { get; set; }

		[Required]
		public Guid PaymentInvoiceId { get; set; }
		[ValidateNever]
		[ForeignKey("PaymentInvoiceId")]
		public PaymentInvoice? PaymentInvoice { get; set; } = null; // Ensure this navigation property exists

		public DateTime AdmissionDate { get; set; }
		public DateTime DischargeDate { get; set; }
		public DateTime TreatmentDate { get; set; }
		public string? ServiceName { get; set; }
		public decimal? Discount { get; set; }
		public decimal AdvancePaid { get; set; }
		public decimal? PaymentPaid { get; set; }
		public decimal? CostOfTreatment { get; set; }
		public decimal? Total { get; set; }
		public Guid CardCheckNumber { get; set; }
		public string? ImageUrl { get; set; }
	}


	public enum Department
    {
        Neuro,
        Ortho,
        General
    }
    public enum PaymentType
    {
        Cash,
        Trandfer,
        POS
    }

}
