using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobeezMart.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IBrandRepository Brand { get; }
        IConditionRepository Condition { get; }
        IProductRepository Product { get; }
        ICompanyRepository Company { get; }
        IShoppingCartRepository ShoppingCart { get; }
        IApplicationUserRepository ApplicationUser { get; }
		IOrderDetailRepository OrderDetail { get; }
		IOrderHeaderRepository OrderHeader { get; }





		void Save();

    }
}
