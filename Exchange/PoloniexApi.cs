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

namespace CryptoBot.Exchange {

    public class PoloniexApiException : Exception {
        public PoloniexApiException(string msg) : base(msg){}
    }

    public class PoloniexApi {
        public int nonce = 1;
        public HMACSHA512 hmac = new HMACSHA512(Encoding.UTF8.GetBytes(PoloniexConfig.secret));

        public string getNonce() {
            return nonce++.ToString();
        }
        public void test() {
			//getTickers();
            //getOrderBook("BTC_ETH", 10);
            getBalances();
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
        private JObject makePublicRequest(string uri) {
            var request = WebRequest.Create(new Uri(uri));
            if(request == null) {
                throw new Exception("Invalid request: " + uri);
            }
            try {
                var response = request.GetResponse();
                var stream = response.GetResponseStream();
                var streamReader = new StreamReader(stream);
                JObject jsonObj = JObject.Parse(streamReader.ReadToEnd());
                return jsonObj;
            }catch(Exception e) {
                Console.WriteLine(e.Message);
                throw new PoloniexApiException("Simple Request Failed: " + uri);
            }
        }
        private JObject makePrivateRequest(string command, Dictionary<string, string> args=null) {
            var request = WebRequest.Create(PoloniexConfig.poloniexApiPrivateBaseUrl);

            //add command and nonce
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
            //Console.WriteLine(sig);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            request.Headers.Add("Key", PoloniexConfig.apiKey);
            request.Headers.Add("Sign", sig);

            try {
                using (var stream = request.GetRequestStream()) {
                    stream.Write(data, 0, data.Length);
                }
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                JObject jsonObj = JObject.Parse(responseString);
                //Console.WriteLine(responseString);                
                return jsonObj;
            }catch(Exception e) {
                throw new PoloniexApiException("Private request failed: " + e.Message);
            }
        }

        public void getBalances() {
            makePrivateRequest("returnBalances");
        }
        public JObject getTickers() {
            string url = makeRequestUrl("returnTicker");
            var res = makePublicRequest(url);
            Console.WriteLine(res);
            return res;
        }

		public JObject getOrderBook(string ticker, int depth=10) {
            Dictionary<string,string> args = new Dictionary<string,string>(){
                {"currencyPair", ticker},
                {"depth", depth.ToString()}
            };
            string url = makeRequestUrl("returnOrderBook", args);

			var res = makePublicRequest(url);
			Console.WriteLine(res);
			return res;
		}
    }
}