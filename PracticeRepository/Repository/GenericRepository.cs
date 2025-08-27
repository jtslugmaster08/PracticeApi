using Microsoft.EntityFrameworkCore;
using PracticeModel.Interface.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticeRepository.Repository
{
    public class GenericRepository<TContext,T>:IGenericRepository<T> where T: class where TContext:DbContext
    {
        protected readonly TContext _context;
        public GenericRepository(TContext context)
        {
            _context = context;
        }
    }
}
