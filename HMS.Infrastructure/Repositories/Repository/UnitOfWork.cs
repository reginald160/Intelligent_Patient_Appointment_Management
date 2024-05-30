using HMS.Infrastructure.Persistence.DataContext;
using HMS.Infrastructure.Repositories.IRepository;
using HMSPortal.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace HMS.Infrastructure.Repositories.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public IPatientRepository Patient { get; private set; }

		public IDoctorRepository Doctor { get; private set; }
		public IPaymentRepository Payment { get; private set; }
		public IPaymentInvoiceRepository PaymentInvoice { get; private set; }
		//public IInvoicePaymentDetailRepository InvoicePaymentDetail { get; private set; }

		public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Patient = new PatientRepository(_db);
			Doctor = new DoctoreRepository(_db);
			Payment = new PaymentRepository(_db);
			PaymentInvoice = new PaymentInvoiceRepository(_db);
            //InvoicePaymentDetail = new InvoicePaymentDetailRepository(_db);
		}

		public void Save()
        {
            if (_db.SaveChanges() > 0)
            {
				_db.SaveChanges();
			}

		}
    }
}
