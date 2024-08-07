using MobeezMart.DataAccess.Repository.IRepository;
using MobeezMart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobeezMart.DataAccess.Repository
{
    public class ConditionRepository : Repository<Condition>, IConditionRepository
    {
        private ApplicationDbContext _db;
        public ConditionRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
        public void Update(Condition obj)
        {
            _db.Conditions.Update(obj);
        }
    }
}
