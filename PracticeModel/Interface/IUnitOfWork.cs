using PracticeModel.Interface.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticeModel.Interface
{
    public interface IUnitOfWork
    {
        IErrorRepository ErrorRepository { get; }
        IWeatherRepository WeatherRepository { get; }

        Task<bool> CommitAsync();
        bool Commit();
        void Rollback();
        bool HasChanges();
    }
}
