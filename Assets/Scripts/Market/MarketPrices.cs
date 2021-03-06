using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;

public class MarketPrices : SerializedMonoBehaviour
{
    public static MarketPrices instance;
    public static string referenceName = "marketPrices";

    public static UnityEvent GotMarketPrices = new UnityEvent();

    public bool hasPrices => costModifierToday != null;

    [SerializeField] List<Dictionary<string, int>> costModifierToday;

    [SerializeField] bool testUpload;
    [SerializeField] int howManyDays;

    private int indexOfActiveCosts = 0;
    private DateTime curDay;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        FirebaseCommunicator.LoggedIn.AddListener(GetPricesForToday);
    }

    private void Start()
    {
        Debug.Log(DateTime.Now.ToString("yyyyMMdd"));
        Debug.Log(DateTime.ParseExact(DateTime.Now.ToString("yyyyMMdd"), "yyyyMMdd", null));
    }

    private void Update()
    {
        var today = DateTime.Now;

        if (testUpload)
        {
            testUpload = false;
            PopulateMarketPrices(today);
        }

        int newIndex = today.Hour / 3;
        if (newIndex != indexOfActiveCosts)
        {
            indexOfActiveCosts = today.Hour / 3;
            GotMarketPrices.Invoke();
        }

        if (today.Day > curDay.Day)
        {
            GetPricesForToday();
        }
    }

    private void PopulateMarketPrices(DateTime today)
    {
        for (int i = 0; i < howManyDays; i++)
        {
            var daySpan = new TimeSpan(i, 0, 0, 0, 0);
            CreateMarketPricesForDay(today + daySpan);
        }
    }

    public void GetPricesForToday()
    {
        FirebaseCommunicator.instance.GetObject(new string[] { referenceName, DateTime.Now.ToString("yyyyMMdd") }, (task) =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("smth went wrong. " + task.Exception.ToString());
                return;
            }

            if (task.IsCompleted)
            {
                Debug.Log("yey got marketPrices");
                string json = task.Result.GetRawJsonValue();
                Debug.Log("Before dictionary");

                if (string.IsNullOrEmpty(json))
                {
                    Debug.Log("no marketPrices, populating marketPrices");
                    PopulateMarketPrices(DateTime.Now);
                }
                costModifierToday = JsonConvert.DeserializeObject<List<Dictionary<string, int>>>(json);
                Debug.Log("after dictionary");
                curDay = DateTime.ParseExact(task.Result.Key, "yyyyMMdd", null);
                GotMarketPrices.Invoke();
            }
        });
    }

    internal int GetCostModifierForItem(Item item)
    {
        return GetCostModifierForItem(item.ItemNameKey);
    }

    public int GetCostModifierForItem(string itemName)
    {
        if (costModifierToday[indexOfActiveCosts].ContainsKey(itemName))
            return costModifierToday[indexOfActiveCosts][itemName];
        return 0;
    }

    public static List<Dictionary<string, int>> GenerateMarketPrices()
    {
        List<Dictionary<string, int>> newDayPrices = new List<Dictionary<string, int>>();

        System.Random ran = new System.Random();

        for (var i = 0; i < 8; i++)
        {
            Dictionary<string, int> itemPrices = new Dictionary<string, int>();

            foreach (var item in ItemManager.instance.itemsData.Items)
            {
                itemPrices.Add(item.ItemNameKey, ran.Next(item.MinModifier, item.MaxModifier));
            }
            newDayPrices.Add(itemPrices);
        }

        return newDayPrices;
    }

    public static void CreateMarketPricesForDay(DateTime date)
    {
        var marketPrices = GenerateMarketPrices();
        string dateString = date.ToString("yyyyMMdd");
        string json = JsonConvert.SerializeObject(marketPrices);
        Debug.Log(dateString + ":" + json);
        FirebaseCommunicator.instance.SendObject(json, new string[] { referenceName, dateString }, (task) =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("smth went wrong at reference." + task.Exception.ToString());
                return;
            }

            if (task.IsCompleted)
            {
                Debug.Log("yey updated marketPrices");
            }
        });
    }
}
