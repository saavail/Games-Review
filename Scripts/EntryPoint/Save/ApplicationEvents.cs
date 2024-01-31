using System;
using UnityEngine;

namespace EntryPoint.Save
{
    public class ApplicationEvents : MonoBehaviour
    {
        public event Action<bool> OnFocus; 
        public event Action<bool> OnPause; 

        private void OnApplicationFocus(bool hasFocus)
        {
            OnFocus?.Invoke(hasFocus);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            OnPause?.Invoke(pauseStatus);
        }
    }
}