using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// Safe area implementation for notched mobile devices. Usage:
    ///  (1) Add this component to the top level of any GUI panel. 
    ///  (2) If the panel uses a full screen background image, then create an immediate child and put the component on that instead, with all other elements childed below it.
    ///      This will allow the background image to stretch to the full extents of the screen behind the notch, which looks nicer.
    ///  (3) For other cases that use a mixture of full horizontal and vertical background stripes, use the Conform X & Y controls on separate elements as needed.
    /// </summary>
    [RequireComponent (typeof (RectTransform))]
    [ExecuteAlways] public class SafeArea : MonoBehaviour
    {
        #region Simulations

        /// <summary>
        /// Simulation device that uses safe area due to a physical notch or software home bar. For use in Editor only.
        /// </summary>
        public enum SimulatedDevice
        {
            None,
            iPhoneX,
            iPhoneXsMax,
        }

        /// <summary>
        /// Симулируемое устройство. Можно изменить (даже в PlayMode) в SafeAreaPreview в любой сцене (вызывается круглой голубой кнопкой в правой стороне экрана);
        /// P.S. Менять это значение в коде НЕ НАДО!
        /// </summary>
        public static SimulatedDevice _SimulatedDevice = SimulatedDevice.None;

        /// <summary>
        /// Normalized safe areas for iPhone X with Home indicator (ratios are identical to iPhone Xs). Absolute values:
        ///  PortraitU x=0, y=102, w=1125, h=2202 on full extents w=1125, h=2436;
        ///  PortraitD x=0, y=102, w=1125, h=2202 on full extents w=1125, h=2436 (not supported, remains in Portrait Up);
        ///  LandscapeL x=132, y=63, w=2172, h=1062 on full extents w=2436, h=1125;
        ///  LandscapeR x=132, y=63, w=2172, h=1062 on full extents w=2436, h=1125.
        ///  Aspect Ratio: ~19.5:9.
        /// </summary>
        private static Rect[] NSA_iPhoneX = new Rect[]
        {
            new Rect (0f, 102f / 2436f, 1f, 2202f / 2436f),
            new Rect (132f / 2436f, 63f / 1125f, 2172f / 2436f, 1062f / 1125f)
        };

        /// <summary>
        /// Normalized safe areas for iPhone Xs Max with Home indicator (ratios are identical to iPhone XR). Absolute values:
        ///  PortraitU x=0, y=102, w=1242, h=2454 on full extents w=1242, h=2688;
        ///  PortraitD x=0, y=102, w=1242, h=2454 on full extents w=1242, h=2688 (not supported, remains in Portrait Up);
        ///  LandscapeL x=132, y=63, w=2424, h=1179 on full extents w=2688, h=1242;
        ///  LandscapeR x=132, y=63, w=2424, h=1179 on full extents w=2688, h=1242.
        ///  Aspect Ratio: ~19.5:9.
        /// </summary>
        private static Rect[] NSA_iPhoneXsMax = new Rect[]
        {
            new Rect (0f, 102f / 2688f, 1f, 2454f / 2688f),
            new Rect (132f / 2688f, 63f / 1242f, 2424f / 2688f, 1179f / 1242f)
        };

        #endregion

        private RectTransform m_panelRectTransform;

        private Rect m_lastSafeArea = new Rect (0, 0, 0, 0);
        public Rect LastSafeArea => m_lastSafeArea;

        [SerializeField] private bool conformX = true; // Conform to screen safe area on X-axis (default true, disable to ignore);
        [SerializeField] private bool conformY = true; // Conform to screen safe area on Y-axis (default true, disable to ignore);
        [SerializeField] private float minSafeAreaOffsetTop = 0;
        [SerializeField] private float minSafeAreaOffsetBottom = 0;
        private void Awake ()
        {
            m_panelRectTransform = GetComponent<RectTransform> ();

            if (m_panelRectTransform)
                Refresh ();

            else
            {
                Debug.LogError ("Cannot apply safe area - no RectTransform found on " + gameObject.name + "!", gameObject);
                Destroy (this);
            }
        }

        private void Update ()
        {
            Refresh ();
        }

        private void Refresh ()
        {
            Rect l_safeArea = GetSafeArea ();

            if (l_safeArea != m_lastSafeArea)
                ApplySafeArea (l_safeArea);

#if UNITY_EDITOR
            ApplySafeArea (l_safeArea);
#endif
        }

        // Приоритет установки SafeArea:
        // 1 - из аргумента, если не None;
        // 2 - из static _SimulatedDevice, если не None;
        // 3 - Оригинальная SafeArea (полученная от OS);
        public static Rect GetSafeArea (SimulatedDevice p_simulatedDevice = SimulatedDevice.None)
        {
            Rect r_safeArea = Screen.safeArea;

            if (p_simulatedDevice != SimulatedDevice.None || _SimulatedDevice != SimulatedDevice.None)
            {
                Rect l_normalizedSafeArea = GetNormalizedSafeAreaBySimulation (p_simulatedDevice != SimulatedDevice.None ? p_simulatedDevice : _SimulatedDevice);
                r_safeArea = new Rect (Screen.width * l_normalizedSafeArea.x, Screen.height * l_normalizedSafeArea.y, Screen.width * l_normalizedSafeArea.width, Screen.height * l_normalizedSafeArea.height);
            }

            return r_safeArea;
        }

        private static Rect GetNormalizedSafeAreaBySimulation (SimulatedDevice p_simulatedDevice)
        {
            Rect r_normalizedSafeArea = new Rect (0, 0, Screen.width, Screen.height);

            if (p_simulatedDevice != SimulatedDevice.None)
            {
                switch (p_simulatedDevice)
                {
                    case SimulatedDevice.iPhoneX:
                        if (Screen.height > Screen.width)
                            r_normalizedSafeArea = NSA_iPhoneX[0]; // Portrait;
                        else
                            r_normalizedSafeArea = NSA_iPhoneX[1]; // Landscape;
                        break;

                    case SimulatedDevice.iPhoneXsMax:
                        if (Screen.height > Screen.width)
                            r_normalizedSafeArea = NSA_iPhoneXsMax[0]; // Portrait;
                        else
                            r_normalizedSafeArea = NSA_iPhoneXsMax[1]; // Landscape;
                        break;

                    default:
                        break;
                }
            }

            return r_normalizedSafeArea;
        }

        private void ApplySafeArea (Rect p_safeArea)
        {
            m_lastSafeArea = p_safeArea;

            // Ignore x-axis?;
            if (!conformX)
            {
                p_safeArea.x = 0;
                p_safeArea.width = Screen.width;
            }

            // Ignore y-axis?;
            if (!conformY)
            {
                p_safeArea.y = 0;
                p_safeArea.height = Screen.height;
            }

            // Convert safe area rectangle from absolute pixels to normalized anchor coordinates;
            Vector2 l_anchorMin = p_safeArea.position;
            Vector2 l_anchorMax = p_safeArea.position + p_safeArea.size;

            l_anchorMin.x /= Screen.width;
            l_anchorMin.y /= Screen.height;
            l_anchorMax.x /= Screen.width;
            l_anchorMax.y /= Screen.height;

            m_panelRectTransform.anchorMin = new Vector2 (l_anchorMin.x, Mathf.Max (l_anchorMin.y,  minSafeAreaOffsetBottom));
            m_panelRectTransform.anchorMax = new Vector2 (l_anchorMax.x, Mathf.Min (l_anchorMax.y, 1- minSafeAreaOffsetTop));
        }
    }
}
