using PracticeModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticeModel.Interface.Services
{
    public interface IErrorService
    {
        void SaveError(Error error);
        void CreateError(Exception exception);
    }
}
