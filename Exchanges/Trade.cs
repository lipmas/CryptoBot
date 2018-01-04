using System;
using System.Collections.Generic;

namespace CryptoBot.ExchangeApi {

    public class TradeHistory {
        public int startTime;
        public int endTime;
        public List<Trade> historicalTrades;

        public TradeHistory() {
            startTime = 0;
            endTime = 0;
            historicalTrades = new List<Trade>();
        }
    }
    public class Trade {
        public decimal qty;
        public decimal price;
        public int timeStamp;
        public Trade(decimal _qty, decimal _price, int _timeStamp) {
            qty = _qty;
            price = _price;
            timeStamp = _timeStamp;
        }
        public void printDetails() {
            Console.WriteLine("{qty: " + qty + ", price: " + price + ", timeStamp: " + timeStamp + "}");
        }
    }
}