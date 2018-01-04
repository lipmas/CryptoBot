using System;
using System.Collections.Generic;

using CryptoBot.Constants;
using CryptoBot.ExchangeApi;

namespace CryptoBot.TechnicalTradingStrategy  {
    /*
     * Very simplistic approximation of support/resistance levels based on order book snapshot
     * uses some parameterized heuristics to determine which price if any may be a support/resistance level
     * this is very simple strategy that doesnt take into account fast changing order books and is unlikely to be profitable
    */
    public class OrderBookSupportResistance : ITechnicalTradingStrategy {
        public decimal? findSupportLevel(Exchange exch, string marketName) {
            exch.updateMarketOrderBook(marketName, TradingParameters.orderBookDepth);
            //reverse the list because bids are presorted by largest->smallest
            var bids = exch.getMarket(marketName).bids;
            List<Quote> reverseBids = new List<Quote>(bids);
            reverseBids.Reverse();
            var consolidatedQuotes = consolidateOrderBook(reverseBids, TradingParameters.aggSize);

            //find the largest qty bucket in the order book
            int maxIndex = 0;
            decimal maxQty = consolidatedQuotes[0].qty;
            for(int i=0; i<consolidatedQuotes.Count; ++i){
                if(consolidatedQuotes[i].qty > maxQty){
                    //but only if this is far enough away from the current market price to be worth targeting
                    if(i >= TradingParameters.aggSize - TradingParameters.awayFromMarketThreshold){
                        continue;
                    }
                    maxQty = consolidatedQuotes[i].qty;
                    maxIndex = i;
                }
            }

            decimal sum = 0;
            foreach(var bucket in consolidatedQuotes) {
                sum += bucket.qty;
            }
            var averageQty = sum / consolidatedQuotes.Count;
            //check if this qty is significantly higher in this bucket than average 
            if( (consolidatedQuotes[maxIndex].qty - averageQty) / averageQty < TradingParameters.levelThreshold) {
                return null;
            }
            //consolidatedQuotes[maxIndex].printDetails();
            return consolidatedQuotes[maxIndex].price;;
        }

        public decimal? findResistanceLevel(Exchange exch, string marketName) {
            exch.updateMarketOrderBook(marketName, TradingParameters.orderBookDepth);
            var asks = exch.getMarket(marketName).asks;
            var consolidatedQuotes = consolidateOrderBook(asks, TradingParameters.aggSize);

            //find the largest qty bucket in the aggregated order book
            int maxIndex = 0;
            decimal maxQty = consolidatedQuotes[0].qty;
            for(int i=0; i<consolidatedQuotes.Count; ++i){
                if(consolidatedQuotes[i].qty > maxQty) {
                    //but only if this is far enough away from the current market price to be worth targeting
                    if(i < TradingParameters.awayFromMarketThreshold) {                  
                        continue;
                    }
                    maxQty = consolidatedQuotes[i].qty;
                    maxIndex = i;
                }
            }

            decimal sum = 0;
            foreach(var bucket in consolidatedQuotes) {
                sum += bucket.qty;
            }
            var averageQty = sum / consolidatedQuotes.Count;
            //check if this qty is significantly higher in this bucket than average 
            if( (consolidatedQuotes[maxIndex].qty - averageQty) / averageQty < TradingParameters.levelThreshold) {
                return null;
            }
            //consolidatedQuotes[maxIndex].printDetails();
            return consolidatedQuotes[maxIndex].price;;
        }

        /*
         * Aggregates the order book into a smaller set of price levels 
         * aggSize: the number of buckets to divide the order book range by during aggregation
         * assumes the quotes are sorted smallest->largest
         */
        private static List<Quote> consolidateOrderBook(List<Quote> quotes, decimal aggSize) {
            decimal spread = quotes[quotes.Count-1].price - quotes[0].price;
            decimal bucketSize = spread / aggSize;
            decimal currBucket = quotes[0].price + bucketSize;

            decimal sum = 0;
            var consolidatedQuotes = new List<Quote>();
            for(int i=0; i<quotes.Count; i++) {
                if(quotes[i].price > currBucket) {
                    consolidatedQuotes.Add(new Quote(currBucket, sum));
                    sum = 0;
                    while(quotes[i].price > currBucket) {
                        currBucket += bucketSize;
                    }
                    continue;
                }
                sum += quotes[i].qty;
                //if this is last quote add it to the last consolidated quote bucket
                if(i == quotes.Count-1) {
                    consolidatedQuotes.Add(new Quote(currBucket, sum));
                }
            }
            return consolidatedQuotes;
        }
    }
}