using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public delegate void PurchaseEvent(Product Model, Action OnComplete);
public interface IIAPProductReciver
{
    public void SetIAPProduct(Product product);
    public event PurchaseEvent OnPurchase;
}
