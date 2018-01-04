using System;

using CryptoBot.ExchangeApi;

namespace CryptoBot.TechnicalTradingStrategy {
    public interface ITechnicalTradingStrategy {
        //strategy to find the most promising support level for a given market on an exchange
        decimal? findSupportLevel(Exchange exch, string market);

         //strategy to find the most promising resistance level for a given market on an exchange
        decimal? findResistanceLevel(Exchange exch, string market);

    }
}