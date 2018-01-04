using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace CryptoBot {
    public class Util {
        public static string ByteArrayToString(byte[] ba) {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
            }
        public static Dictionary<string,string> readKeyValuesFromFile(string filePath) {
            string[] lines = File.ReadAllLines(filePath);
            var dict = new Dictionary<string,string>();
            foreach(var line in lines) {
                string[] arr = line.Split("=");
                dict[arr[0]] = arr[1];
            }
            return dict;
        }
    }
}