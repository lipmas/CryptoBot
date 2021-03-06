using System;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

using CryptoBot.Utility;

namespace CryptoBot.ExchangeApi.Poloniex {
    /*
     * Wrapper class of the PoloniexApi to implement the interface of a generic exchange class
     */
    public class PoloniexExchange : Exchange {

        private PoloniexApi api;

        public PoloniexExchange(List<string> _supportedCoins) : base("Poloniex") {
            supportedCoins = _supportedCoins;
            api = new PoloniexApi();
            getSupportedMarkets();
        }

        public override void updateTradeHistory(string marketName, int secondsBack) {
            var currentTime = DateTime.UtcNow;
            var end = Util.getUnixTimestamp(currentTime);
            var start = end - secondsBack;

            var res = api.getTradeHistory(marketName, start, end);
            var trades = new List<Trade>();
            foreach(var tkn in res){
                var parsedTimestamp = DateTime.ParseExact((string)tkn["date"], "yyyy-MM-dd HH:mm:ss", null);
                var unixTimeStamp = Util.getUnixTimestamp(parsedTimestamp);
                var trade = new Trade((decimal)tkn["amount"], (decimal)tkn["rate"], unixTimeStamp);
                //trade.printDetails();
                trades.Add(trade);
            }
            var hist = new TradeHistory();
            hist.historicalTrades = trades;
            hist.startTime = start;
            hist.endTime = end;
            tradeHistories[marketName] = hist;
        }

        public override void updateMarketPrices() {
            JObject res = api.getTickers();
            foreach(var kv in res) {
                //if this is a supported market
                if(markets.ContainsKey(kv.Key)) {
                    var lastPrice =  kv.Value["last"];
                    var highestBid = kv.Value["highestBid"];
                    var lowestAsk =  kv.Value["lowestAsk"];
                    markets[kv.Key].lastPrice = (decimal)lastPrice;
                }
            }
        }
        public override void updateMarketOrderBook(string marketName, int depth) {
            JObject res = api.getOrderBook(marketName, depth);
            var market = getMarket(marketName);

            foreach(var kv in res) {
                if(kv.Key == "bids") {
                    var quotes = new List<Quote>();
                    foreach(var buy in kv.Value) {
                        quotes.Add(new Quote((decimal)buy[0], (decimal)buy[1]));
                    }
                    market.bids = quotes;
                }
                 if(kv.Key == "asks") {
                    var quotes = new List<Quote>();
                    foreach(var buy in kv.Value) {
                        quotes.Add(new Quote((decimal)buy[0], (decimal)buy[1]));
                    }
                    market.asks = quotes;
                }
            }
        }
        public void getSupportedMarkets() {
            var res = api.getTickers();
            foreach(var kv in res) {
                string name = kv.Key; //name of market in form baseTicker_marketTicker
                string baseName  = name.Split("_")[0];
                string marketName = name.Split("_")[1];
                //only care about the market if we suppport both coins
                if(supportedCoins.Contains(baseName) && supportedCoins.Contains(marketName)){
                    markets.Add(name, new Market(name, baseName, marketName));
                    marketNames.Add(name);
                }
            }
        }

        public override bool placeBuyLimitOrder(string marketName, decimal rate, decimal amount) {
            return api.placeBuyOrder(marketName, rate, amount, false, false);
        }

        public override bool placeSellLimitOrder(string marketName, decimal rate, decimal amount) {
            return api.placeSellOrder(marketName, rate, amount, false, false);
        }
        public override bool getBalances() {
            try {
                JObject res = api.getBalances();
                foreach(var kv in res) {
                    //if this is a supported coin
                    if(supportedCoins.Contains(kv.Key)) {
                        balances.Add(kv.Key, (decimal)kv.Value);
                        //Console.WriteLine(kv.Key + " balance is: " + kv.Value);
                    }
                }
            return true;
            } catch(PoloniexApiException e){
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}
