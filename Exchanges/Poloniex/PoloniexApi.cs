using System;
using System.IO;
using System.Text;
using System.Net;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;

using CryptoBot.Constants;
using CryptoBot.Utility;

namespace CryptoBot.ExchangeApi.Poloniex {

    public class PoloniexApiException : Exception {
        public PoloniexApiException(string msg) : base(msg){}
    }

    /*
     * Interacts with Poloniex's REST api
     * Implements public data api calls using GET request
     * Implements private/authenticated trade requests using POST request
     * Currently only implements a small subset of the api requests
     * TODO: use Poloniex's push api through websockets to receive market tick updates instead of polling
     * TODO: add caching/flood protection if many bots will be using the same exchange object
     */
    public class PoloniexApi {
        public int nonce;
        public HMACSHA512 hmac;
        private string api_key;
        private string api_secret; 

        public PoloniexApi() {
            //TODO these should be stored encrypted in a file instead of in plain text
            var secrets = Util.readKeyValuesFromFile(PoloniexConfig.secretsFile);
            api_secret = secrets["API_SECRET"];
            api_key = secrets["API_KEY"];
            hmac = new HMACSHA512(Encoding.UTF8.GetBytes(api_secret));

            //recover previous last nonce from file
            if(File.Exists(PoloniexConfig.nonceFile)){
                nonce =  Convert.ToInt32(Util.readKeyValuesFromFile(PoloniexConfig.nonceFile)["poloniex_nonce"]);
            }else{
                nonce = 1;
            }
        }

        //Nonce is required monotonically increasing value that must be included
        //Persist it to file so that the bot can recover latest nonce on restart
        public string getNonce() {
            var last_nonce = nonce;
            nonce++;
            File.WriteAllText(PoloniexConfig.nonceFile, "poloniex_nonce=" + nonce.ToString());
            return last_nonce.ToString();
        }
        private string makeRequestUrl(string command, Dictionary<string, string> args=null) {
            string url = PoloniexConfig.poloniexApiPublicBaseUrl;
            url += "?command=" + command;
            if(args != null) {
                foreach(var arg in args) {
                    url += "&" + arg.Key + "=" + arg.Value;
                }
            }
            return url;
        }
        private string makePublicRequest(string uri) {
            var request = WebRequest.Create(new Uri(uri));
            if(request == null) {
                throw new Exception("Invalid request: " + uri);
            }
            try {
                var response = request.GetResponse();
                var stream = response.GetResponseStream();
                var streamReader = new StreamReader(stream);
                return streamReader.ReadToEnd();
            }catch(Exception e) {
                Console.WriteLine(e.Message);
                throw new PoloniexApiException("Simple Request Failed: " + uri);
            }
        }
        private JObject makePrivateRequest(string command, Dictionary<string, string> args=null) {
            var request = WebRequest.Create(PoloniexConfig.poloniexApiPrivateBaseUrl);

            //add command and nonce to every request
            string postData = "";
            postData += "command=" + command;
            postData += "&nonce=" + getNonce();
            if(args != null) {
                foreach(var arg in args) {
                    postData += "&" + arg.Key + "=" + arg.Value;
                }
            }
            var data = Encoding.ASCII.GetBytes(postData);
            var sig = Util.ByteArrayToString(hmac.ComputeHash(data));

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            request.Headers.Add("Key", api_key);
            request.Headers.Add("Sign", sig);
            //Console.WriteLine(postData);

            try {
                using (var stream = request.GetRequestStream()) {
                    stream.Write(data, 0, data.Length);
                }
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                JObject jsonObj = JObject.Parse(responseString);
                return jsonObj;
            }catch(Exception e) {
                throw new PoloniexApiException("Private request failed: " + e.Message);
            }
        }

        public JObject getBalances() {
            return makePrivateRequest("returnBalances");
        }

        public JObject getTickers() {
            string url = makeRequestUrl("returnTicker");
            var res = makePublicRequest(url);
            JObject jsonObj = JObject.Parse(res);
            return jsonObj;
        }

		public JObject getOrderBook(string ticker, int depth=10) {
            Dictionary<string,string> args = new Dictionary<string,string>(){
                {"currencyPair", ticker},
                {"depth", depth.ToString()}
            };
            string url = makeRequestUrl("returnOrderBook", args);
			var res = makePublicRequest(url);
            JObject jsonObj = JObject.Parse(res);
			return jsonObj;
		}

        public JArray getTradeHistory(string ticker, int start, int end) {
            Dictionary<string,string> args = new Dictionary<string,string>(){
                {"currencyPair", ticker},
                {"start", start.ToString()},
                {"end", end.ToString()}
            };
            string url = makeRequestUrl("returnTradeHistory", args);
			var res = makePublicRequest(url);
            JArray jsonArr = JArray.Parse(res);
			return jsonArr;

        }
        public bool placeBuyOrder(string currencyPair, decimal rate, decimal amount, bool immediateOrCancel=false, bool postOnly=false) {
            var args = new Dictionary<string, string> {
                {"currencyPair", currencyPair},
                {"rate", rate.ToString()},
                {"amount", amount.ToString()},
            };
            if(immediateOrCancel){
                args.Add("immediateOrCancel","1");
            }
            if(postOnly){
                args.Add("postOnly", "1");
            }
            var ret = makePrivateRequest("buy", args);

            if(ret["error"] != null) {
                Console.WriteLine("Error placing buy order: " + ret["error"]);
                return false;
            }
            return true;
        }

        public bool placeSellOrder(string currencyPair, decimal rate, decimal amount, bool immediateOrCancel=false, bool postOnly=false) {
            var args = new Dictionary<string, string> {
                {"currencyPair", currencyPair},
                {"rate", rate.ToString()},
                {"amount", amount.ToString()},
            };
            if(immediateOrCancel){
                args.Add("immediateOrCancel","1");
            }
            if(postOnly){
                args.Add("postOnly", "1");
            }
            var ret = makePrivateRequest("sell", args);
            if(ret["error"] != null) {
                Console.WriteLine("Error placing sell order: " + ret["error"]);
                return false;
            }
            return true;
        }
        public void test() {
            getTickers();
            getOrderBook("BTC_ETH", 10);
            getBalances();
        }
    }
}