using System;
using System.Collections.Generic;

namespace CryptoBot.ExchangeApi {
    /*
     * Abstract exchange class that tracks many different markets
     * and exposes a common interface for interacting with the exchange data
     */
    public abstract class Exchange {

        protected string exchangeName;
        protected List<String> supportedCoins;
        protected Dictionary<string, Market> markets =  new Dictionary<string, Market>();
        protected List<string> marketNames = new List<string>();
        protected Dictionary<string, TradeHistory> tradeHistories = new Dictionary<string, TradeHistory>();
        protected Dictionary<string, decimal> balances;

        public Exchange(string name) {
            exchangeName = name;
        }
        public List<string> getMarketNames() {
            return marketNames;
        }
        public List<Trade> getHistoricalTrades(string marketName){
            return tradeHistories[marketName].historicalTrades;
        }
        public Market getMarket(string name) {
            return markets[name];
        }
        public void printMarket(string name) {
            var market = markets[name];
            Console.WriteLine("{ name: " + market.name + ", " + "lastPrice: " + market.lastPrice );
        }
        public void printMarkets() {
            foreach(var market in markets) {
                printMarket(market.Key);
            } 
        }
        public void printBalances() {
            foreach(var bal in balances) {
                printMarket(bal.Key + ": " + bal.Value);
            }
        }
        //specific exchanges must implement these data updates through the appropriate API calls
        public abstract void updateMarketPrices();
        public abstract void updateMarketOrderBook(string marketName, int depth);
        public abstract void updateTradeHistory(string marketName, int secondsBack);
        public abstract bool placeBuyLimitOrder(string marketName, decimal rate, decimal amount);
        public abstract bool placeSellLimitOrder(string marketName, decimal rate, decimal amount);
        public abstract bool getBalances();

    }
}