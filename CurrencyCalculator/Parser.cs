using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace CurrencyCalculator
{
    public class Parser
    {
        public List<Currency> ParseData(string response)
        {
            List<Currency> currencies = new List<Currency>();

            JArray jsonArray = JArray.Parse(response);
            JObject jsonObject = (JObject)jsonArray[0];
            JArray rates = (JArray)jsonObject["rates"];

            foreach (JObject rate in rates)
            {
                Currency currency = new Currency
                {
                    CurrencyName = (string)rate["currency"],
                    Code = (string)rate["code"],
                    Mid = (string)rate["mid"]
                };
                currencies.Add(currency);
            }

            return currencies;
        }
    }
}
