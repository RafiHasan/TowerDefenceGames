using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json;

namespace UpgradeSystem
{
    struct GameData
    {
        public string Name;
        public string Description;
        public string Version;
        public string Url;
    }
    public class VersionUpgradeManager : MonoBehaviour
    {
        [Header("## UI References :")]
        [SerializeField] GameObject uiCanvas;
        [SerializeField] Button uiNotNowButton;
        [SerializeField] Button uiUpdateButton;
        [SerializeField] TextMeshProUGUI uiDescriptionText;

        [Space(20f)]
        [Header("## Settings :")]
        [SerializeField][TextArea(1, 5)] string jsonDataURL;

        static bool isAlreadyCheckedForUpdates = false;

        GameData latestGameData;

        void Start()
        {
            if (!isAlreadyCheckedForUpdates)
            {
                StopAllCoroutines();
                StartCoroutine(CheckForUpdates());
            }
        }

        
        IEnumerator CheckForUpdates()
        {
            UnityWebRequest request = UnityWebRequest.Get(jsonDataURL);
            
            request.disposeDownloadHandlerOnDispose = true;
            request.timeout = 60;

            yield return request.SendWebRequest();

            if (request.isDone)
            {
                isAlreadyCheckedForUpdates = true;

                if (request.result == UnityWebRequest.Result.Success)
                {
                    
                    Dictionary<string, object> dict = new Dictionary<string, object>();
                    Debug.Log(request.downloadHandler.text);
                    dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(request.downloadHandler.text);
                    Debug.Log(dict.Keys.Count);
                    foreach (string key in dict.Keys)
                    {
                        Debug.Log(key);
                    }

                    latestGameData = JsonUtility.FromJson<GameData>(dict[Application.platform.ToString()].ToString());
                    if (!string.IsNullOrEmpty(latestGameData.Version) && !Application.version.Equals(latestGameData.Version))
                    {
                        
                        // new update is available
                        uiDescriptionText.text = latestGameData.Description;
                        ShowPopup();
                    }
                }
            }

            request.Dispose();
        }

        void ShowPopup()
        {
            // Add buttons click listeners :
            uiNotNowButton.onClick.AddListener(() => {
                HidePopup();
            });

            uiUpdateButton.onClick.AddListener(() => {
                Application.OpenURL(latestGameData.Url);
                HidePopup();
            });

            uiCanvas.SetActive(true);
        }

        void HidePopup()
        {
            uiCanvas.SetActive(false);

            // Remove buttons click listeners :
            uiNotNowButton.onClick.RemoveAllListeners();
            uiUpdateButton.onClick.RemoveAllListeners();
        }


        void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}

