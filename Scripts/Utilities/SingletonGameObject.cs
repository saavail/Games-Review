using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using EntryPoint;
using UnityEngine;

namespace Utilities
{
    public class SingletonGameObject<T> : MonoBehaviour, IAsyncInitializable
        where T : SingletonGameObject<T>, new()
    {
        private static T _instance = null;
        private static bool _dontDestroyOnLoad = true;
        private static readonly string PrefabPath = $"GoSingletons/{typeof(T).Name}";
        private static string _parentName = "GOSingletons";

        private static bool _isGettingInstance = false;

        public static bool IsAlive
        {
            get { return (_instance != null); }
        }

        public static T Instance
        {
            get
            {
                return GetInstance();
            }
        }

        public static T TryInstance()
        {
            return _instance;
        }

        private void Awake()
        {
            // GetInstance(this);
        }

        private void Start()
        {
            if (_instance != this as T)
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (_instance == this as T)
            {
                FreeInstance();
            }
        }

        protected virtual void Init()
        {
        }

        protected virtual void DeInit()
        {
        }

        public static void SetForceInstance(T instance)
        {
            _instance = instance;
            _instance.Init();
        }
        
        public async UniTask InitializeAsync()
        {
            await LoadAndInstantiateAsync();
        }
        
        public static async UniTask LoadAndInstantiateAsync()
        {
            if (_instance != null || _isGettingInstance)
                return;
            
#if DEBUG_SINGLETON
			Debug.Log(string.Format("SingletonGameObject of '{0}' created!", typeof(T)));
#endif

            GameObject prefab;

            // Load resource
            var request = Resources.LoadAsync(PrefabPath);
            var loadedPrefab = await request;

            if (loadedPrefab)
            {
                prefab = Instantiate(loadedPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            }
            else
            {
                prefab = new GameObject(typeof(T).Name);
                prefab.AddComponent(typeof(T));
            }

            bool canDontDestroyOnLoad = Application.isPlaying;

            // Move to parent
            if (_parentName != null)
            {
                string name;
                if (_dontDestroyOnLoad)
                {
                    name = _parentName + "_DontDestroy";
                }
                else
                {
                    name = _parentName + "_Destroy";
                }

                GameObject parent = GameObject.Find("/" + name);
                if (!parent) parent = new GameObject(name);
                if (_dontDestroyOnLoad && canDontDestroyOnLoad)
                {
                    DontDestroyOnLoad(parent);
                }

                prefab.transform.SetParent(parent.transform);
            }
            else if (_dontDestroyOnLoad && canDontDestroyOnLoad)
            {
                // Apply don't destroy on load only for root objects. Otherwise Unity will create warnings.
                DontDestroyOnLoad(prefab);
            }

            // Get component
            T component = prefab.GetComponent<T>();
            
            if (component == null)
                return;

            _instance = component;

            // Init singleton
            _instance.Init();

            _isGettingInstance = false;
        }

        public static T GetInstance(SingletonGameObject<T> forcedInstance = null)
        {
            if (_instance == null && !_isGettingInstance)
            {
                _isGettingInstance = true;
#if DEBUG_SINGLETON
			    Debug.Log(string.Format("SingletonGameObject of '{0}' created!", typeof(T)));
#endif

                GameObject prefab = forcedInstance?.gameObject;

                if (prefab == null)
                {
                    bool result = true;

                    // Load resource
                    Object resource = Resources.Load(PrefabPath);
                    if (resource == null)
                    {
                        result = false;
                    }

                    // Instantiate
                    if (resource as GameObject == null)
                    {
                        result = false;
                    }

                    if (result)
                    {
                        prefab = Instantiate(resource, Vector3.zero, Quaternion.identity) as GameObject;
                    }
                    else
                    {
                        prefab = new GameObject(typeof(T).Name);
                        prefab.AddComponent(typeof(T));
                    }
                }

                bool canDontDestroyOnLoad = true;

#if UNITY_EDITOR
                // ������� � Unity 5.3.4, �������� ������, ���� �� �������� DontDestroyOnLoad � ������ ��������������
                if (!Application.isPlaying)
                {
                    canDontDestroyOnLoad = false;
                }
#endif

                // Move to parent
                if (_parentName != null)
                {
                    string name;
                    if (_dontDestroyOnLoad)
                    {
                        name = _parentName + "_DontDestroy";
                    }
                    else
                    {
                        name = _parentName + "_Destroy";
                    }

                    GameObject parent = GameObject.Find("/" + name);
                    if (!parent) parent = new GameObject(name);
                    if (_dontDestroyOnLoad && canDontDestroyOnLoad)
                    {
                        DontDestroyOnLoad(parent);
                    }
                    prefab.transform.SetParent(parent.transform);
                }
                else if (_dontDestroyOnLoad && canDontDestroyOnLoad)
                {
                    // Apply don't destroy on load only for root objects. Otherwise Unity will create warnings.
                    DontDestroyOnLoad(prefab);
                }

                // Get component
                T component = prefab.GetComponent<T>();
                if (component == null)
                {
                    return null;
                }
                _instance = component;

                // Init singleton
                _instance.Init();

                _isGettingInstance = false;
            }
            return _instance;
        }

        public static void FreeInstance()
        {
            if (_instance)
            {
#if DEBUG_SINGLETON
			Debug.Log(string.Format("SingletonGameObject of '{0}' destroyed!", typeof(T)));
#endif
                _instance.DeInit();
                GameObject go = _instance.gameObject;
                _instance = null;

                // если объект уже уничтожен, он будет null
                if (go != null)
                {
                    if (Application.isPlaying)
                        Destroy(go);
                    else
                        DestroyImmediate(go);
                }
            }
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void FreeInstanceImmediate()
        {
            if (_instance)
            {
#if DEBUG_SINGLETON
			Debug.Log(string.Format("SingletonGameObject of '{0}' destroyed!", typeof(T)));
#endif
                _instance.DeInit();
                GameObject go = _instance.gameObject;
                _instance = null;
                DestroyImmediate(go);
                go = null;
            }
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void FreeInstanceImmediateWithParent()
        {
            if (_instance)
            {
#if DEBUG_SINGLETON
			Debug.Log(string.Format("SingletonGameObject of '{0}' destroyed!", typeof(T)));
#endif
                _instance.DeInit();
                GameObject go = _instance.gameObject;

                if (go.transform.parent != null) DestroyImmediate(go.transform.parent.gameObject);
                else DestroyImmediate(go);

                _instance = null;
                go = null;
            }
        }
    }
}