using UnityEngine;
using UnityEngine.Advertisements;

public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
{
    [SerializeField] private string _androidGameId = "5944721";
    [SerializeField] private string _iOSGameId = "5944720";
    [SerializeField] private bool _testMode = true;

    private string _gameId;

    private void Awake()
    {
        // Initialize Ads once
        InitializeAds();
    }

    public void InitializeAds()
    {
#if UNITY_IOS
            _gameId = _iOSGameId;
#elif UNITY_ANDROID
        _gameId = _androidGameId;
#elif UNITY_EDITOR
            _gameId = _androidGameId; // For testing in Editor
#endif

        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(_gameId, _testMode, this);
            Debug.Log("[AdsInitializer] Unity Ads initializing...");
        }
        else
        {
            Debug.Log("[AdsInitializer] Unity Ads already initialized or not supported.");
        }
    }

    public void OnInitializationComplete()
    {
        Debug.Log("[AdsInitializer] Unity Ads initialization complete.");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogError($"[AdsInitializer] Initialization Failed: {error} - {message}");
    }
}
