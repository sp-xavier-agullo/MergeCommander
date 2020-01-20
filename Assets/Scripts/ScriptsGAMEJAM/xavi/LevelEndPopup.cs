using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelEndPopup : MonoBehaviour {

    public GameObject levelTitle;
    public GameObject winTitle;
    public GameObject loseTitle;

    public Text troopsLeftText;
    public Text rewardText;

    // Use this for initialization
    void Start () {

        levelTitle.GetComponent<TextMeshProUGUI>().text = "Level " + PlayerPrefs.GetInt("playerPreferencesCurrentLevel").ToString();
}

    // Lose or win?
    public void showWin(bool showWinBool)
    {

        if (showWinBool == true)
        {
            winTitle.SetActive(true);
            loseTitle.SetActive(false);
        }

        else if (showWinBool == false)
        {
            winTitle.SetActive(false);
            loseTitle.SetActive(true);
        }

    }

    // Change Scene
    public void OnClickedPlay()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void SetValues(int unitsLeft, int reward)
    {
        troopsLeftText.text = unitsLeft.ToString();
        rewardText.text = reward.ToString();
    }
}
