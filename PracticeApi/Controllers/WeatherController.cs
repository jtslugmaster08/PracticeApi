using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PracticeModel.Dto;
using PracticeModel.Entities;
using PracticeModel.Interface.Services;

namespace PracticeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IErrorService _errorservice;
        private readonly IWeatherService _weatherService;

        public WeatherController(IMapper mapper, IErrorService errorservice, IWeatherService weatherService)
        {
            _mapper = mapper;
            _errorservice = errorservice;
            _weatherService = weatherService;
        }

        [HttpGet]
        [Route("GetMyWeather")]
        public IActionResult GetMyWeather()
        {
            ResponseDto<List<Weather>> response = new ResponseDto<List<Weather>>();
            try
            {
                response.Data = _weatherService.GetMyWeather();
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.Errors.Add(new ErrorDto { ErrorCode = ex.Message });
            }
            return Ok(response);
        }
    }
}
