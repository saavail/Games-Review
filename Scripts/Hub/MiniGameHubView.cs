using System;
using Core;
using Cysharp.Threading.Tasks;
using DependencyInjector;
using Enums;
using FlappyPlane.Scripts;
using Games;
using Games.RoosterGame.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hub
{
    public class MiniGameHubView : MonoBehaviour
    {
        [SerializeField]
        private GameType _gameType;
        [SerializeField]
        private TextMeshProUGUI _nameText;
        [SerializeField]
        private TextMeshProUGUI _progressText;
        [SerializeField]
        private TextMeshProUGUI _progressCountText;
        [SerializeField]
        private Image _mainSprite;
        [SerializeField]
        private Button _mainButton;

        private void Start()
        {
            Localize();
            
            _mainButton.onClick.AddListener(() =>
            {
                switch (_gameType)
                {
                    case GameType.Game2048:
                        AllServices.Container.Single<GamesService>().Factory.Create2048().Forget();
                        break;
                   
                    case GameType.FlappyPlane:
                        AllServices.Container.Single<GamesService>().Factory.CreateFlappyPlane().Forget();
                        break;
                    
                    case GameType.RoosterRunner:
                        AllServices.Container.Single<GamesService>().Factory.CreateRoosterRunner().Forget();
                        break;
                    
                    case GameType.Stack:
                        AllServices.Container.Single<GamesService>().Factory.CreateStack().Forget();
                        break;
                }
            });
        }

        private void Localize()
        {
            _progressText.text = Strings.CommonProgress;
            
            switch (_gameType)
            {
                case GameType.Game2048:
                    _nameText.text = Strings.Game2048;
                    break;
                
                case GameType.FlappyPlane:
                    _nameText.text = Strings.GameFlappyPlane;
                    break;
                
                case GameType.RoosterRunner:
                    _nameText.text = Strings.GameRoosterRunner;
                    break;
            }
        }

        private void OnEnable()
        {
            _progressCountText.text = GetMaxScore().ToString();
        }

        private int GetMaxScore()
        {
            switch (_gameType)
            {
                case GameType.Game2048:
                    return GetMaxScore(typeof(Game2048));
                
                case GameType.FlappyPlane:
                    return GetMaxScore(typeof(GameFlappyPlane));
            }
            
            return 0;
        }

        private int GetMaxScore(Type type)
        {
            return PlayerPrefs.GetInt($"{type.Name}MaxScoreSaveKey", 0);
        }
    }
}