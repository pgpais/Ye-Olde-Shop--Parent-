using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechUI : MonoBehaviour
{
    [SerializeField] SmallTechUI SmallUnlockableUIPrefab;
    [Space]
    [SerializeField] Transform requirementsUI;
    [SerializeField] TMPro.TMP_Text text;

    private Unlockable unlockable;

    public void InitUI(Unlockable unlockable)
    {
        this.unlockable = unlockable;

        text.text = unlockable.UnlockableName;

        foreach (var requirement in unlockable.Requirements)
        {
            Instantiate(SmallUnlockableUIPrefab, requirementsUI).InitUI(requirement);
        }

        unlockable.UnlockableUpdated.AddListener(UpdateUI);
    }

    void UpdateUI(Unlockable unlockable)
    {
        text.text = this.unlockable.UnlockableName;
        if (unlockable.Unlocked)
        {
            text.color = Color.green;
        }
        Debug.Log($"{unlockable.UnlockableName} was updated!");
    }
}
