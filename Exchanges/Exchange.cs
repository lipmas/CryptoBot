using System;
using System.Collections.Generic;

namespace CryptoBot.ExchangeApi {

    /*
     * Abstract exchange class that tracks many different markets
     * and exposes a common interface for interacting with the exchange data
     */
    public abstract class Exchange {
        protected List<String> supportedCoins;
        protected Dictionary<string, Market> markets =  new Dictionary<string, Market>();
        protected List<string> marketNames = new List<string>();

        public Exchange() {}
        public List<string> getMarketNames() {
            return marketNames;
        }
        public Market getMarket(string name) {
            return markets[name];
        }
        public void printMarket(string name) {
            var market = markets[name];
            Console.WriteLine("{ name: " + market.name + ", " + "lastPrice: " + market.lastPrice );
        }
        public void printMarkets() {
            Console.WriteLine("-----Market Update------");
            foreach(var market in markets) {
                printMarket(market.Key);
            }
            Console.WriteLine("------------------------");
            
        }
        //exchanges must implement these updates through the appropriate API calls
        public abstract void updateMarketPrices();
        public abstract void updateMarketOrderBook(string marketName, int depth);
        public abstract bool placeBuyLimitOrder(string marketName, decimal rate, decimal amount);
        public abstract bool placeSellLimitOrder(string marketName, decimal rate, decimal amount);
        public abstract Dictionary<string, decimal> getBalances();

    }
}