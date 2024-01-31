using TMPro;
using UnityEngine;

namespace EntryPoint
{
    public class DebugConsole : MonoBehaviour, IDebugConsole
    {
        [SerializeField]
        private TextMeshProUGUI[] _messages;

        private int _index;

        public void Initialize()
        {
            _index = 0;
            
            foreach (var message in _messages) 
                message.gameObject.SetActive(false);
            
            DontDestroyOnLoad(gameObject);
        }

        public void Post(string message)
        {
            if (_index < _messages.Length)
            {
                TextMeshProUGUI text = _messages[_index++];
                text.gameObject.SetActive(true);
                text.text = message;
            }
            else
            {
                var text = _messages[0];
                string oldMessage = text.text;
                text.text = message;
                
                for (int i = 1; i < _messages.Length; i++)
                {
                    text = _messages[i];
                    (text.text, oldMessage) = (oldMessage, text.text);
                }
            }
        }
    }
    
    public class EmptyConsole : IDebugConsole 
    {
        public void Initialize() { }
        public void Post(string message) { }
    }
}