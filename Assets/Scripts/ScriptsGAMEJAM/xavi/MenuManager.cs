using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GJ18;
using TMPro;

public class MenuManager : MonoBehaviour {

    //public Button leftTab;
    //public Button rightTab;

    public GameObject leftSection;
    public GameObject rightSection;

    public int unitSelected;
    public int currentLevel;

    [Header("Config Units")]
    public CoreConfig coreConfigUnits;
    public List<int> unitsMapping;

    [Header("UnitsMenu")]

    
    public GameObject topBarName;
    public Text characterFrameName;
    public Text powerText;
    public Text attacksText;
    public Text strengthText;
    public Text defenseText;
    public Text rangedText;
    public Text fliesText;
    public Text descText;
    public GameObject bigPortrait;
    public GameObject item1Sprite;
    public GameObject item2Sprite;
    public GameObject item3Sprite;

    [Header("UnitsButtons")]
    public List<GameObject> unitsButtons;

    private const string PlayerPrefsCurrentLevel = "playerPreferencesCurrentLevel";
    
    public static int GetCurrentLevel()
    {
        return PlayerPrefs.GetInt(PlayerPrefsCurrentLevel);
    }

    public static void SetCurrentLevel(int level)
    {
        PlayerPrefs.SetInt(PlayerPrefsCurrentLevel, level);
    }
    
    public static void SetNextLevel()
    {
        const int NumLevels = 10;
        SetCurrentLevel(Mathf.Min(GetCurrentLevel() + 1, NumLevels));
    }
    
    void Awake()
    {
        if (PlayerPrefs.HasKey(PlayerPrefsCurrentLevel))
        {
            currentLevel = GetCurrentLevel();

        } else

        {
            SetCurrentLevel(1);
            currentLevel = GetCurrentLevel(); 

            Debug.Log("Player prefs current level set to " + GetCurrentLevel());
        }
    }

    void Start ()
    {
        selectUnit(unitSelected);
    }


    /* UPDATE DISPLAY INFO
     * ------------------------------------------------------------------*/

    public void updateUnitDisplay (int unitNum)
    {
        Debug.Log("this is working: " + unitNum.ToString());

        int mappedUnit = unitsMapping[unitNum];

        var unit = coreConfigUnits.UnitConfigs[mappedUnit];
        
        topBarName.GetComponent<TextMeshProUGUI>().text = coreConfigUnits.UnitConfigs[mappedUnit].unitShortName;
        characterFrameName.text = coreConfigUnits.UnitConfigs[mappedUnit].unitLongName;
        powerText.text = coreConfigUnits.UnitConfigs[mappedUnit].unitPower.ToString();
        attacksText.text = coreConfigUnits.UnitConfigs[mappedUnit].unitNumAttacks.ToString();
        strengthText.text = ((int)(Math.Round(unit.attackChance * 100.0f))).ToString();
        defenseText.text = ((int)(Math.Round(unit.defenseChance * 100.0f))).ToString();
        rangedText.text = coreConfigUnits.UnitConfigs[mappedUnit].unitRanged;
        fliesText.text = coreConfigUnits.UnitConfigs[mappedUnit].unitFlies;
        descText.text = coreConfigUnits.UnitConfigs[mappedUnit].unitDesc;
        bigPortrait.GetComponent<SpriteRenderer>().sprite = coreConfigUnits.UnitConfigs[mappedUnit].portraitBig;
        item1Sprite.GetComponent<SpriteRenderer>().sprite = coreConfigUnits.UnitConfigs[mappedUnit].recipeItem1;
        item2Sprite.GetComponent<SpriteRenderer>().sprite = coreConfigUnits.UnitConfigs[mappedUnit].recipeItem2;
        item3Sprite.GetComponent<SpriteRenderer>().sprite = coreConfigUnits.UnitConfigs[mappedUnit].recipeItem3;

        bigPortrait.transform.localScale = new Vector3 (55f,55f,55f);
        LeanTween.scale (bigPortrait, new Vector3 (65f, 65f, 65f), 0.5f).setEase (LeanTweenType.easeOutElastic);

    }

    /* UPDATE BUTTONS
     * ------------------------------------------------------------------*/
     public void updateButtons ()
    {


    }


    /* BROWSE BUTTONS
     * ------------------------------------------------------------------*/
    public void switchUnit (int numOffset)
    {
        unitSelected += numOffset;

        if (unitSelected < 0)
        {
            unitSelected = unitsMapping.Count-1;
        }
        else if (unitSelected > unitsMapping.Count - 1)
        {
            unitSelected = 0;
        }

        selectUnit(unitSelected);
    }

    public void selectUnit (int unitIndex)
    {

        unitSelected = unitIndex;

        updateUnitDisplay(unitSelected);

        for (int i = 0; i < unitsButtons.Count; i++)
        {
            unitsButtons[i].GetComponent<CharacterFrame>().unselectFrame();
        }

        unitsButtons[unitIndex].GetComponent<CharacterFrame>().selectFrame();

    }


    /* TABS LOGIC
     * ------------------------------------------------------------------*/

    // Select Section
    public void SelectSection(string sectionTab)
    {
        // Left Tab
        if (sectionTab == "left")
        {
            leftSection.SetActive(true);
            rightSection.SetActive(false);
        }

        // Right Tab
        else if (sectionTab == "right")
        {
            leftSection.SetActive(false);
            rightSection.SetActive(true);

            //Update Units
            selectUnit(unitSelected);

        }
    }
	
}
