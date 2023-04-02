using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdTest : MonoBehaviour
{
    public Button interstatialbtn;
    public Button rewardedbtn;

    private void Update()
    {
        if (AdManager.Instance.CanShowInterstitialAd())
        {
            interstatialbtn.interactable = true;
        }
        else
        {
            interstatialbtn.interactable = false;
        }

        if (AdManager.Instance.CanShowRewardedAd())
        {
            rewardedbtn.interactable = true;
        }
        else
        {
            rewardedbtn.interactable = false;
        }
    }

    public void ShowInterstatial()
    {
        AdManager.Instance.ShowInterstatialAd();
    }
    public void ShowRewarded()
    {
        AdManager.Instance.ShowRewardedAd("Test");
    }
}
