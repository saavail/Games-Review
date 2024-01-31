using UnityEngine;

namespace EntryPoint
{
    [CreateAssetMenu(menuName = "Scriptables/" + nameof(GameSettingsData), fileName = nameof(GameSettingsData))]
    public class GameSettingsData : ScriptableObject
    {
        [SerializeField]
        private LogType _logType;

        public LogType LogType => _logType;
    }
}