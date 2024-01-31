using System;
using DependencyInjector;
using EntryPoint.Save;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EntryPoint
{
    public class Game : AsyncInitializableAndLoad<GameSettingsData>, IGameService
    {
        private readonly ApplicationEvents _applicationEvents;
        
        protected override string LoadPath => nameof(GameSettingsData);

        public Game(IResourceLoader resourceLoader)
            : base(resourceLoader)
        {
            var gameObject = new GameObject("ApplicationEventsReceiver");
            Object.DontDestroyOnLoad(gameObject);
            _applicationEvents = gameObject.AddComponent<ApplicationEvents>();
            
            _applicationEvents.OnFocus += ApplicationEvents_OnFocus;
        }

        private void ApplicationEvents_OnFocus(bool hasFocus)
        {
            if (hasFocus)
                return;

            foreach (var service in AllServices.Container.GetAll<ISaveableService>()) 
                service.Save();
            
            PlayerPrefs.Save();
        }
    }
}