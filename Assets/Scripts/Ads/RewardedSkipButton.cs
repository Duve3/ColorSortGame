using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class RewardedSkipButton : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] private Button _showAdButton;
    [SerializeField] private string _androidAdUnitId = "Rewarded_Android_v2";
    [SerializeField] private string _iOSAdUnitId = "Rewarded_iOS_v2";
    [SerializeField] private GameHandler GameHandle;
    [SerializeField] private Sprite YesImage;
    [SerializeField] private Sprite LoadingImage;
    string _adUnitId = null;

    void Awake()
    {
        #if UNITY_IOS
                _adUnitId = _iOSAdUnitId;
        #elif UNITY_ANDROID
            _adUnitId = _androidAdUnitId;
        #endif

        ChangeButtonState(false);
        LoadAd();
    }

    private void ChangeButtonState(bool active)
    {
        if (active)
        {
            _showAdButton.interactable = true;
            _showAdButton.gameObject.GetComponent<Image>().sprite = YesImage;
        } else
        {
            _showAdButton.interactable = false;
            _showAdButton.gameObject.GetComponent<Image>().sprite = LoadingImage;
        }
    }

    // Call this public method when you want to get an ad ready to show.
    public void LoadAd()
    {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        Debug.Log("Loading Ad: " + _adUnitId);
        Advertisement.Load(_adUnitId, this);
    }

    // If the ad successfully loads, add a listener to the button and enable it:
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Ad Loaded: " + adUnitId);

        if (adUnitId.Equals(_adUnitId))
        {
            // Configure the button to call the ShowAd() method when clicked:
            _showAdButton.onClick.AddListener(ShowAd);
            // Enable the button for users to click:
            ChangeButtonState(true);
        }
    }

    // Implement a method to execute when the user clicks the button:
    public void ShowAd()
    {
        // Disable the button:
        ChangeButtonState(false);
        // Then show the ad:
        Advertisement.Show(_adUnitId, this);
    }

    // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("Unity Ads Rewarded Ad Completed");
            Debug.Log("NEXTLEVEL COMPLETE");
            // Grant a reward.

            GameHandle.NextLevel();

            // cleanup
            _showAdButton.onClick.RemoveListener(ShowAd);

            // ensures we ALWAYS have an ad ready! 
            LoadAd();

            
        }
    }

    // Implement Load and Show Listener error callbacks:
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }

    void OnDestroy()
    {
        // Clean up the button listeners:
        _showAdButton.onClick.RemoveAllListeners();
    }
}
