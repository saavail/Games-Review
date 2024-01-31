using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DependencyInjector;
using EntryPoint;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UISystem
{
    public sealed class WindowSystem : AsyncInitializableAndLoad<WindowData>, IService
    {
        private readonly List<Window> _spawnedWindows = new();
        private readonly Stack<Window> _windowsStack = new();

        private UIHolder _uiHolder;
        private UserProfile _userProfile;
        private IBackendWaiter _backendWaiter;

        public UserProfile UserProfile => _userProfile;
        public Window Current => _windowsStack.Count == 0 ? null : _windowsStack.Peek();

        public WindowSystem(IResourceLoader resourceLoader)
            : base(resourceLoader)
        {
        }

        public override async UniTask InitializeAsync()
        {
            await base.InitializeAsync();
            
            _uiHolder = Object.Instantiate(Data.UIHolderPrefab);
            _uiHolder.transform.position = new Vector3(0, 0, -100);
            
            _userProfile = Object.Instantiate(Data.UserProfilePrefab, _uiHolder.transform);
            
            Object.DontDestroyOnLoad(_uiHolder.gameObject);
        }

        public TWindow Show<TWindow>(Action callback = null)
            where TWindow : Window
        {
            Window window = _spawnedWindows.FirstOrDefault(i => i.GetType() == typeof(TWindow));
            
            if (window == null)
            {
                Window dataWindow = Data.Popups.FirstOrDefault(i => i.GetType() == typeof(TWindow));

                if (dataWindow == null)
                {
                    AllServices.Container.Single<IDebugConsole>().Post($"trying open {nameof(TWindow)}, but it's not exist in data container");
                    return default;
                }
                
                window = Object.Instantiate(dataWindow, _uiHolder.WindowsRoot);
                window.Setup(this);
                window.Open(callback);
                
                _spawnedWindows.Add(window);
            }
            else if (!window.IsOpen)
            {
                window.Open(callback);
            }
            else
            {
                AllServices.Container.Single<IDebugConsole>().Post($"trying open {nameof(TWindow)}, but he is already open");
                return default;
            }

            _windowsStack.Push(window);
            return (TWindow) window;
        }

        public void CloseCurrentWindow(Action callback = null)
        {
            if (_windowsStack.Count <= 1)
            {
                Debug.LogError("Cant close current window because it's last window in stack");
                return;
            }
        
            _windowsStack.Peek().Close();
            callback?.Invoke();
        }

        public void OnClose(Window window)
        {
            if (Current != window)
                return;

            _windowsStack.Pop();
            
            if (Current != null)
                Current.Refresh();
        }

        public TWindow GetWindow<TWindow>()
            where TWindow : Window
        {
            Window window = _spawnedWindows.FirstOrDefault(i => i.GetType() == typeof(TWindow));
            return (TWindow)window;
        }

        public void CloseAll(Action callback = null)
        {
            while (_windowsStack.Count > 0) 
                _windowsStack.Peek().ForceClose();
            
            callback?.Invoke();
        }
    }
}