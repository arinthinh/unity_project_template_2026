using UnityEngine;

namespace Amanotes.Orchestra
{
    /// <summary>
    /// MonoBehaviour that handles Unity lifecycle events for KVibrations
    /// Automatically creates itself when needed and persists across scenes
    /// </summary>
    [AddComponentMenu("")] 
    public class KVibrationsComponent : MonoBehaviour
    {
        private static KVibrationsComponent _instance;

        void Awake()
        {
            // Ensure singleton pattern
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Handle application focus changes - stop vibrations when app loses focus
        /// </summary>
        void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                // Stop any playing vibration when app loses focus
                KVibrations.Stop();
            }
        }

        /// <summary>
        /// Handle application pause - stop vibrations when app is paused
        /// </summary>
        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                // Stop any playing vibration when app is paused
                KVibrations.Stop();
            }
        }

        /// <summary>
        /// Stop vibrations when component is destroyed
        /// </summary>
        void OnDestroy()
        {
            if (_instance == this)
            {
                KVibrations.Stop();
                _instance = null;
            }
        }

        /// <summary>
        /// Auto-create instance when application starts
        /// This ensures the manager exists and KVibrations is initialized at game start
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void AutoCreate()
        {
            // Create the singleton instance
            if (_instance == null)
            {
                // Try to find existing instance in scene first
                _instance = FindObjectOfType<KVibrationsComponent>();

                // If none exists, create one
                if (_instance == null)
                {
                    GameObject go = new GameObject("[KVibrations]");
                    _instance = go.AddComponent<KVibrationsComponent>();
                    DontDestroyOnLoad(go);
                }
            }
        }
    }
}
