using System;
using System.Collections.Generic;

using CryptoBot.Bots;
using CryptoBot.ExchangeApi;
using CryptoBot.ExchangeApi.Poloniex;
using CryptoBot.TechnicalTradingStrategy;
using CryptoBot.Constants;

class Program
{
    public static void Main(string[] args) {

        var poloniexExchange = new PoloniexExchange(new List<string>(TradingParameters.supportedCoins));

        /*
        var orderBookStrategy = new OrderBookSR();
        var bot1 = new TechnicalTradingBot(poloniexExchange, orderBookStrategy);
        bot1.start();
        */

        var fibonaciiStrategy = new FibonacciSR();
        var bot2 = new TechnicalTradingBot(poloniexExchange, fibonaciiStrategy);
        bot2.start();

        // can extend this by creating new types of bots
        // that can run on other exchanges
        // or are using other types of trading strategies...

        //wait for user input to terminate bot
        Console.ReadKey();        
    }
}