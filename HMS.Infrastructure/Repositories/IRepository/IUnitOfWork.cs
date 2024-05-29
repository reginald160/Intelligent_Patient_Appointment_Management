using HMS.Infrastructure.Persistence.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Infrastructure.Repositories.IRepository
{
    public interface IUnitOfWork
    {
        IPatientRepository Patient { get;  }
        IDoctorRepository Doctor { get;  }
        IPaymentRepository Payment { get; }
        IPaymentInvoiceRepository PaymentInvoice { get; }
        //IInvoicePaymentDetailRepository InvoicePaymentDetail { get; }
        void Save();
    }
}
