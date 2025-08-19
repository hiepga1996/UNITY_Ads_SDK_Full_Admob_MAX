using System.Collections;
using UnityEngine;
using System;
using UnityEngine.Purchasing;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;
using Ultility;

[Serializable]
public class ConsumableItem  {
    public string Name;
    public string Id;
    public string desc;
    public float price;
    public Text priceText;
    [SerializeField]
    public UnityEvent _callSuccess;
}
[Serializable]
public class NonConsumableItem
{
    public string Name;
    public string Id;
    public string desc;
    public float price;
    public Text priceText;
    [SerializeField]
    public UnityEvent _callSuccess;
}
[Serializable]
public class SubscriptionItem
{
    public string Name;
    public string Id;
    public string desc;
    public float price;
    public Text priceText;
    public int timeDuration;// in Days
    [SerializeField]
    public UnityEvent _callSuccess;
}

public class ShopScript : MonoBehaviour, IDetailedStoreListener
{
    public static ShopScript instance;

    IStoreController m_StoreContoller;

    public List<ConsumableItem> list_Consumable;
    public List<NonConsumableItem> list_Nonconsumable;
    public List<SubscriptionItem> list_Subscript;

    public Data data;
    public Payload payload;
    public PayloadData payloadData;

    public bool isConsum = false;
    public bool isNonConsum = false;
    public bool isSub = false;
    public bool isRestore = false;

    [Header("Restore Panel")]
    //[SerializeField] Button restoreClickeButton;
    //[SerializeField] GameObject restoreClicked;
    //[SerializeField] Text pack1;
    //[SerializeField] Text pack2;
    //[SerializeField] Text pack3;

    [Header("Button")]
    [SerializeField] Button button1;

    private void Start()
    {
        //restoreClickeButton.onClick.AddListener(() => { OnButtonRestore(); });
        //if (PlayerPrefs.GetInt("restore_button", 0) != 0)
        //    restoreClickeButton.gameObject.SetActive(false);
        StartCoroutine("InitIAP");
    }

    void Awake()
    {
        instance = this;
    }

    void InitButton()
    {
        //button1.onClick.AddListener(() => Consumable_Btn_Pressed(common.superBundle));
    }

    IEnumerator InitIAP()
    {
        yield return new WaitForSeconds(0.5f);
        SetupBuilder();
        yield return new WaitForEndOfFrame();
        InitButton();
    }

    #region setup and initialize
    void SetupBuilder()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        for (var i = 0; i < list_Consumable.Count; i++)
        {
            builder.AddProduct(list_Consumable[i].Id, ProductType.Consumable);
        }

        for (var i = 0; i < list_Nonconsumable.Count; i++)
        {
            builder.AddProduct(list_Nonconsumable[i].Id, ProductType.NonConsumable);
        }

        for (var i = 0; i < list_Subscript.Count; i++)
        {
            builder.AddProduct(list_Subscript[i].Id, ProductType.Subscription);
        }
        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        print("Success");
        m_StoreContoller = controller;
        for (var i = 0; i < list_Consumable.Count; i++)
        {
            var product = m_StoreContoller.products.WithID(list_Consumable[i].Id);
           
            if (product != null)
                list_Consumable[i].priceText.text = product.metadata.localizedPriceString;
        }

