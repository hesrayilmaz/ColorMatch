using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Transform levelsParent;
    [SerializeField] private Sprite unlockedButton;
    [SerializeField] private Sprite lockedButton;

    // Start is called before the first frame update
    void Start()
    {
        //ResetSettings();
        //UnlockAllLevels();

        for (int i = 0; i < levelsParent.childCount; i++)
        {
            Transform level = levelsParent.GetChild(i);

            if (PlayerPrefs.GetString(("Level" + (i+1)), "Locked") == "Unlocked")
            {
                level.GetComponent<Image>().sprite = unlockedButton;
                level.GetComponent<Button>().interactable = true;
            }
            else
            {
                level.GetComponent<Image>().sprite = lockedButton;
                level.GetComponent<Button>().interactable = false;
            }
        }
    }

    void ResetSettings()
    {
        PlayerPrefs.SetInt("ActiveLevelIndex", 1);
        PlayerPrefs.SetString("Level1", "Unlocked");

        for (int i = 1; i < levelsParent.childCount; i++)
            PlayerPrefs.SetString(("Level" + (i + 1)), "Locked");
    }

    void UnlockAllLevels()
    {
        for (int i = 0; i < levelsParent.childCount; i++)
            PlayerPrefs.SetString(("Level" + (i + 1)), "Unlocked");
    }
}