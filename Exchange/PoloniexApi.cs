using System;
using System.IO;
using System.Net;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CryptoBot.Exchange {

    public class PoloniexApiException : Exception {
        public PoloniexApiException(string msg) : base(msg){}
    }

    public class PoloniexApi {
        private const string baseUrl = "https://poloniex.com/public";

        public void test() {
			getTickers();
            getOrderBook("BTC_ETH", 10);
        }

        public JObject getTickers() {
            string uri = baseUrl + "?command=returnTicker";
            var res = makePublicRequest(uri);
            Console.WriteLine(res);
            return res;
        }

		public JObject getOrderBook(string ticker, int depth) {
			string uri = baseUrl + "?command=returnOrderBook"
			+ "&currencyPair=" + ticker 
			+ "&depth=" + depth;

			var res = makePublicRequest(uri);
			Console.WriteLine(res);
			return res;
		}

		/* 
 		private string SendPostRequest(string url, Dictionary<string, string> args)
        {
            url = _baseURL + url;
            var dataStr = BuildPostData(args);
            var data = Encoding.ASCII.GetBytes(dataStr);
            var request = WebRequest.Create(new Uri(url));

            request.Method = "POST";
            request.Timeout = 15000;
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var reqStream = request.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            var response = request.GetResponse();
            using (var resStream = response.GetResponseStream())
            {
                using (var resStreamReader = new StreamReader(resStream))
                {
                    string resString = resStreamReader.ReadToEnd();
                    return resString;
                }
            }
        }
		*/

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
    }
}