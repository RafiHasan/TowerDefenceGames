using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviour, IStoreListener
{

    public static IAPManager Instance;

    public static event Action<Product, bool> OnGlobalPurchaseActionCompleted;
    public event Action OnStorePurchaseCompleted;
    private IStoreController StoreController;
    private IExtensionProvider ExtensionProvider;

    public bool IsInitialized { get; private set; }

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            InitializeIAP();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }


    }

    private async void InitializeIAP()
    {
        IsInitialized = false;
        InitializationOptions options = new InitializationOptions()
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            .SetEnvironmentName("test");
#else
            .SetEnvironmentName("production");
#endif
        await UnityServices.InitializeAsync(options);
        ResourceRequest operation = Resources.LoadAsync<TextAsset>("IAPProductCatalog");
        operation.completed += HandleIAPCatalogLoaded;
    }

    private void HandleIAPCatalogLoaded(AsyncOperation Operation)
    {
        ResourceRequest request = Operation as ResourceRequest;

        Debug.Log($"Loaded Asset: {request.asset}");
        ProductCatalog catalog = JsonUtility.FromJson<ProductCatalog>((request.asset as TextAsset).text);
        Debug.Log($"Loaded catalog with {catalog.allProducts.Count} items");

        StandardPurchasingModule.Instance().useFakeStoreUIMode = FakeStoreUIMode.StandardUser;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        StandardPurchasingModule.Instance().useFakeStoreAlways = true;
#else
        StandardPurchasingModule.Instance().useFakeStoreAlways = false;
#endif

#if UNITY_ANDROID
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(
            StandardPurchasingModule.Instance(AppStore.GooglePlay)
        );
#elif UNITY_IOS
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(
            StandardPurchasingModule.Instance(AppStore.AppleAppStore)
        );
#else
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(
            StandardPurchasingModule.Instance(AppStore.NotSpecified)
        );
#endif
        foreach (ProductCatalogItem item in catalog.allProducts)
        {
            builder.AddProduct(item.id, item.type);
        }

        Debug.Log($"Initializing Unity IAP with {builder.products.Count} products");
        UnityPurchasing.Initialize(this, builder);
    }


    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        StoreController = controller;
        ExtensionProvider = extensions;
        Debug.Log($"Successfully Initialized Unity IAP. Store Controller has {StoreController.products.all.Length} products");
        StoreIconProvider.Initialize(StoreController.products);
        Debug.Log("List of products in store:");
        foreach (Product p in controller.products.all)
        {
            Debug.Log(p.definition.id + " - " + p.metadata.localizedPriceString);
        }

        IsInitialized = true;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError($"Error initializing IAP because of {error}." +
            $"\r\nShow a message to the player depending on the error.");
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError(message);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"Failed to purchase {product.definition.id} because {failureReason}");
        OnGlobalPurchaseActionCompleted?.Invoke(product, false);
        OnStorePurchaseCompleted?.Invoke();
        OnStorePurchaseCompleted = null;
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        Debug.Log($"Successfully purchased {purchaseEvent.purchasedProduct.definition.id}");
        OnGlobalPurchaseActionCompleted?.Invoke(purchaseEvent.purchasedProduct, true);
        OnStorePurchaseCompleted?.Invoke();
        OnStorePurchaseCompleted = null;
        return PurchaseProcessingResult.Complete;
    }

    public Product[] GetAllProducts()
    {
        return StoreController.products.all;
    }

    public Product GetProduct(string productId)
    {
        return StoreController.products.WithID(productId);
    }

    public void InitiatePurchase(Product product)
    {
        if (IsOwned(product))
            return;

        StoreController.InitiatePurchase(product);
    }

    public static bool IsOwned(Product product)
    {
        if (product.definition.type == ProductType.NonConsumable && product.hasReceipt)
            return true;

        return false;
    }
}
