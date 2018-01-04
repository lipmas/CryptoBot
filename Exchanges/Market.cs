using System;
using System.Collections.Generic;

namespace CryptoBot.ExchangeApi {
    /*
        Market represents a pair of currencies that form a marketplace
        baseCurrency- The currency the price is quoted in
        marketCurrency- The currency being bought/sold for the baseCurrency
        lastPrice- last known price
        buys/sells- represent full order book at last snapshot
     */
    public class Market {
        public  string name {get; set;}
        public string baseCurrency {get; set;}
        public string marketCurrency {get; set;}
        public decimal lastPrice;

        //order book of buys and sells
        private List<Quote> buys;
        private List<Quote> sells;

         public Market(string _name, string _base, string _market) {
            name =_name;
            baseCurrency = _base;
            marketCurrency = _market;
        }

        public void updateLastPrice(decimal price) {
            lastPrice = price;
        }
        public void updateOrderBook(List<Quote> new_buys, List<Quote> new_sells) {
            buys = new_buys;
            sells = new_sells;
        }
    }
    
    public class Quote {
        public decimal price {get; set;}
        public decimal qty {get; set;}
        public void printDetails() {
            Console.WriteLine("{ Price: " + price + "," + " Qty: "   + qty + "}");
        }
    }
}