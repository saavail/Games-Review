using System;
using Enums;

namespace Core.UserStuff
{
    [Serializable]
    public class Currency
    {
        public CurrencyType currencyType;
        public float amount;
    }
}