        for (var i = 0; i < list_Nonconsumable.Count; i++)
        {
            var product = m_StoreContoller.products.WithID(list_Nonconsumable[i].Id);

            if (product != null)
                list_Nonconsumable[i].priceText.text = product.metadata.localizedPriceString;
        }
    }
    #endregion


    #region button clicks 
    public void Consumable_Btn_Pressed(String Id)
    {
        //Advertisements.Instance.isBannerClickedAOA = true;
        isConsum = true;
        isNonConsum = false;
        isSub = false;
        m_StoreContoller.InitiatePurchase(Id);
    }

    public void NonConsumable_Btn_Pressed(String Id)
    {
        //Advertisements.Instance.isBannerClickedAOA = true;
        isConsum = false;
        isNonConsum = true;
        isSub = false;
        m_StoreContoller.InitiatePurchase(Id);
    }

    public void Subscription_Btn_Pressed(String Id)
    {
        //Advertisements.Instance.isBannerClickedAOA = true;
        isConsum = false;
        isNonConsum = false;
        isSub = true;
        m_StoreContoller.InitiatePurchase(Id);
    }

    public void OnButtonRestore()
    {
        isRestore = true;
        for (int i = 0; i < list_Nonconsumable.Count; i++)
        {
            if (CodelessIAPStoreListener.Instance.StoreController.products.WithID(list_Nonconsumable[i].Id).hasReceipt)
            {
                //restoreClicked.SetActive(true);
                //list_Nonconsumable[i]._callSuccess.Invoke();
                //if (list_Nonconsumable[i].Id == "tricky_remove_ads")
                //    pack1.text = "Successfully!";
                //if (list_Nonconsumable[i].Id == "tricky_unlock_level")
                //    pack3.text = "Successfully!";

                //PlayerPrefs.SetInt("restore_button", 1);
                //restoreClickeButton.gameObject.SetActive(false);
                //isRestore = false;
            }
        }
    }
    public void OnFailedRestore()
    {
        for (int i = 0; i < list_Nonconsumable.Count; i++)
        {
            //restoreClicked.SetActive(true);
            //if (list_Nonconsumable[i].Id == "market_no_ads_1")
            //    pack1.text = "Failed!";
            //if (list_Nonconsumable[i].Id == "market_no_ads_2")
            //    pack2.text = "Failed!";
        }
    }

    public void OffRestore()
    {
        //restoreClicked.SetActive(false);
    }

    public void onTouchRestorePurchase(IStoreController controller, IExtensionProvider extensions)
    {
        
    }

    #endregion

    #region main
    //processing purchase
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        //Retrive the purchased product
        var product = purchaseEvent.purchasedProduct;

        print("Purchase Complete" + product.definition.id);

        //ShopManager.instance.OnSuccess();
        CheckItemPurchaseSuccess(purchaseEvent);

        return PurchaseProcessingResult.Complete;
    }
    #endregion

    void CheckItemPurchaseSuccess(PurchaseEventArgs purchaseEvent)
    {
        var product = purchaseEvent.purchasedProduct;
        var id = product.definition.id;
       
        // Check Consumable
        for (var i = 0; i < list_Consumable.Count; i++)
        {
            // check tay cac goi
            if(id == list_Consumable[i].Id)
            {
                StartCoroutine(WaitForPurchaseDoneConsume(list_Consumable[i]));
                return;
            }            
        }

        // Check Non Consumable
        for (var i = 0; i < list_Nonconsumable.Count; i++)
        {
            // check tay cac goi
            if (id == list_Nonconsumable[i].Id)
            {
                StartCoroutine(WaitForPurchaseDoneNonConsume(list_Nonconsumable[i]));
                return;
            }
        }

        //Check SubScript
        for (var i = 0; i < list_Subscript.Count; i++)
        {
            // check tay cac goi
            if (id == list_Subscript[i].Id)
            {
                StartCoroutine(WaitForPurchaseDoneSub(list_Subscript[i]));

                return;
            }
        }
    }

    public void onRestorePurchaseSuccess()
    {

        for (var i = 0; i < list_Nonconsumable.Count; i++)
        {
            //list_Nonconsumable[i].purchased.SetActive(false);
        }

        for (var i = 0; i < list_Subscript.Count; i++)
        {
           //list_Subscript[i].purchased.SetActive(false);
        }
    }

    public void onRestorePurchaseFail()
    {
        //ShopManager.instance.OnFail();
    }
    public void purchaseNonConsumable(string id)
    {
        switch (id)
        {
            //case "market_no_ads_1":
            //    ShopManager.instance.RemoveAds();
            //    break;

            //case "market_no_ads_2":
            //    ShopManager.instance.RemoveAds2();
            //    break;
        }
    }

    public void purchaseSubScript(string id)
    {
        //if (id == "gym_click_sub_week")
        //{
        //    common.num_ticket += 3;
        //    PlayerPrefs.SetInt(common.Save_Ticket, common.num_ticket);
        //    purchaseNoAds();
        //    purchaseRandomSkin();
        //    list_Subscript[0].purchased.SetActive(true);
        //}
        //else if (id == "gym_click_sub_month")
        //{
        //    common.num_ticket += 3;
        //    PlayerPrefs.SetInt(common.Save_Ticket, common.num_ticket);
        //    purchaseNoAds();
        //    purchaseRandomSkin();
        //    list_Subscript[1].purchased.SetActive(true);
        //}
    }

    void CheckNonConsumable() {

        if (m_StoreContoller!=null)
        {
            for(var i = 0; i < list_Nonconsumable.Count; i++)
            {
                var product = m_StoreContoller.products.WithID(list_Nonconsumable[i].Id);
                if (product != null)
                {
                    if (product.hasReceipt)//purchased
                    {
                        //UIController.instance.ToggleSuccess();
                        purchaseNonConsumable(list_Nonconsumable[i].Id);
                    }
                }
            }
        }
    }

    void CheckSubscription() {
        if (m_StoreContoller == null)
            return;

        for (var i = 0; i < list_Subscript.Count; i++)
        {
            var subProduct = m_StoreContoller.products.WithID(list_Subscript[i].Id);
            //string json = JsonUtility.ToJson(subProduct);
            //MessageBox.instance.showMessage(json);
            if (subProduct != null)
            {
                try
                {
                    if (subProduct.hasReceipt)
                    {
                        var subManager = new SubscriptionManager(subProduct, null);
                        var info = subManager.getSubscriptionInfo();

                        if (info.isSubscribed() == Result.True)
                        {
                            print("We are subscribed");
                            list_Subscript[i]._callSuccess.Invoke();
                        }
                        else
                        {
                            print("Un subscribed");
                            //list_Subscript[i].purchased.SetActive(false);
                        }

                    }
                    else
                    {
                        print("receipt not found !!");
                    }
                }
                catch (Exception)
                {

                    print("It only work for Google store, app store, amazon store, you are using fake store!!");
                }
            }
            else
            {
                print("product not found !!");
            }
        }

            
    }

    #region error handeling
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        print("failed" + error);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        print("initialize failed" + error + message);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        isConsum = false;
        isNonConsum = false;
        isSub = false;
        //IAPManager.instance.OnIAPCancel();
        print("purchase failed" + failureReason);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        isConsum = false;
        isNonConsum = false;
        isSub = false;
        //IAPManager.instance.OnIAPCancel();
        print("purchase failed" + failureDescription);
    }
    #endregion

    IEnumerator WaitForPurchaseDoneNonConsume(NonConsumableItem item)
    {
        yield return new WaitForSecondsRealtime(0.5f);
        if(isNonConsum)
        {
            if (item._callSuccess != null)
            {
                item._callSuccess.Invoke();
                //IAPPurchasing.instance.OnComplete();
            }
        }
    }

    IEnumerator WaitForPurchaseDoneConsume(ConsumableItem item)
    {
        yield return new WaitForSecondsRealtime(0.5f);
        if (isConsum)
        {
            if(item._callSuccess != null)
            {
                item._callSuccess.Invoke();
            }
        }
    }

    IEnumerator WaitForPurchaseDoneSub(SubscriptionItem item)
    {
        //Advertisements.Instance.isBannerClickedAOA = true;
        yield return new WaitForSecondsRealtime(0.5f);
        if (isSub)
        {
            if (item._callSuccess != null)
            {
                item._callSuccess.Invoke();
            }
        }
    }
    #region extra 

    #endregion

}


[Serializable]
public class SkuDetails
{
    public string productId;
    public string type;
    public string title;
    public string name;
    public string iconUrl;
    public string description;
    public string price;
    public long price_amount_micros;
    public string price_currency_code;
    public string skuDetailsToken;
}

[Serializable]
public class PayloadData
{
    public string orderId;
    public string packageName;
    public string productId;
    public long purchaseTime;
    public int purchaseState;
    public string purchaseToken;
    public int quantity;
    public bool acknowledged;
}

[Serializable]
public class Payload
{
    public string json;
    public string signature;
    public List<SkuDetails> skuDetails;
    public PayloadData payloadData;
}

[Serializable]
public class Data
{
    public string Payload;
    public string Store;
    public string TransactionID;
}