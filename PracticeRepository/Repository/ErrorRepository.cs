using PracticeModel.Entities;
using PracticeModel.Interface.Repositories;
using PracticeRepository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticeRepository.Repository
{
    public class ErrorRepository:GenericRepository<DataContext,Error>,IErrorRepository
    {
        public ErrorRepository(DataContext context):base(context) { }
    }
}
