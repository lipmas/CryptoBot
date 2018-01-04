using System;
using System.Collections.Generic;

namespace CryptoBot.ExchangeApi {

    public abstract class Exchange {
        protected List<String> supportedCoins;
        protected Dictionary<string, Market> markets;
        protected List<string> marketNames;

        //accessors
        public List<string> getMarketNames() {
            return marketNames;
        }

        public Market getMarket(string name) {
            return markets[name];
        }

        public Exchange() {
            markets = new Dictionary<string, Market>();
        }

        public void printMarket(string name) {
            var market = markets[name];
            Console.WriteLine("---------------");
            Console.WriteLine("name: " + market.name);
            Console.WriteLine("lastPrice: " + market.lastPrice);
            Console.WriteLine("---------------");
            
        }
        public void printMarkets() {
            foreach(var market in markets) {
                printMarket(market.Key);
            }
        }

        public abstract void updateMarketPrices();

    }
}