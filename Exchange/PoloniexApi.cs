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
        public int nonce = 2;
        public HMACSHA512 hmac;

        private string api_key;
        private string api_secret; 

        public PoloniexApi() {
            //TODO store these password encrypted in a file instead of in plain text
            var secrets = Util.readKeyValuesFromFile(PoloniexConfig.secretsFile);
            api_secret = secrets["API_SECRET"];
            api_key = secrets["API_KEY"];
            hmac = new HMACSHA512(Encoding.UTF8.GetBytes(api_secret));

            if(File.Exists(PoloniexConfig.nonceFile)){
                nonce =  Convert.ToInt32(Util.readKeyValuesFromFile(PoloniexConfig.nonceFile)["nonce"]);
            }
        }

        public void test() {
			getTickers();
            getOrderBook("BTC_ETH", 10);
            getBalances();
        }
        
        public string getNonce() {
            var last_nonce = nonce;
            nonce++;
            //persist current nonce to file
            File.WriteAllText(PoloniexConfig.nonceFile, "nonce=" + nonce.ToString());
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
            Console.WriteLine(api_key);
            Console.WriteLine(api_secret);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            request.Headers.Add("Key", api_key);
            request.Headers.Add("Sign", sig);
            Console.WriteLine(postData);

            try {
                using (var stream = request.GetRequestStream()) {
                    stream.Write(data, 0, data.Length);
                }
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                JObject jsonObj = JObject.Parse(responseString);
                Console.WriteLine(responseString);                
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