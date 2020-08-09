using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ricardo.Contracts;
using ricardo.Datasources;

namespace ricardo.Controllers
{
    [ApiController]
    public class MervalController : ControllerBase
    {
        

        private readonly ILogger<MervalController> _logger;
        private readonly FileCache _cache;

        public MervalController(ILogger<MervalController> logger, FileCache cache)
        {
            _logger = logger;
            _cache = cache;
            Console.WriteLine($"{this.GetType().FullName} constructor");
            _logger.LogInformation($"{this.GetType().FullName} constructor");
        }

        [HttpGet]
        [Route("[controller]/mep/{fecha}")]
        public async Task<IActionResult> MEP(string Fecha)
        {
            var key = $"MEP/{Fecha.ToString()}";
            var value = _cache.Get(key);

            if (value!=null)
                return Ok((decimal)value);

            try {
                var newValue = await this.getMepRate(Fecha);
                this._cache[key] = newValue;
                return Ok(newValue);
            } catch (Exception) {
                return BadRequest();
            }

        }

        protected async Task<decimal> getMepRate(string Fecha) {
            if (Fecha.Length!=8)
                throw new ArgumentException(Fecha);

            Int16.TryParse( Fecha.Substring(0,4), out var Y );
            Int16.TryParse( Fecha.Substring(4,2), out var M );
            Int16.TryParse( Fecha.Substring(6,2), out var D );

            // Validate args
            var fecha = new DateTime(Y, M, D);
            if (!(fecha.Year==Y && fecha.Month==M && fecha.Day==D))
                throw new ArgumentException(Fecha);

            IMepExchange datasource = new CronistaMep();
            var rate = await datasource.RetrieveRateAsync(fecha);
            return rate;
        }
    }
}
