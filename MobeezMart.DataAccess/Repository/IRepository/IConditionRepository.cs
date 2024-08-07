﻿using MobeezMart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobeezMart.DataAccess.Repository.IRepository
{
    public interface IConditionRepository : IRepository<Condition>
    {
        void Update(Condition obj);
    }
}
