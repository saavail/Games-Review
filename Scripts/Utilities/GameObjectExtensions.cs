using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities
{
    public static class StaticGameObjectExtension
    {
        public static Coroutine InvokeWithDelay(this MonoBehaviour self, float delay, Action method)
        {
            if (self.enabled == false)
            {
                Debug.LogWarning("InvokeWithDelay couldn't be called from inactive MonoBehaviour");
                return null;
            }
            return self.StartCoroutine(InvokeWithDelayCor(self, delay, method));
        }

        public static Coroutine InvokeWithDelayRealtime(this MonoBehaviour self, float delay, Action method)
        {
            if (self.enabled == false)
            {
                Debug.LogWarning("InvokeWithDelayRealtime couldn't be called from inactive MonoBehaviour");
                return null;
            }
            return self.StartCoroutine(InvokeWithDelayRealtimeCor(self, delay, method));
        }

        public static Coroutine InvokeWithFrameDelay(this MonoBehaviour self, int frames, Action method)
        {
            if (self.enabled == false)
            {
                Debug.LogWarning("InvokeWithFrameDelay couldn't be called from inactive MonoBehaviour");
                return null;
            }
            return self.StartCoroutine(InvokeWithFrameDelayCor(self, frames, method));
        }

        public static Coroutine InvokeWithFrameEndDelay(this MonoBehaviour self, Action method)
        {
            if (self.enabled == false)
            {
                Debug.LogWarning("InvokeWithFrameDelay couldn't be called from inactive MonoBehaviour");
                return null;
            }
            return self.StartCoroutine(InvokeWithFrameEndDelayCor(self, method));
        }

        public static void SetLayerRecursively(this GameObject obj, int newLayer)
        {
            obj.layer = newLayer;

            foreach (Transform curChild in obj.transform)
            {
                curChild.gameObject.SetLayerRecursively(newLayer);
            }
        }

        //returns true if object state was changed
        public static bool TrySetActive(this GameObject obj, bool value)
        {
            if (obj == null) return false;
            if (obj.activeSelf != value)
            {
                obj.SetActive(value);
                return true;
            }
            return false;
        }

        private static IEnumerator InvokeWithDelayCor(MonoBehaviour self, float delay, Action method)
        {
            yield return new WaitForSeconds(delay);
            try
            {
                method();
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("[InvokeWithDelay] - exception on action : name:{0}; ex: {1}", self.gameObject.name, ex.Message);
                throw;
            }
        }

        private static IEnumerator InvokeWithDelayRealtimeCor(MonoBehaviour self, float delay, Action method)
        {
            yield return new WaitForSecondsRealtime(delay);
            try
            {
                method();
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("[InvokeWithDelayRealtime] - exception on action : name:{0}; ex: {1}", self.gameObject.name, ex.Message);
                throw;
            }
        }

        private static IEnumerator InvokeWithFrameDelayCor(MonoBehaviour self, int frames, Action method)
        {
            for (int i = 0; i < frames; i++)
            {
                yield return null;
            }

            try
            {
                method();
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("[InvokeWithFrameDelay] - exception on action : name:{0}; ex: {1}", self.gameObject.name, ex.Message);
                throw;
            }
        }

        private static IEnumerator InvokeWithFrameEndDelayCor(MonoBehaviour self, Action method)
        {
            yield return new WaitForEndOfFrame();
            try
            {
                method();
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("[InvokeWithFrameEndDelay] - exception on action : name:{0}; ex: {1}", self.gameObject.name, ex.Message);
                throw;
            }
        }
    
        public static string GetHierarchyPath(this GameObject obj)
        {
            string path = "/" + obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = "/" + obj.name + path;
            }
            return path;
        }
    
        public static void DestroyChildren(this GameObject gameObject)
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                Object.Destroy(gameObject.transform.GetChild(i).gameObject);
            }
        }

#if UNITY_EDITOR
        public static void DestroyChildrenEditor(this GameObject gameObject, bool allowAsset = false)
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                Object.DestroyImmediate(gameObject.transform.GetChild(i).gameObject, allowAsset);
            }
        }
#endif

        public static void StopCoroutine(this MonoBehaviour behaviour, ref Coroutine coroutine)
        {
            if (coroutine != null)
            {
                behaviour.StopCoroutine(coroutine);
                coroutine = null;
            }
        }
    }
}
