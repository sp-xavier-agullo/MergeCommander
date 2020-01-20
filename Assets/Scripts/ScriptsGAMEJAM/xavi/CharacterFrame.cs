using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GJ18;

public class CharacterFrame : MonoBehaviour {

    [Header("Config Units")]
    public int unitIndex;
    public CoreConfig coreConfigUnits;

    [Header("Elements")]
    public GameObject lockedFrame;
    public GameObject unlockedFrame;
    public GameObject selectedFrame;

    public Image unlockedPortrait;
    public Image lockedPortrait;

    // Use this for initialization
    void Start () {

        unlockedPortrait.sprite = coreConfigUnits.UnitConfigs[unitIndex].portraitButtonLocked;
        lockedPortrait.sprite = coreConfigUnits.UnitConfigs[unitIndex].portraitButton;

    }

    public void lockFrame ()
    {

    }

    public void unselectFrame()
    {
            lockedFrame.SetActive(false);
            unlockedFrame.SetActive(true);
            selectedFrame.SetActive(false);
    }

    public void selectFrame()
    {
        lockedFrame.SetActive(false);
        unlockedFrame.SetActive(false);
        selectedFrame.SetActive(true);
    }




}
