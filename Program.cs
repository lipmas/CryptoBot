using System;

using CryptoBot;
using CryptoBot.ExchangeApi;
using CryptoBot.ExchangeApi.Poloniex;
using CryptoBot.Constants;

class Program
{
    public static void Main(string[] args) {
        //testApi();
        var bot = new TradingBot();
        bot.start();
        Console.ReadKey();
    }

    public static void testApi() {
        PoloniexApi api = new PoloniexApi();
        api.test();
    }
}