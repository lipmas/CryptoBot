# CryptoBot
A WIP simple cryptocurrency trading bot as a C# console application

# Overview of code
- ```Exchanges/Exchange```: abstract exchange class which represents a generic interface and data source for a cryptcurrency exchange. It contains data about the current order book and prices, historical trades, your balances.
- ```PoloniexExchangeApi```: wraps around Poloniex's public REST APIs and ```PoloniexExchange``` is a wrapper class to make it confirm to the common exchange interface, so other exchanges could more easily be supported in the future. 
- ```TechnicalTradingStrategy```: the purpose of a trading strategy class should be able to wrap the exchange interface and inform the bot on which trades to make and when. Another component that is not currently implemented would then execute this trade plan as efficiently as possible without slipping from the target price. This particular strategy implements technical analysis and support/resistance finding algorithms.
- ```Bots/TechnicalTradingBot```: contains an example implementation of a bot that puts all the pieces together. The goal of this library was to be flexible enough to create bots that could use any exchange source and many pluggable trading strategies. It starts using the TA strategy and polls for market prices waiting for one of these S/R levels to be hit.

# Running the code

After installing .NET SDK
```
dotnet run
```
Should start the bot running the default fibonacci support/resistance strategy
- Any key-press will terminate the program
- Only console trace of potential buys/sell opportunities will appear for now
- Can modify trading parameters [here](https://github.com/lipmas/CryptoBot/blob/master/Constants/TradeParamaters.cs)

## Running with Poloniex trading API enabled
- Must currently set tradingEnabled to true in trading parameters
- Must replace secrets.cfg file with your Poloniex secret key and api key
- This is required for making any authenticated API requests to poloniex including to see your balances
- No trade execution strategies are currently implemented however there is support in the API for placing orders

# Current Exchanges Supported
- Only Poloniex exchange is currently supported

# Current Strategies Supported
Only supports a couple exteremly simple support/resistance trading algorithms for now that are very unlikely to be profitable
1. One is based on historical data and  TA fibonacci retracement levels
2. The other is based on a simple order book snapshot at a given time
