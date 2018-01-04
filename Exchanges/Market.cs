using System;
using System.Collections.Generic;

namespace CryptoBot.ExchangeApi {
    /*
     *  Market represents a pair of currencies that form a marketplace
     *  baseCurrency: The currency the price is quoted in
     *  marketCurrency: The currency being bought/sold for the baseCurrency
     *  lastPrice: last known price
     *  bids/asks: full order book at the last snapshot
     */
    public class Market {
        public  string name {get; set;}
        public string baseCurrency {get; set;}
        public string marketCurrency {get; set;}
        public decimal lastPrice {get; set;}
        public List<Quote> bids;
        public List<Quote> asks;
         public Market(string _name, string _base, string _market) {
            name =_name;
            baseCurrency = _base;
            marketCurrency = _market;
         }
    }
}