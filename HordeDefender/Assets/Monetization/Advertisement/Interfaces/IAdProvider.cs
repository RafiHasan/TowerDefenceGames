using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAdProvider 
{
    public bool CanShowInterstitialAd();
    public bool CanShowRewardedAd();
    public void ShowInterstitialAd();
    public void ShowRewardedAd(string rewardId);
}
