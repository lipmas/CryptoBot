 namespace CryptoBot.Constants {

    public class TradingParameters {
            //only currency pairs that are all included in supported coins will be used by bot
            public static readonly string[] supportedCoins = {
                "BTC",
                "LTC",
                "BCH",
                "ETH",
                "ZEC",
                "XMR"
            };

            //toggle trading enabled
             public const bool tradingEnabled = false;

            //poll for market price updates in ms
            public const int marketPollRate = 5 * 1000;

            //poll for order book updates in ms
            public const int orderBookPollRate = 5 * 60 * 1000;

            /* 
             * Parameters used for support resistance calculations 
            */
            public const int orderBookDepth = 100;

            //number of slices to divide the orderbook into when aggregating
            public const decimal aggSize = 10;

            //threshold for being at least this far away from the top of the aggregated order book
            public const int awayFromMarketThreshold = 1;

            //how much bigger is this agg qty level than average on the order book
            public const decimal levelThreshold = 1M;          
        }
 }