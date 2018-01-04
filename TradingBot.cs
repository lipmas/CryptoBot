using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

using CryptoBot.Constants;
using CryptoBot.ExchangeApi;
using CryptoBot.ExchangeApi.Poloniex;
using CryptoBot.TradingStrategy;

namespace CryptoBot {
    public class TradingBot {
        private Exchange exch;
        private Dictionary<string, decimal> supportLevels = new Dictionary<string, decimal>();
        private Dictionary<string, decimal> resistanceLevels = new Dictionary<string, decimal>();
        private Dictionary<string, decimal> balances;

        public TradingBot() {
            exch = new PoloniexExchange(new List<string>(TradingParameters.supportedCoins)); 
        }
        public void start() {
            if(TradingParameters.tradingEnabed) {
                balances = exch.getBalances();
            }

            //initialize prices
            exch.updateMarketPrices();

            //initialize order books and strategy
            foreach(var name in exch.getMarketNames()) {
                updateStrategy(name);
            }

            //setup a polling timer to get market ticks
            Timer market_timer = new Timer(TradingParameters.marketPollRate);
            market_timer.Elapsed += processMarketTick;
            market_timer.Start();

            //setup a polling timer to update order book strategies
            Timer order_book_timer = new Timer(TradingParameters.orderBookPollRate);
            order_book_timer.Elapsed += processUpdateOrderBooks;
            order_book_timer.Start();
        }

        public void processMarketTick(Object source, ElapsedEventArgs e) {
            exch.updateMarketPrices();
            exch.printMarkets();

            //check if we have crossed any support/resistance lines for any markets
            foreach(var name in exch.getMarketNames()) {
                var market = exch.getMarket(name);
                if(supportLevels[name] != -1 && market.lastPrice <= supportLevels[name]){
                    Console.WriteLine("Hit the support level for " + name + " at " + supportLevels[name]);
                    updateStrategy(name);
                }
                if(resistanceLevels[name] != -1 && market.lastPrice >= resistanceLevels[name]){
                    Console.WriteLine("Hit the resistance level for " + name + " at " + resistanceLevels[name]);
                    updateStrategy(name);
                }
            }
        }

        public void processUpdateOrderBooks(Object source, ElapsedEventArgs e) {
            foreach(var name in exch.getMarketNames()) {
                updateStrategy(name);
            }
        }

        public void updateStrategy(string marketName) {
            var market = exch.getMarket(marketName);
            exch.updateMarketOrderBook(marketName, TradingParameters.orderBookDepth);

            var support = TradingStrategy.OrderBookSupportResistance.findSupportLevel(market.bids);
            var resistance = TradingStrategy.OrderBookSupportResistance.findResitanceLevel(market.asks);

            Console.WriteLine("For " + marketName);
            Console.WriteLine("Strategy to buy at support level of: " + support ?? "None");
            Console.WriteLine("Strategy to sell at resistance level of: " + resistance ?? "None");
            supportLevels[marketName] = support ?? -1;
            resistanceLevels[marketName] = resistance ?? -1;

            if(TradingParameters.tradingEnabed) {
               //TODO implement actual trading logic based on balances/risk tolerances/fees etc.
            }
        }
    }
}