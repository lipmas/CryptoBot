using System;
using System.Collections.Generic;

using CryptoBot;
using CryptoBot.ExchangeApi;
using CryptoBot.ExchangeApi.Poloniex;
using CryptoBot.TechnicalTradingStrategy;
using CryptoBot.Constants;

class Program
{
    public static void Main(string[] args) {
        var exchange = new PoloniexExchange(new List<string>(TradingParameters.supportedCoins));
        var strategy = new OrderBookSupportResistance();
        var bot = new TechnicalTradingBot(exchange, strategy);
        bot.start();
        Console.ReadKey();

        //can run other bots on other exchanges or using other strategies...
    }

    public static void testApi() {
        PoloniexApi api = new PoloniexApi();
        api.test();
    }
}