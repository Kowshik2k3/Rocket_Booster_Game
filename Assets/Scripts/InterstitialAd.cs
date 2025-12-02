using UnityEngine;
using UnityEngine.Advertisements;
using System;

public class InterstitialAd : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] private string _androidAdUnitId = "Interstitial_Android";
    [SerializeField] private string _iOSAdUnitId = "Interstitial_iOS";

    private string _adUnitId;
    private bool _adReady = false;
    private Action _onAdComplete;

    private void Awake()
    {
        _adUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)
                    ? _iOSAdUnitId
                    : _androidAdUnitId;

        LoadAd(); // Load first ad at start
    }

    public void LoadAd()
    {
        if (!Advertisement.isInitialized)
        {
            Debug.LogWarning("[InterstitialAd] Unity Ads not initialized yet.");
            return;
        }

        Debug.Log("[InterstitialAd] Loading interstitial ad...");
        Advertisement.Load(_adUnitId, this);
    }

    // For Unity Button OnClick
    public void ShowAdButton()
    {
        ShowAd(); // Calls ShowAd() with default null callback
    }

    public void ShowAd(Action onComplete = null)
    {
        _onAdComplete = onComplete;

        if (_adReady)
        {
            Debug.Log("[InterstitialAd] Showing interstitial ad.");
            Advertisement.Show(_adUnitId, this);
            _adReady = false;
        }
        else
        {
            Debug.Log("[InterstitialAd] Interstitial not ready, reloading...");
            LoadAd();
            _onAdComplete?.Invoke(); // fallback immediately
        }
    }

    #region IUnityAdsLoadListener
    public void OnUnityAdsAdLoaded(string placementId)
    {
        if (placementId == _adUnitId)
        {
            _adReady = true;
            Debug.Log("[InterstitialAd] Interstitial loaded and ready.");
        }
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        if (placementId == _adUnitId)
        {
            _adReady = false;
            Debug.LogError($"[InterstitialAd] Failed to load interstitial: {error} - {message}");
        }
    }
    #endregion

    #region IUnityAdsShowListener
    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogError($"[InterstitialAd] Failed to show interstitial: {error} - {message}");
        LoadAd();
        _onAdComplete?.Invoke();
        _onAdComplete = null;
    }

    public void OnUnityAdsShowStart(string placementId) { }
    public void OnUnityAdsShowClick(string placementId) { }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log("[InterstitialAd] Interstitial ad completed.");
        LoadAd(); // automatically load next ad
        _onAdComplete?.Invoke();
        _onAdComplete = null;
    }
    #endregion
}
