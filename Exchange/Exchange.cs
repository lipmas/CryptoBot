using System;
using System.Collections.Generic;

namespace CryptoBot.Exchange {

    public class Exchange {
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

        public void printMarkets() {
            foreach(var market in markets) {
                Console.WriteLine(market.Key);
                //markets[market.Key].printDetails();
            }
        }

    }
}