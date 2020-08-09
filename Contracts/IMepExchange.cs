using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ricardo.Contracts
{
    public interface IMepExchange
    {
        Task<decimal> RetrieveRateAsync(DateTime RateAtDate);
    }

}
        

