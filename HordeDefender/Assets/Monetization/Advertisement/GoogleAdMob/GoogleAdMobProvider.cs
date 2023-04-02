using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class GoogleAdMobProvider : MonoBehaviour,IAdProvider
{
#if UNITY_ANDROID
    private string _adUnitIdinterstitial = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
    private string _adUnitIdinterstitial = "ca-app-pub-3940256099942544/4411468910";
#else
    private string _adUnitIdinterstitial = "unused";
#endif

    private InterstitialAd interstitialAd;


#if UNITY_ANDROID
    private string _adUnitIdrewarded = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
    private string _adUnitIdrewarded = "ca-app-pub-3940256099942544/1712485313";
#else
    private string _adUnitIdrewarded = "unused";
#endif

    private RewardedAd rewardedAd;

    public void Start()
    {
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            LoadInterstitialAd();
            LoadRewardedAd();
        });
    }

    public void LoadInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest.Builder().AddKeyword("unity-admob-sample").Build();

        // send the request to load the ad.
        InterstitialAd.Load(_adUnitIdinterstitial, adRequest,(InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +"with error : " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "+ ad.GetResponseInfo());

                interstitialAd = ad;
            });
    }

    public void LoadRewardedAd()
    {
        // Clean up the old ad before loading a new one.
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        Debug.Log("Loading the rewarded ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest.Builder().Build();

        // send the request to load the ad.
        RewardedAd.Load(_adUnitIdrewarded, adRequest,(RewardedAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +"with error : " + error);
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : "+ ad.GetResponseInfo());

                rewardedAd = ad;
            });
    }

    public void ShowInterstitialAd()
    {

        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            Debug.Log("Showing interstitial ad.");
            interstitialAd.Show();
            interstitialAd.OnAdFullScreenContentClosed += InterstitialAd_OnAdFullScreenContentClosed;
        }
        else
        {
            LoadInterstitialAd();
            Debug.LogError("Interstitial ad is not ready yet.");
        }
    }

    private void InterstitialAd_OnAdFullScreenContentClosed()
    {
        LoadInterstitialAd();
    }

    public void ShowRewardedAd(string rewardId)
    {
        const string rewardMsg =
            "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                AdManager.ProvideReward(rewardId);
                // TODO: Reward the user.
                Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
                
            });
            rewardedAd.OnAdFullScreenContentClosed += RewardedAd_OnAdFullScreenContentClosed;
        }
        
    }

    private void RewardedAd_OnAdFullScreenContentClosed()
    {
        LoadRewardedAd();
    }

    public bool CanShowInterstitialAd()
    {
        if(interstitialAd==null || !interstitialAd.CanShowAd())
        {
            return false;
        }

        return true;
    }

    public bool CanShowRewardedAd()
    {
        if (rewardedAd == null || !rewardedAd.CanShowAd())
        {
            return false;
        }

        return true;
    }
}

