using System;

using CryptoBot.ExchangeApi;

namespace CryptoBot.TechnicalTradingStrategy {
    /*
     * interface for all technical trading strategies using support/resistance
     */
    public interface ITechnicalTradingStrategy {
        //strategy to find the most promising support/resistance levels for a given market on an exchange
         void findSupportResistanceLevels(Exchange exch, string market, out decimal? support, out decimal? resistance);
    }
}