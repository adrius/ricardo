using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using adrius.ricardo.Contracts;
using adrius.ricardo.Datasources;

namespace adrius.ricardo.Controllers
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
        }

        [HttpGet]
        [Route("[controller]/mep/{Fecha}")]
        public async Task<IActionResult> MEP(string Fecha)
        {
            _logger.LogTrace($"GET /merval/mep/{Fecha}");
            var key = $"MEP/{Fecha.ToString()}";
            var value = _cache.Get(key);

            if (value!=null) {
                _logger.LogInformation($"cache hit. Result: {(decimal)value}");
                return Ok((decimal)value);
            }
                

            try {
                var newValue = await this.getMepRate(Fecha);
                this._cache[key] = newValue;
                _logger.LogInformation($"cache miss. Result: {(decimal)newValue}");
                return Ok(newValue);
            } catch (Exception ex) {
                _logger.LogError($"Error getting rate. Ex: {ex.Message}\n{ex.StackTrace}");
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

            if (fecha > DateTime.Today)
                throw new ArgumentException(Fecha);
                
            IMepExchange datasource = new AmbitoMep();
            var rate = await datasource.RetrieveRateAsync(fecha);
            return rate;
        }
    }
}
