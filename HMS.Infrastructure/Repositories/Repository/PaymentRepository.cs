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
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        private readonly ApplicationDbContext _db;

        public PaymentRepository(ApplicationDbContext db) : base (db)
        {
            _db = db;
        }

        public void Update(Payment obj)
        {
            _db.Payments.Update(obj);
        }
    }
}
