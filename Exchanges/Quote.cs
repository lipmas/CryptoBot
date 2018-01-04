 using System;
using System.Collections.Generic;

namespace CryptoBot.ExchangeApi {  
    /*
     * A single order book quote
     */
    public class Quote {
        public decimal price {get; set;}
        public decimal qty {get; set;}

        public Quote(decimal _price, decimal _qty) {
            price = _price;
            qty  = _qty;
        }

        public void printDetails() {
            Console.WriteLine("{ Price: " + price + "," + " Qty: "   + qty + "}");
        }
    }
}