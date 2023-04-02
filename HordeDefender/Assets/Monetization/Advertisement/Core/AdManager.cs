using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance;

    public static event Action<string> OnRewardedAdCompleted;

    public IAdProvider adProvider;

    private void Awake()
    {
        if(Instance==null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        adProvider = GetComponent<IAdProvider>();
    }

    public void ShowInterstatialAd()
    {
        adProvider?.ShowInterstitialAd();
    }

    public void ShowRewardedAd(string rewardId)
    {
        adProvider?.ShowRewardedAd(rewardId);
    }

    public bool CanShowInterstitialAd()
    {
        if(adProvider!=null)
        {
            return adProvider.CanShowInterstitialAd();
        }
        return false;
    }
    public bool CanShowRewardedAd()
    {
        if (adProvider != null)
        {
            return adProvider.CanShowRewardedAd();
        }
        return false;
    }

    public static void ProvideReward(string rewardId)
    {
        Debug.Log("Rewarded With "+ rewardId);
        OnRewardedAdCompleted?.Invoke(rewardId);
    }

}
