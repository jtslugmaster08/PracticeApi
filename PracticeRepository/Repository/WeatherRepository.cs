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
    public class WeatherRepository:GenericRepository<DataContext,Weather>,IWeatherRepository
    {
        public WeatherRepository(DataContext context) : base(context)
        {

        }
    }
}
