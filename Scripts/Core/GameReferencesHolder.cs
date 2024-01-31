using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class GameReferencesHolder : MonoBehaviour, IDataCollectable
    {
        [HideInInspector]
        [SerializeField]
        private MonoBehaviour[] _startableHolders;
        
        [HideInInspector]
        [SerializeField]
        private MonoBehaviour[] _deadableHolders;
        
        [HideInInspector]
        [SerializeField]
        private MonoBehaviour[] _restartableHolders;
        
        [HideInInspector]
        [SerializeField]
        private MonoBehaviour[] _exitableHolders;
        
        [HideInInspector]
        [SerializeField]
        private MonoBehaviour[] _saveableHolders;
        
        [HideInInspector]
        [SerializeField]
        private MonoBehaviour[] _scoreChangeableHolders;
        
        [HideInInspector]
        [SerializeField]
        private SkinChanger _skinChanger;

        public IGameStartable[] Startables { get; private set; }
        public IGameDeadable[] Deadables { get; private set; }
        public IGameRestartable[] Restartables { get; private set; }
        public IGameExitable[] Exitables { get; private set; }
        public IGameSaveable[] Saveables { get; private set; }
        public IScoreChangeable[] ScoreChangeables { get; private set; }
        public SkinChanger SkinChanger => _skinChanger;

        public virtual void Initialize()
        {
            Startables = _startableHolders.Cast<IGameStartable>().ToArray();
            Deadables = _deadableHolders.Cast<IGameDeadable>().ToArray();
            Restartables = _restartableHolders.Cast<IGameRestartable>().ToArray();
            Exitables = _exitableHolders.Cast<IGameExitable>().ToArray();
            Saveables = _saveableHolders.Cast<IGameSaveable>().ToArray();
            ScoreChangeables = _scoreChangeableHolders.Cast<IScoreChangeable>().ToArray();
        }
        
        [Button]
        public virtual void CollectData()
        {
            foreach (var dataCollectable in SceneManager.GetActiveScene()
                         .GetRootGameObjects()
                         .SelectMany(i => i.GetComponentsInChildren<IDataCollectable>()))
            {
                if (dataCollectable != (IDataCollectable) this)
                    dataCollectable.CollectData();
            }
            
            _startableHolders = Collect<IGameStartable>();
            _deadableHolders = Collect<IGameDeadable>();
            _restartableHolders = Collect<IGameRestartable>();
            _exitableHolders = Collect<IGameExitable>();
            _saveableHolders = Collect<IGameSaveable>();
            _scoreChangeableHolders = Collect<IScoreChangeable>();

            _skinChanger = FindObjectOfType<SkinChanger>();
        }

        protected MonoBehaviour[] Collect<T>()
        {
            return SceneManager.GetActiveScene()
                .GetRootGameObjects()
                .SelectMany(i => i.GetComponentsInChildren<T>())
                .Distinct()
                .Cast<MonoBehaviour>()
                .ToArray();
        }
    }
}