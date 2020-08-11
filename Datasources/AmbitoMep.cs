using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using adrius.ricardo.Contracts;

namespace adrius.ricardo.Datasources
{
    public class AmbitoMep : IMepExchange
    {
        private const string MepUrl = @"https://mercados.ambito.com//dolarrava/mep/historico-general/{DD}-{MM}-{YYYY}/{DD+1}-{MM+1}-{YYYY+1}";
        public async Task<decimal> RetrieveRateAsync(DateTime RateAtDate)
        {
            var client = new HttpClient();

            var url = MepUrl
                .Replace("{YYYY}", RateAtDate.Year.ToString("D4"))
                .Replace("{MM}", RateAtDate.Month.ToString("D2"))
                .Replace("{DD}", RateAtDate.Day.ToString("D2"))
                .Replace("{YYYY+1}", RateAtDate.AddDays(1).Year.ToString("D4"))
                .Replace("{MM+1}", RateAtDate.AddDays(1).Month.ToString("D2"))
                .Replace("{DD+1}", RateAtDate.AddDays(1).Day.ToString("D2"));

            //var resultString = await client.GetStringAsync(url);

            var result = await client.GetStreamAsync(url);
            var rateData = await JsonSerializer.DeserializeAsync<List<List<string>>>(result);

            var rate  = (rateData.Where( group => group.Contains($"{RateAtDate.Day.ToString("D2")}-{RateAtDate.Month.ToString("D2")}-{RateAtDate.Year.ToString("D4")}")).First().Where( item => !item.Contains("-")).First());
            rate = rate.Replace(",",".");
            Decimal.TryParse(rate, out var rateDecimal);
            
            return rateDecimal;
            
        }
    }

    

    

}
        

