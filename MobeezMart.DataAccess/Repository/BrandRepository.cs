using MobeezMart.DataAccess.Repository.IRepository;
using MobeezMart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobeezMart.DataAccess.Repository
{
    public class BrandRepository : Repository<Brand>, IBrandRepository
    {
        private ApplicationDbContext _db;
        public BrandRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
        public void Update(Brand obj)
        {
            _db.Brands.Update(obj);
        }
    }
}
