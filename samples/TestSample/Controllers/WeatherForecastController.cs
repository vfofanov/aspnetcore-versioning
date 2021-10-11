using System;
using System.Collections.Generic;
using System.Linq;
using AspNetCore.Versioning;
using Microsoft.AspNetCore.Mvc;
using TestSample.Models;

namespace TestSample.Controllers
{
    
    public class WeatherForecastController : ApiController
    {
        private static readonly string[] Summaries = {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        /// <summary>
        /// Get comment V1
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiVersionV1]
        public IEnumerable<WeatherForecast> Get()
        {
            var apiVersion = this.GetApiVersion();
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)],
                ApiVersion = apiVersion?.ToString()
            })
            .ToArray();
        }

        /// <summary>
        /// Foo comment V2
        /// </summary>
        /// <param name="count">Count of items</param>
        /// <returns></returns>
        [ApiVersionV2]
        [HttpGet("FooV2/{count:int}")]
        public IEnumerable<WeatherForecast> GetV2(int count = 20)
        {
            var apiVersion = this.GetApiVersion();
            var rng = new Random();
            return Enumerable.Range(1, count).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = 100,
                    Summary = Summaries[rng.Next(Summaries.Length)],
                    ApiVersion = apiVersion?.ToString()
                })
                .ToArray();
        }

    }
}
