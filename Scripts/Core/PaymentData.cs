using System;
using Enums;
using UnityEngine;

namespace Core
{
    [Serializable]
    public struct PaymentData
    {
        [SerializeField]
        private CurrencyType _currencyType;
        [SerializeField]
        private float _value;

        public CurrencyType CurrencyType => _currencyType;
        public float Value => _value;
    }
}