//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace HMSPortal.Domain.Models
//{
//    public class InvoicePaymentDetail : BaseIndividual
//    {
//        public Guid PatientId { get; set; }
//        [ForeignKey("PatientId")]
//        public Patient? Patient { get; set; }

//        [Required]
//        public Guid DoctorId { get; set; }
//        [ForeignKey("DoctorId")]
//        public Doctor? Doctor { get; set; }

//        public Guid PaymentInvoiceId { get; set; }
//        [ForeignKey("PaymentInvoiceId")]
//        public PaymentInvoice? PaymentInvoice { get; set; }
//    }
//}
