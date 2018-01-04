using System;
using System.Collections.Generic;

using CryptoBot.Constants;
using CryptoBot.ExchangeApi;

namespace CryptoBot.TechnicalTradingStrategy {
    /*
     * Fetches historical trade data over some time period
     * uses the high-low prices in this range and the technical analysis indicator of fibonacci retracement levels
     * to determine a basis for support/resistance lines
     */
    public class FibonacciSR : ITechnicalTradingStrategy {
        public void findSupportResistanceLevels(Exchange exch, string marketName, out decimal? support, out decimal? resistance) {
            exch.updateTradeHistory(marketName, HistoricalSRParams.lookBackPeriod);
            var trades = exch.getHistoricalTrades(marketName);
            fibonacciSR(trades, out support, out resistance);
        }
        private void fibonacciSR(List<Trade> trades, out decimal? support, out decimal? resistance) {
            decimal currPrice = trades[trades.Count-1].price;
            decimal max = trades[0].price;
            decimal min = trades[0].price;

            for(var i=0; i<trades.Count; ++i) {
                if(trades[i].price > max) {
                    max = trades[i].price;
                }
                if(trades[i].price < min) {
                    min = trades[i].price;
                }
            }

            //list of fibbonacci retracement levels
            decimal[] fibonacciLevels = {.23M, .38M, .61M, 1.00M, 1.61M, 2.61M};

            //determine which fibonacci support/resistance the current price is at
            //scaled to low-high range from historical data
            if(currPrice < fibonacciLevels[0] * max) {
                resistance =  fibonacciLevels[0] * max;
                support = null;
            } else if(currPrice < fibonacciLevels[1] * max) {
                resistance = fibonacciLevels[1] * max;
                support = fibonacciLevels[0]*max;
            }else if(currPrice < fibonacciLevels[2] * max) {
                resistance = fibonacciLevels[2] * max;
                support = fibonacciLevels[1]*max;
            }else if(currPrice < fibonacciLevels[3] * max) {
                resistance = fibonacciLevels[3] * max;
                support = fibonacciLevels[2]*max;
            }else if(currPrice < fibonacciLevels[4] * max) {
                resistance = fibonacciLevels[4] * max;
                support = fibonacciLevels[3]*max;
            }else if(currPrice < fibonacciLevels[5] * max) {
                resistance = fibonacciLevels[5] * max;
                support = fibonacciLevels[4]*max;
            }else{
                resistance = null;
                support = fibonacciLevels[5]*max;
            }
        }
    }
}