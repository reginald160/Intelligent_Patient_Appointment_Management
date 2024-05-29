using HMSPortal.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Infrastructure.Repositories.IRepository
{
    public interface IPaymentRepository : IRepository<Payment>
    {
		void Update(Payment obj);
    }
}
