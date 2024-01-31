using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public static class MonoBehaviourUtility
    {
        public static void SpawnOrRefreshList<TBehaviour>(List<TBehaviour> currentArr, int sourceArrLength, Transform parent, 
            TBehaviour prefab, Action<TBehaviour, int> setupAction)
            where TBehaviour : MonoBehaviour
        {
            int index = 0;
            for (; index < sourceArrLength && index < currentArr.Count; index++)
            {
                currentArr[index].gameObject.SetActive(true);
                setupAction(currentArr[index], index);
            }

            for (; index < sourceArrLength; index++)
            {
                TBehaviour behaviour = UnityEngine.Object.Instantiate(prefab, parent);
                setupAction(behaviour, index);
                currentArr.Add(behaviour);
            }

            for (; index < currentArr.Count; index++)
            {
                currentArr[index].gameObject.SetActive(false);
            }
        }
    }
}