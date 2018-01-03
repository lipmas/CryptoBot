using System;

using System.Net.Http;
using System.Collections.Generic;
using CryptoBot.Exchange;


class Program
{
    public static void Main(string[] args)
    {
        var supportedTickers = new List<string>() {"BTC", "ETH", "LTC"};

        PoloniexApi api = new PoloniexApi();
        api.test();
    }
}
