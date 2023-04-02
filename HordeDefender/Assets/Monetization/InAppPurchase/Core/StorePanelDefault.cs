using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class StorePanelDefault : MonoBehaviour
{

    [SerializeField]
    private GameObject UIProductPrefab;
    [SerializeField]
    private Transform ContentPanel;
    [SerializeField]
    private GameObject LoadingOverlay;

    private void Start()
    {
        LoadingOverlay.SetActive(false);
        StartCoroutine(CreateUI());
    }

    private IEnumerator CreateUI()
    {

        yield return new WaitUntil(()=>IAPManager.Instance.IsInitialized);

        List<Product> sortedProducts = IAPManager.Instance.GetAllProducts()
            .TakeWhile(item => !item.definition.id.Contains("sale"))
            .OrderBy(item => item.metadata.localizedPrice)
        .ToList();

        foreach (Product product in sortedProducts)
        {
            GameObject UIProductPrefabgameObject = Instantiate(UIProductPrefab);
            IIAPProductReciver uiProduct = UIProductPrefabgameObject.GetComponent<IIAPProductReciver>();
            uiProduct.OnPurchase += HandlePurchase;
            uiProduct?.SetIAPProduct(product);
            UIProductPrefabgameObject.transform.SetParent(ContentPanel.transform, false);
            yield return null;
        }
    }

    private void HandlePurchase(Product Product, Action OnPurchaseCompleted)
    {
        LoadingOverlay.SetActive(true);
        IAPManager.Instance.OnStorePurchaseCompleted += OnPurchaseCompleted;
        IAPManager.Instance.OnStorePurchaseCompleted += Instance_OnPurchaseTryCompleted;
        IAPManager.Instance.InitiatePurchase(Product);
    }

    private void Instance_OnPurchaseTryCompleted()
    {
        LoadingOverlay.SetActive(false);
    }
}
