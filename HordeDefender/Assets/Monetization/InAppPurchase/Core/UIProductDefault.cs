using System;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class UIProductDefault : MonoBehaviour, IIAPProductReciver
{
    [SerializeField]
    private string _productId="";
    [SerializeField]
    private TextMeshProUGUI NameText;
    [SerializeField]
    private TextMeshProUGUI DescriptionText;
    [SerializeField]
    private Image Icon;
    [SerializeField]
    private TextMeshProUGUI PriceText;
    [SerializeField]
    private Button PurchaseButton;

    
    

    private Product Model;

    public event PurchaseEvent OnPurchase;

    private void Awake()
    {
        SetIAPProductWithId(_productId);
    }

    public void Purchase()
    {
        PurchaseButton.enabled = false;
        OnPurchase?.Invoke(Model, HandlePurchaseComplete);
    }

    private void HandlePurchase(Product Product, Action OnPurchaseCompleted)
    {
        IAPManager.Instance.OnStorePurchaseCompleted += OnPurchaseCompleted;
        IAPManager.Instance.InitiatePurchase(Product);
    }

    private void HandlePurchaseComplete()
    {
        PurchaseButton.enabled = true;
        if(IAPManager.IsOwned(Model))
        {
            PriceText.text = "Purchased";
            PurchaseButton.enabled = false;
        }
    }

    public void SetIAPProductWithId(string productID)
    {
        Product product = IAPManager.Instance.GetProduct(productID);

        if (product == null)
            return;

        OnPurchase += HandlePurchase;
        SetIAPProduct(product);
    }
    public void SetIAPProduct(Product product)
    {
        Model = product;
        NameText?.SetText(product.metadata.localizedTitle);
        DescriptionText?.SetText(product.metadata.localizedDescription);
        PriceText?.SetText($"{product.metadata.localizedPriceString} " +
            $"{product.metadata.isoCurrencyCode}");

        if(Icon!=null)
        {
            Texture2D texture = StoreIconProvider.GetIcon(product.definition.id);
            if (texture != null)
            {
                Sprite sprite = Sprite.Create(texture,
                    new Rect(0, 0, texture.width, texture.height),
                    Vector2.one / 2f
                );

                Icon.sprite = sprite;
            }
            else
            {
                Debug.LogError($"No Sprite found for {product.definition.id}!");
            }
        }

        PurchaseButton.onClick.AddListener(Purchase);

    }
}