using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoPoolMiner.API
{
    public class NanoPoolXMRApi
    {
        const string BaseUrl = "https://api.nanopool.org/v1/xmr";
        string _wallet;

        public void SetWallet(string wallet)
        {
            _wallet = wallet;
        }

        public T Execute<T>(RestRequest request) where T : new()
        {
            var client = new RestClient();
            client.BaseUrl = new System.Uri(BaseUrl);
            //client.Authenticator = new HttpBasicAuthenticator(_accountSid, _secretKey);
            if (request.Resource.Contains("{wallet}"))
            {
                request.AddParameter("wallet", _wallet, ParameterType.UrlSegment); // used on every request
            }
            var response = client.Execute<T>(request);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response.  Check inner details for more info.";
                var exception = new ApplicationException(message, response.ErrorException);
                throw exception;
            }
            return response.Data;
        }

        public class Return<T>
        {
            public bool status { get; set; }
            public string error { get; set; }
            public T data { get; set; }
        }

        public class AccountInfo
        {
            public decimal balance { get; set; }
            public decimal unconfirmed_balance { get; set; }
            public decimal hashrate { get; set; }
            public AverageHashrate avgHashrate { get; set; }
            public List<Worker> workers { get; set; }
        }

        public class AverageHashrate
        {
            public decimal h1 { get; set; }
            public decimal h3 { get; set; }
            public decimal h6 { get; set; }
            public decimal h12 { get; set; }
            public decimal h24 { get; set; }
        }

        public class Worker
        {
            public string id { get; set; }
            public decimal hashrate { get; set; }
            public DateTime lastshare { get; set; }
            public int rating { get; set; }
            public decimal h1 { get; set; }
            public decimal h3 { get; set; }
            public decimal h6 { get; set; }
            public decimal h12 { get; set; }
            public decimal h24 { get; set; }
        }

        public class Price
        {
            public decimal price_btc { get; set; }
            public decimal price_usd { get; set; }
            public decimal price_eur { get; set; }
        }

        public decimal GetBalance()
        {
            var request = new RestRequest();
            request.Resource = "balance/{wallet}";

            return ProcessRequest<decimal>(request);
        }

        public AccountInfo GetAccountInfo()
        {
            var request = new RestRequest();
            request.Resource = "user/{wallet}";

            return ProcessRequest<AccountInfo>(request);
        }

        public Price GetPrice()
        {
            var request = new RestRequest();
            request.Resource = "prices";

            return ProcessRequest<Price>(request);
        }

        private T ProcessRequest<T>(RestRequest request)
        {
            var ret = Execute<Return<T>>(request);

            if (ret.status)
            {
                return ret.data;
            }
            else
            {
                throw new Exception(ret.error ?? "request failed");
            }
        }
    }
}
