using Google.Play.Review;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InAppReviewManager : SingletonNoCreate<InAppReviewManager>
{
    // Create instance of ReviewManager
    private ReviewManager _reviewManager;

    public void ShowReviewPanel()
    {
        StartCoroutine(StartTheReview());
    }

    public IEnumerator StartTheReview()
    {

        _reviewManager = new ReviewManager();
        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            yield break;
        }

        var _playReviewInfo = requestFlowOperation.GetResult();
        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            yield break;
        }
    }
}