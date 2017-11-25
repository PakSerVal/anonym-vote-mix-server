using System;
using System.Collections;
using System.Numerics;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using MixCustom.Models.Entities;

namespace MixCustom.Models
{
    public class Bulletins
    {
        private Config config;

        public Bulletins(Config config)
        {
            this.config = config;
        }

        public T[] Shuffle<T>(T[] OriginalArray)
        {
            var matrix = new SortedList();
            var r = new Random();
            for (var x = 0; x <= OriginalArray.GetUpperBound(0); x++)
            {
                var i = r.Next();

                while (matrix.ContainsKey(i)) { i = r.Next(); }

                matrix.Add(i, OriginalArray[x]);
            }
            var OutputArray = new T[OriginalArray.Length];
            matrix.Values.CopyTo(OutputArray, 0);
            return OutputArray;
        }

        public void ReEncrypt(Bulletin[] bulletins)
        {
            BigInteger p = BigInteger.Parse(config.MixNetKey["p"]);
            BigInteger g = BigInteger.Parse(config.MixNetKey["g"]);
            BigInteger y = BigInteger.Parse(config.MixNetKey["y"]);
            foreach (Bulletin bulletin in bulletins)
            {
                BigInteger s = RandomIntegerBelow(p);
                bulletin.Data.a = BigInteger.Pow(g, 1024).ToString();
                bulletin.Data.b = BigInteger.Pow(y, 1024).ToString();
            }
        }
        
        public BigInteger RandomIntegerBelow(BigInteger N)
        {
            byte[] bytes = N.ToByteArray();
            BigInteger R;
            var random = new Random();
            do
            {
                random.NextBytes(bytes);
                bytes[bytes.Length - 1] &= (byte)0x7F;
                R = new BigInteger(bytes);
            } while (R >= N);
            return R;
        }
        
        public bool sendToNextMix(Bulletin[] bulletins)
        {
            string serializedBulletins = JsonConvert.SerializeObject(bulletins);
            if (requestToMix(serializedBulletins, config.Next_MixServer_Url, "api/ppg/handle-bulletins").IsSuccessStatusCode)
                return true;
            return false;
        }
        
        private HttpResponseMessage requestToMix(string serializedObjectData, string baseUrl, string actioUri)
        {
            var client = new HttpClient();
            var content = new StringContent(serializedObjectData, Encoding.UTF8, "application/json");
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client.PostAsync(actioUri, content).Result;
        }
    }
}
