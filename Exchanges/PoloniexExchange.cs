using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace CryptoBot.ExchangeApi {

    /*
    PoloniexExchange wraps the PoloniexApi to build a generic exchange class
     */
    public class PoloniexExchange : Exchange {

        private PoloniexApi api;

        public PoloniexExchange(List<string> _supportedCoins) {
            supportedCoins = _supportedCoins;
            api = new PoloniexApi();
            getSupportedMarkets();
        }

        public void test() {
            printMarkets();
        }

        public override void updateMarketPrices() {
            var res = api.getTickers();
            foreach(var ticker in res) {
                //if this is a supported market
                if(markets.ContainsKey(ticker.Key)) {
                    var lastPrice =  ticker.Value["last"];
                    var highestBid = ticker.Value["highestBid"];
                    var lowestAsk =  ticker.Value["lowestAsk"];
                    markets[ticker.Key].updateLastPrice((decimal) lastPrice);
                }
            }
        }
        public void updateMarketOrderBook(string market) {

        }

        public void getSupportedMarkets() {
            var res = api.getTickers();
            foreach(var ticker in res) {
                string name = ticker.Key; //name of market in form baseTicker_marketTicker
                string baseName  = name.Split("_")[0];
                string marketName = name.Split("_")[1];
                //only care about the market if we suppport both coins
                if(supportedCoins.Contains(baseName) && supportedCoins.Contains(marketName)){
                    markets.Add(name, new Market(name, baseName, marketName));
                }
                //Console.WriteLine(ticker);
            }
        }
    }
}