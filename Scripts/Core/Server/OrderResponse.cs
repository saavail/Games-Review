using System;

namespace Core.Server
{
    [Serializable]
    public class OrderResponse
    {
        public string id;
        public string orderId;
        public string name;
        public string status;
        public string amount;
        public string totalAmount;
        public string eosName;
        public string currency;
        public string link;
    }
}