# CryptoBot
A WIP simple cryptocurrency trading bot as a c# console application

After installing .NET SDK
```
dotnet build
dotnet run
```

# Running with trading enabled mode
- Must currently set tradingEnabled to true [here](https://github.com/lipmas/CryptoBot/blob/master/Constants/TradeParamaters.cs)
- Must replace secrets.cfg file with your Poloniex secret key and api key
- This is required for making any authenticated requests to poloniex including to see your balances
- No trade execution strategies are currently implemented

# Exchanges
Only Poloniex Exchange is currently supported

# Strategies
Only supports a single simple orderbook based support/resistance trading algorithm for now
