using BackgroundServiceSample.WebApi.Model;
using Microsoft.AspNetCore.Mvc;

namespace BackgroundServiceSample.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RateController(CurrencyRepository repository) : ControllerBase
    {
        private readonly CurrencyRepository _repository = repository;

        [HttpGet]
        public IActionResult Get() => Ok(_repository.Currencies);


        [HttpPut]
        public IActionResult Put(Currency model)
        {
            Currency? currency = _repository.Update(model);
            return currency is not null ? Ok(currency) : NotFound("Current Not Found");
        }
    }
}
