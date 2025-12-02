using UnityEngine;
using UnityEngine.Advertisements;

public class BannerAd : MonoBehaviour
{
    [SerializeField] private string _androidAdUnitId = "Banner_Android";
    [SerializeField] private string _iOSAdUnitId = "Banner_iOS";
    [SerializeField] private BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER;

    private string _adUnitId;

    private void Start()
    {
#if UNITY_IOS
            _adUnitId = _iOSAdUnitId;
#elif UNITY_ANDROID
        _adUnitId = _androidAdUnitId;
#endif

        Advertisement.Banner.SetPosition(_bannerPosition);
        LoadBanner();
    }

    public void LoadBanner()
    {
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };

        Advertisement.Banner.Load(_adUnitId, options);
    }

    private void OnBannerLoaded()
    {
        Debug.Log("[BannerAd] Banner loaded successfully.");
        ShowBanner();
    }

    private void OnBannerError(string message)
    {
        Debug.LogError("[BannerAd] Banner failed to load: " + message);
    }

    public void ShowBanner()
    {
        BannerOptions options = new BannerOptions
        {
            clickCallback = OnBannerClicked,
            hideCallback = OnBannerHidden,
            showCallback = OnBannerShown
        };

        Advertisement.Banner.Show(_adUnitId, options);
        Debug.Log("[BannerAd] Banner shown.");
    }

    public void HideBanner()
    {
        Advertisement.Banner.Hide();
        Debug.Log("[BannerAd] Banner hidden.");
    }

    private void OnBannerClicked() { }
    private void OnBannerShown() { }
    private void OnBannerHidden() { }
}
