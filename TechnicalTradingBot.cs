using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

using CryptoBot.Constants;
using CryptoBot.ExchangeApi;
using CryptoBot.ExchangeApi.Poloniex;
using CryptoBot.TechnicalTradingStrategy;

namespace CryptoBot {
    /*
     * Technical trading bot that uses a technical trading strategy to find support/resistance levels
     * for a particular currency pair market and then exeutes trades based on those levels
     */
    public class TechnicalTradingBot {
        private Exchange exch;
        private Dictionary<string, decimal> supportLevels = new Dictionary<string, decimal>();
        private Dictionary<string, decimal> resistanceLevels = new Dictionary<string, decimal>();
        private ITechnicalTradingStrategy strategy;

        public TechnicalTradingBot(Exchange theExch, ITechnicalTradingStrategy theStrategy) {
            exch = theExch;
            strategy = theStrategy;
        }

        public void start() {
            if(TradingParameters.tradingEnabled){
                exch.getBalances();
                exch.printBalances();
            }

            //initialize prices
            exch.updateMarketPrices();

            //for now use OrderBook strategy
            var strategy = new OrderBookSupportResistance();

            //initialize strategy
            foreach(var name in exch.getMarketNames()) {
                executeStrategy(strategy, name);
            }

            //setup a polling timer to get market ticks
            Timer market_tick_timer = new Timer(TradingParameters.marketPollRate);
            market_tick_timer.Elapsed += processMarketTick;
            market_tick_timer.Start();

            /* 
            //setup a polling timer to update order book strategies
            Timer order_book_timer = new Timer(TradingParameters.orderBookPollRate);
            order_book_timer.Elapsed += processUpdateOrderBooks;
            order_book_timer.Start();
            */
        }

        //event handler function that gets called by the market_tick_timer
        public void processMarketTick(Object source, ElapsedEventArgs e) {
            exch.updateMarketPrices();
            Console.WriteLine("-----Market Update------");
            exch.printMarkets();
            Console.WriteLine("------------------------");  

            //check if we have crossed any support/resistance lines for any markets
            foreach(var name in exch.getMarketNames()) {
                var market = exch.getMarket(name);
                if(supportLevels[name] != -1 && market.lastPrice <= supportLevels[name]){
                    Console.WriteLine("Hit the support level for " + name + " at " + supportLevels[name]);
                    executeTrade(name, supportLevels[name], true);
                    //get next trade
                    executeStrategy(strategy, name);
                }
                if(resistanceLevels[name] != -1 && market.lastPrice >= resistanceLevels[name]){
                    Console.WriteLine("Hit the resistance level for " + name + " at " + resistanceLevels[name]);
                    executeTrade(name, resistanceLevels[name], false);
                    //get next trade
                    executeStrategy(strategy, name);
                }
            }
        }

        /* 
        public void processUpdateOrderBooks(Object source, ElapsedEventArgs e) {
            foreach(var name in exch.getMarketNames()) {
                updateStrategy(name);
            }
        }
        */

        /*
         * Uses the technical strategy to predict where the next support/resistance levels will be
         * and primes the bot to make trades when these levels are reached
         */
        public void executeStrategy(ITechnicalTradingStrategy strategy, string marketName) {
            var support    = strategy.findSupportLevel(exch, marketName);
            var resistance = strategy.findResistanceLevel(exch, marketName);

            Console.WriteLine("For " + marketName);
            Console.WriteLine("Strategy to buy at support level of: " + support);
            Console.WriteLine("Strategy to sell at resistance level of: " + resistance);

            //set the support/resistance levels
            supportLevels[marketName] = support ?? -1;
            resistanceLevels[marketName] = resistance ?? -1;
        }

        public void executeTrade(string marketName, decimal price, bool isSupport) {
            //TODO: use the exch api to place orders/execute a trade
            //how many orders to generate at what prices exactly can be determined
            //by avaiable balances on the exchange, risk tolerance, fees, and other parameters
        }
    }
}