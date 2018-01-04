
using System.Collections.Generic;

namespace CryptoBot.Constants {
    class PoloniexConfig {
        public const string poloniexApiPublicBaseUrl = "https://poloniex.com/public";
        public const string poloniexApiPrivateBaseUrl = "https://poloniex.com/tradingApi";
        public const string secretsFile = "my_secrets.cfg";
        public const string nonceFile = "last_nonce";
    }

    public class TradingSettings {
        public static readonly string[] supportedCoins = {"BTC", "LTC", "BCH", "ETH"};

        public const int pollRate = 15;
        
    }
}