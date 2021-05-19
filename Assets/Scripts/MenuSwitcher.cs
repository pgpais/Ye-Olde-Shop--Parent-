using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

public class MenuSwitcher : SerializedMonoBehaviour
{
    public static MenuSwitcher instance;

    [SerializeField] GameObject canvas;

    [SerializeField] TMP_Dropdown menuDropdown;

    [SerializeField] string defaultScreenName = "Processing";
    [SerializeField] Dictionary<string, GameObject> menus;

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

        canvas.SetActive(false);

        FirebaseCommunicator.LoggedIn.AddListener(() =>
        {
            canvas.SetActive(true);

            // TODO: #12 Figure out another flow for starting the game
        });
        menuDropdown.onValueChanged.AddListener(SwitchToMenu);
    }

    private void Start()
    {
        PopulateDropdownAndSetDefault();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
            return;
        }
    }

    void PopulateDropdownAndSetDefault()
    {
        menuDropdown.ClearOptions();

        var keys = new List<string>(menus.Keys);
        menuDropdown.AddOptions(keys);

        menuDropdown.value = keys.FindIndex((key) => key == defaultScreenName);
    }

    void SwitchToMenu(int menuIndex)
    {
        var keys = new List<string>(menus.Keys);

        for (var i = 0; i < keys.Count; i++)
        {
            if (i == menuIndex)
            {
                menus[keys[i]].SetActive(true);
            }
            else
            {
                menus[keys[i]].SetActive(false);
            }
        }
    }
}
