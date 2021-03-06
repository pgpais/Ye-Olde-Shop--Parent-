using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour, IComparable<ItemUI>
{
    public Item Item => item;

    [SerializeField] TMP_Text itemNameText;
    // [SerializeField] TMP_Text itemDescriptionText;
    [SerializeField] TMP_Text itemQuantityText;
    [SerializeField] GameObject itemQuantityLabel;
    // [SerializeField] TMP_Text itemValueText;
    [SerializeField] Image itemImage;
    [SerializeField] Button startProcessingItemButton;
    [SerializeField] Toggle processOneItemToggle;
    [SerializeField] Toggle processTenItemToggle;
    [SerializeField] Toggle processAllItemToggle;
    // [SerializeField] Button sellItemButton;

    private bool available = false;

    private Item item;

    private void Awake()
    {

    }

    public void Init(Item item, int itemQuantity)
    {
        this.item = item;

        itemNameText.text = item.ItemName;
        // itemDescriptionText.text = item.Description;
        // this.itemValueText.transform.parent.gameObject.SetActive(false);

        available = true;
        MakeAvailable(itemQuantity);

        if (item.ItemSprite != null)
        {
            itemImage.sprite = item.ItemSprite;
        }
    }

    public void Init(Item item)
    {
        this.item = item;

        itemNameText.text = item.ItemName;
        // itemDescriptionText.text = item.Description;
        // this.itemValueText.transform.parent.gameObject.SetActive(false);

        available = false;
        MakeUnavailable();

        if (item.ItemSprite != null)
        {
            itemImage.sprite = item.ItemSprite;
        }
    }

    private void UpdateUI(Item item, int quantity)
    {
        if (item.ItemNameKey == this.item.ItemNameKey)
            itemQuantityText.text = quantity.ToString();
    }

    private void OnEnable()
    {
        startProcessingItemButton.onClick.AddListener(StartProcessing);
    }

    private void OnDisable()
    {
        startProcessingItemButton.onClick.RemoveListener(StartProcessing);
    }

    private void ShowStartProcessScreen()
    {

        ProcessingManager.instance.ShowNewProcessMenu(itemNameText.text, 0, ItemManager.instance.itemQuantity[itemNameText.text]);
    }

    private void ShowSellScreen()
    {
        ItemManager.instance.ShowSellGameMenu(item.ItemNameKey, 0, ItemManager.instance.itemQuantity[item.ItemNameKey]);
    }

    public void MakeUnavailable()
    {
        if (item.ProcessResult != null && !item.ProcessResult.Unlocked)
        {
            Item.ItemUnlocked.RemoveListener(ShowProcessingButtons);
        }

        itemQuantityText.gameObject.SetActive(false);
        itemQuantityLabel.gameObject.SetActive(false);
        // sellItemButton.gameObject.SetActive(false);
        HideProcessingButtons(item.ProcessResult);
        gameObject.SetActive(false);

    }

    public void MakeAvailable(int itemQuantity)
    {
        gameObject.SetActive(true);
        itemQuantityText.gameObject.SetActive(true);
        itemQuantityLabel.gameObject.SetActive(true);
        // sellItemButton.gameObject.SetActive(true);
        if (item.ProcessResult != null)
        {
            if (!item.ProcessResult.Unlocked)
            {
                HideProcessingButtons(item.ProcessResult);

                Item.ItemUnlocked.AddListener(ShowProcessingButtons);
            }
            else
            {
                ShowProcessingButtons(item);
            }
        }
        else
        {
            HideProcessingButtons(item.ProcessResult);
        }

        itemQuantityText.text = itemQuantity.ToString();

        // TODO: #13 Move item event listeners to parent UI script
        ItemManager.ItemUpdated.AddListener(UpdateUI);

        // sellItemButton.onClick.AddListener(ShowSellScreen);

        bool canItemBeProcessed = item.Type != Item.ItemType.Processed && item.ProcessResult != null;

        if (!canItemBeProcessed)
        {
            HideProcessingButtons(item);
        }
        else
        {
            ShowProcessingButtons(item);
        }

        bool isRawItem = item.Type != Item.ItemType.Processed && item.Type != Item.ItemType.Valuable;

    }

    void ShowProcessingButtons(Item item)
    {
        if (item == this.item)
        {
            startProcessingItemButton.gameObject.SetActive(true);
            processOneItemToggle.gameObject.SetActive(true);
            processTenItemToggle.gameObject.SetActive(true);
            processAllItemToggle.gameObject.SetActive(true);
        }
    }

    void HideProcessingButtons(Item item)
    {
        if (item == this.item)
        {
            startProcessingItemButton.gameObject.SetActive(false);
            processOneItemToggle.gameObject.SetActive(false);
            processTenItemToggle.gameObject.SetActive(false);
            processAllItemToggle.gameObject.SetActive(false);
        }
    }

    private void StartProcessing()
    {
        int amount = 0;

        if (processOneItemToggle.isOn)
        {
            amount = 1;
        }
        else if (processTenItemToggle.isOn)
        {
            amount = 10;
        }
        else if (processAllItemToggle.isOn)
        {
            amount = ItemManager.instance.itemQuantity[item.ItemNameKey];
        }

        ProcessingManager.instance.StartProcessing(item.ItemNameKey, amount);
    }

    public int CompareTo(ItemUI other)
    {
        return item.CompareTo(other.item);
    }
}
