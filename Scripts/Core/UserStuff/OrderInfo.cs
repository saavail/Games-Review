using System;

namespace Core.UserStuff
{
    public class OrderInfo
    {
        public const string BoughtStatus = "order_confirmed";
        
        public int SlotId { get; }
        public string Id { get; private set; }
        public bool IsBought { get; private set; }

        public OrderInfo(int slotId)
        {
            SlotId = slotId;
            IsBought = true;
        }
        
        public OrderInfo(int slotId, string id, string status)
        {
            SlotId = slotId;
            Id = id;
            
            SetStatus(status);
        }

        public void SetStatus(string status)
        {
            IsBought = status.Equals(BoughtStatus, StringComparison.InvariantCulture);
        }

        public void SetOrderId(string orderId)
        {
            Id = orderId;
        }
    }
}