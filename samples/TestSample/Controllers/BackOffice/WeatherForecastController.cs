using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TestSample.Models;

namespace TestSample.Controllers.BackOffice
{
    [ApiVersionV2]
    public class WeatherForecastController : BackOfficeController
    {
        [HttpPost]
        public Task Create([FromBody]WeatherForecast dto)
        {
            return Task.CompletedTask;
        }
        
        [HttpPut("{date:datetime:required}")]
        public Task Edit(DateTime date, [FromBody]WeatherForecast dto)
        {
            return Task.CompletedTask;
        }
    }
}