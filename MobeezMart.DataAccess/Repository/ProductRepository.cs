using MobeezMart.DataAccess.Repository.IRepository;
using MobeezMart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobeezMart.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
        public void Update(Product obj)
        {
            var objFromDb = _db.Products.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.DeviceName= objFromDb.DeviceName;
                objFromDb.Description= objFromDb.Description;
                objFromDb.IMEI= objFromDb.IMEI;
                objFromDb.ListPrice= objFromDb.ListPrice;
                objFromDb.Price50= objFromDb.Price50;
                objFromDb.Price100= objFromDb.Price100;
                objFromDb.BrandId= objFromDb.BrandId;
                objFromDb.ConditionId= objFromDb.ConditionId;

                if (obj.ImageUrl != null)
                {
                    objFromDb.ImageUrl= obj.ImageUrl;
                }
            }
        }
    }
}
