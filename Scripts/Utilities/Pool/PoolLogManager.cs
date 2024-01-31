// using UnityEngine;
// using System.Collections.Generic;
// using System;
// using Utilities.Pool;
//
// namespace MergeMarines
// {
//     public class PoolLogManager : MonoBehaviour
//     {
//         private Dictionary<PooledBehaviour, int> _poolInstantiations = new Dictionary<PooledBehaviour, int>();
//
//         private void OnEnable()
//         {
//             GameManager.GameStarted += GameManager_GameStarted;
//             GameManager.GameFinished += GameManager_GameFinished;
//
//             Pool.EntryInstantiated += Pool_EntryInstantiated;
//         }
//
//         private void OnDisable()
//         {
//             GameManager.GameStarted -= GameManager_GameStarted;
//             GameManager.GameFinished -= GameManager_GameFinished;
//
//             Pool.EntryInstantiated -= Pool_EntryInstantiated;
//         }
//         
//         private void Pool_EntryInstantiated(PooledBehaviour prefab, int count)
//         {
//             // NOTE: пока игра не началась считаем что пул преперится
//             bool canLog = GameManager.Instance.IsGameStarted && DebugSafe.CanLog(DebugType.Pool);
//
//             if (!canLog)
//             {
//                 return;
//             }
//
//             if (Application.isEditor)
//             {
//                 Debug.LogErrorFormat("[{0}] Pool_EntryInstantiated: {1} ({2}), count: {3}", GetType().Name, prefab.name, prefab.GetType().Name, count);
//             }   
//
//             if (_poolInstantiations.TryGetValue(prefab, out int totalCount))
//             {
//                 totalCount += count;
//             }
//             else
//             {
//                 totalCount = count;
//             }   
//
//             _poolInstantiations[prefab] = totalCount;
//
// #if UNITY_EDITOR
//
//             Pool.Instance.GetBusyCounts().TryGetValue(prefab, out int busyCount);
//             Pool.Instance.GetFreeCounts().TryGetValue(prefab, out int freeCount);
//
//             Debug.LogWarningFormat("[{0}] Pool_EntryInstantiated: {1}, Free: {2}, Busy: {3}", GetType().Name, prefab.name, freeCount, busyCount);
// #endif
//         }
//     }
// }
