//using HMS.Infrastructure.Persistence.DataContext;
//using HMS.Infrastructure.Repositories.IRepository;
//using HMSPortal.Domain.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace HMS.Infrastructure.Repositories.Repository
//{
//    public class InvoicePaymentDetailRepository : Repository<InvoicePaymentDetail>, IInvoicePaymentDetailRepository
//    {
//        private readonly ApplicationDbContext _db;

//        public InvoicePaymentDetailRepository(ApplicationDbContext db) : base(db)
//        {
//            _db = db;
//        }

//        public void Update(InvoicePaymentDetail obj)
//        {
//            _db.InvoicePaymentDetails.Update(obj);
//        }
//    }
//}
