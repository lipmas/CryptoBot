using System;
using System.Collections.Generic;
using CryptoBot.Exchange;
using CryptoBot.Constants;

using Newtonsoft.Json.Linq;

class Program
{
    public static void Main(string[] args)
    {
        var supportedTickers = new List<string>() {"BTC", "ETH", "LTC"};
        testApi();
    }

    public static void testApi() {
         PoloniexApi api = new PoloniexApi();
        //JObject bal = api.getBalances();
        api.test();
    }
}