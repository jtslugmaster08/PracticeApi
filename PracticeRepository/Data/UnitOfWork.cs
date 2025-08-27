using PracticeModel.Interface;
using PracticeModel.Interface.Repositories;
using PracticeRepository.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticeRepository.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        public UnitOfWork(DataContext context)
        {
            _context = context;
        }

        public IWeatherRepository WeatherRepository=>new WeatherRepository(_context);
        public IErrorRepository ErrorRepository=>new ErrorRepository(_context);
        public async Task<bool> CommitAsync()
        {
            if (_context.ChangeTracker.HasChanges())
            {
                return await _context.SaveChangesAsync() > 0;
            }
            return true;
        }
        public bool Commit()
        {
            if (_context.ChangeTracker.HasChanges())
            {
                return _context.SaveChanges() > 0;
            }
            return true;
        }
        public bool HasChanges()
        {
            if(_context.ChangeTracker.HasChanges())
            {
                return true;
            }
            return false;
        }

        public void Rollback()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
