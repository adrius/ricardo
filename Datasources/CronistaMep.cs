using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ricardo.Contracts;

namespace ricardo.Datasources
{
    public class CronistaMep : IMepExchange
    {
        private const string MepUrl = @"https://www.cronista.com/MercadosOnline/json/getDinamicos.html?tipo=monedas&id=ARSMEP0&fechaDesde={DD}/{MM}/{YYYY}&fechaHasta={DD}/{MM}/{YYYY}";
        public async Task<decimal> RetrieveRateAsync(DateTime RateAtDate)
        {
            var client = new HttpClient();

            var url = MepUrl
                .Replace("{YYYY}", RateAtDate.Year.ToString("D4"))
                .Replace("{MM}", RateAtDate.Month.ToString("D2"))
                .Replace("{DD}", RateAtDate.Day.ToString("D2"));

            //var resultString = await client.GetStringAsync(url);

            var result = await client.GetStreamAsync(url);
            var rateData = await JsonSerializer.DeserializeAsync<CronistaMepDto>(result);

            if (rateData.historico.Count==0)
                return await this.RetrieveRateAsync(RateAtDate.AddDays(-1));

            return ((rateData.historico[0].Compra + rateData.historico[0].Venta) / 2 );
        }
    }

    internal class CronistaMepDto {
        public CronistaMepMonedasDto monedas { get; set; }
        public IList<CronistaMepHistoricoDto> historico { get; set; }
    }

    internal class CronistaMepMonedasDto {
        public decimal VariacionPorcentual { get; set; }
        public decimal VariacionNeta { get; set; }
        public decimal Compra { get; set; }
        public decimal Venta { get; set; }
        public decimal Ultimo { get; set; }
        public decimal ValorCierreAnterior { get; set; }
        public string Icono { get; set; }
        public string Pais { get; set; }
        public string Tipo { get; set; }
        public string Ric { get; set; }
        public string UltimaActualizacion { get; set; }
        public string Nombre { get; set; }
        public string UrlId { get; set; }
        public decimal Id { get; set; }
    }

    internal class CronistaMepHistoricoDto {
        public decimal Compra { get; set; }
        public decimal Venta { get; set; }
        public string Ric { get; set; }
        public string Fecha { get; set; }
        public decimal Id { get; set; }
    }

}
        

