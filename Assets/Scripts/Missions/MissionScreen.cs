using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionScreen : MonoBehaviour
{

    [Header("Gatherables Screen")]
    [SerializeField] AbundantGatherableScreen abundantGatherablesScreen;
    [SerializeField] Button abundantGatherablesScreenButton;
    [SerializeField] Image abundantGatherableImage;
    [SerializeField] TMPro.TMP_Text itemNameText;

    [Header("Difficulty Screen")]
    [SerializeField] Toggle easyToggle;
    [SerializeField] Toggle mediumToggle;
    [SerializeField] Toggle hardToggle;

    private void Awake()
    {
        if (MissionManager.instance.GotAbundantGatherable)
        {
            SetAbundantGatherable(MissionManager.instance.AbundantGatherable);
        }
        else
        {
            MissionManager.OnGotAbundantGatherable.AddListener(SetAbundantGatherable);
        }

        if (MissionManager.instance.GotDifficulty)
        {
            SetDifficulty(MissionManager.instance.Difficulty);
        }
        else
        {
            MissionManager.OnGotDifficulty.AddListener(SetDifficulty);
        }
    }

    private void SetDifficulty(int difficulty)
    {
        if (difficulty <= 10)
        {
            easyToggle.isOn = true;
            mediumToggle.isOn = false;
            hardToggle.isOn = false;
        }
        else if (difficulty >= 11 && difficulty <= 20)
        {
            easyToggle.isOn = false;
            mediumToggle.isOn = true;
            hardToggle.isOn = false;
        }
        else
        {
            easyToggle.isOn = false;
            mediumToggle.isOn = false;
            hardToggle.isOn = true;
        }
    }

    private void Start()
    {
        easyToggle.onValueChanged.AddListener(OnEasyToggle);
        mediumToggle.onValueChanged.AddListener(OnMediumToggle);
        hardToggle.onValueChanged.AddListener(OnHardToggle);

        abundantGatherablesScreenButton.onClick.AddListener(ShowAbundantGatherableScreen);
        abundantGatherablesScreen.OnAbundantGatherableSelected.AddListener(OnAbundantGatherableSelected);
    }

    private void OnHardToggle(bool isOn)
    {
        if (isOn)
        {
            MissionManager.instance.SetDifficulty(MissionDifficulty.Hard);
        }
    }
    private void OnMediumToggle(bool isOn)
    {
        if (isOn)
        {
            MissionManager.instance.SetDifficulty(MissionDifficulty.Medium);
        }
    }

    private void OnEasyToggle(bool isOn)
    {
        if (isOn)
        {
            MissionManager.instance.SetDifficulty(MissionDifficulty.Easy);
        }
    }

    void SetAbundantGatherable(Item item)
    {
        abundantGatherableImage.sprite = item.ItemSprite;
        itemNameText.text = item.ItemName;
    }

    void ShowAbundantGatherableScreen()
    {
        abundantGatherablesScreen.Show();
    }

    void HideAbundantGatherableScreen()
    {
        abundantGatherablesScreen.Hide();
    }

    void OnAbundantGatherableSelected(Item item)
    {
        SetAbundantGatherable(item);
        MissionManager.instance.SetAbundantGatherable(item);

        HideAbundantGatherableScreen();
    }
}
