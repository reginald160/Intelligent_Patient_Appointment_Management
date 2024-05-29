using HMS.Infrastructure.Persistence.DataContext;
using HMS.Infrastructure.Repositories.IRepository;
using HMSPortal.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Infrastructure.Repositories.Repository
{
    public class PaymentInvoiceRepository : Repository<PaymentInvoice>, IPaymentInvoiceRepository
    {
        private readonly ApplicationDbContext _db;

        public PaymentInvoiceRepository(ApplicationDbContext db) : base (db)
        {
            _db = db;
        }

        public void Update(PaymentInvoice obj)
        {
            _db.PaymentInvoices.Update(obj);
        }
    }
}
