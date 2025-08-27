using PracticeModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticeModel.Interface.Services
{
    public interface IWeatherService
    {
        public List<Weather> GetMyWeather();
    }
}
