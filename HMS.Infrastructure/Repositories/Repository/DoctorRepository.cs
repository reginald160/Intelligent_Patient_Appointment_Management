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
   public class DoctoreRepository : Repository<Doctor>, IDoctorRepository
    {
        private readonly ApplicationDbContext _db;

        public DoctoreRepository(ApplicationDbContext db) : base (db)
        {
            _db = db;
        }

        public void Update(Doctor obj)
        {
            _db.Doctors.Update(obj);
        }
    }
}
