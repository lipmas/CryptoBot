using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

using CryptoBot.Constants;
using CryptoBot.ExchangeApi;

namespace CryptoBot {
    public class TradingBot {

        private Exchange exch;

        public TradingBot() {
            exch = new PoloniexExchange(new List<string>(TradingSettings.supportedCoins)); 
        }
        public void start() {
            Timer timer = new Timer(TradingSettings.pollRate);
            timer.Elapsed += processMarketTick;
            timer.Start();
        }

        public void processMarketTick(Object source, ElapsedEventArgs e) {
            exch.updateMarketPrices();
            exch.printMarkets();
        }
    }
}