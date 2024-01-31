using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.PoolStuff
{
    public abstract class Poolable : MonoBehaviour
    {
        private Action _releaseAction;

        private UniTask _task;
        private CancellationTokenSource _tokenSource;
        
        internal void SetReleaseAction(Action releaseAction)
        {
            _releaseAction = releaseAction;
        }

        public void Release()
        {
            _releaseAction?.Invoke();
        }

        public void ReleaseWithDelay(float seconds)
        {
            if (_tokenSource != null && _task.Status is UniTaskStatus.Pending)
            {
                _tokenSource.Cancel();
            }

            _tokenSource = new CancellationTokenSource();
            _task = ReleaseWithDelay(seconds, _tokenSource);
        }

        private async UniTask ReleaseWithDelay(float seconds, CancellationTokenSource tokenSource)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(seconds), cancellationToken: tokenSource.Token);
            Release();
            _tokenSource = null;
        }

        public abstract void FullReset();
    }
}