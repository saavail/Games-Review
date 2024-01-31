using System;
using System.Collections.Generic;
using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
using DependencyInjector;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.Tutorial
{
    public class RoosterRunnerTutorial : Window
    {
        private const float TutorialDelay = 2f;
        
        [SerializeField]
        private Button _doneButton;
        [SerializeField]
        private Button _continueButton;
        [SerializeField]
        private List<TutorialObject> _tutorialObjects;

        private Queue<TutorialObject> _tutorialObjectsQueue;
        private CancellationTokenSource _cancellationTokenSource;

        protected override void OnOpen()
        {
            _doneButton.onClick.AddListener(DoneButton);
            _continueButton.onClick.AddListener(ContinueButton);

            GenerateQueue();
            ShowTutorialPart( _tutorialObjectsQueue.Dequeue());
            GenerateCancellationToken();
        }
        
        private void GenerateQueue()
        {
            _tutorialObjectsQueue = new Queue<TutorialObject>();
            
            foreach (var tutorialObject in _tutorialObjects)
            {
                _tutorialObjectsQueue.Enqueue(tutorialObject);
            }
        }
        
        private void ShowTutorialPart(TutorialObject tutorialObject)
        {
            tutorialObject.Show();
        }
        
        private void GenerateCancellationToken()
        {
            _cancellationTokenSource?.Cancel();
            
            if (_tutorialObjectsQueue.Count > 0)
            {
                _cancellationTokenSource = new CancellationTokenSource();
                ShowNextTutorialPartWithDelay().Forget();
            }
        }
        
        private async UniTask ShowNextTutorialPartWithDelay()
        {
            var cancellationToken = _cancellationTokenSource.Token;
    
            if (_tutorialObjectsQueue.Count > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(TutorialDelay), cancellationToken: cancellationToken);
                    
                if (!_cancellationTokenSource.IsCancellationRequested && _tutorialObjectsQueue.Count > 0)
                {
                    ContinueButton();
                }
            }
        }
        
        private void ContinueButton()
        {
            if (_tutorialObjectsQueue.Count != 0)
            {
                _cancellationTokenSource?.Cancel();
                
                ShowTutorialPart( _tutorialObjectsQueue.Dequeue());
                ShowTutorialPart( _tutorialObjectsQueue.Dequeue());
                
                if (_tutorialObjectsQueue.Count == 1)
                {
                    ShowTutorialPart( _tutorialObjectsQueue.Dequeue());
                }

                GenerateCancellationToken();
            }
            else
            {
                DoneButton();
            }
        }
        
        private void DoneButton()
        {
            AllServices.Container.Single<GamesService>().Factory.Current.OnTutorialShowed();
            Close();
        }

        protected override void OnClose()
        {
            _cancellationTokenSource.Cancel();
        }

        public override void Refresh() { }
        
        protected override void Localize() { }
    }
}