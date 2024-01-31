using UnityEngine;

namespace Core.UserStuff
{
    [CreateAssetMenu(menuName = "Scriptables/" + nameof(RemoteBalanceData), fileName = nameof(RemoteBalanceData))]
    public class RemoteBalanceData : ScriptableObject
    {
        [SerializeField]
        public RemoteBalance RemoteBalance;
    }
}