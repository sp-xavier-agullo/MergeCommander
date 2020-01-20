using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CheatsPopup : MonoBehaviour {

	public GameObject CheatsScreen;
	public GameObject CheatsButton;
	public GameObject SoftPopupCanvas;
	public GameObject PopupCanvas;
	//public GameObject PopupManager;


	// Showing Hiding Cheats panel
	public void showCheats(){
		CheatsScreen.SetActive (true);
	}

	public void hideCheats(){
		CheatsScreen.SetActive (false);
	}

	///////////////////////////////////////////
	// Assign cheats
	///////////////////////////////////////////

	// CHEAT 1 - RESET GAME
	public void cheat1 (){

        SceneManager.LoadScene("Loading");
        PlayerPrefs.DeleteAll();
	}

	// CHEAT 2 - LEVEL UP
	public void cheat2 (){

        int currentLevel = PlayerPrefs.GetInt("playerPreferencesCurrentLevel");

        currentLevel += 1;

        if (currentLevel > 10)
        {
            currentLevel = 10;
        }

        PlayerPrefs.SetInt("playerPreferencesCurrentLevel", currentLevel);

        SceneManager.LoadScene("Loading");
	}

	// CHEAT 3 - LEVEL DOWN
	public void cheat3 (){

        int currentLevel = PlayerPrefs.GetInt("playerPreferencesCurrentLevel");

        currentLevel -= 1;

        if (currentLevel < 1)
        {
            currentLevel = 1;
        }

        PlayerPrefs.SetInt("playerPreferencesCurrentLevel", currentLevel);

        SceneManager.LoadScene("Loading");
    }

	// CHEAT 4 - LEVEL 3
	public void cheat4 (){

        int currentLevel = 3;
        PlayerPrefs.SetInt("playerPreferencesCurrentLevel", currentLevel);
        SceneManager.LoadScene("Loading");

    }

	// CHEAT 5 - LEVEL 5
	public void cheat5 (){

        int currentLevel = 5;
        PlayerPrefs.SetInt("playerPreferencesCurrentLevel", currentLevel);
        SceneManager.LoadScene("Loading");

    }

    // CHEAT 6 - LEVEL 7
    public void cheat6 (){

        int currentLevel = 7;
        PlayerPrefs.SetInt("playerPreferencesCurrentLevel", currentLevel);
        SceneManager.LoadScene("Loading");

    }

	// CHEAT 7 -
	public void cheat7 (){

        int currentLevel = 9;
        PlayerPrefs.SetInt("playerPreferencesCurrentLevel", currentLevel);
        SceneManager.LoadScene("Loading");

    }

	// CHEAT 8 - POPUP 1
	public void cheat8 (){

		PopupCanvas.GetComponent<PopupManager>().showLevelEndPopup (false);
		SoftPopupCanvas.GetComponent<SoftPopup>().printSoftPopup("LEVEL END POPUP");
		hideCheats ();

	}

	// CHEAT 9 - POPUP 2
	public void cheat9 (){

        PopupCanvas.GetComponent<PopupManager>().showOptionsPopup();
        SoftPopupCanvas.GetComponent<SoftPopup>().printSoftPopup("OPTIONS POPUP");
        hideCheats();

	}

	// CHEAT 10 - POPUP 3
	public void cheat10 (){

		SoftPopupCanvas.GetComponent<SoftPopup>().printSoftPopup("CHEAT 10");
		hideCheats ();

	}

	// CHEAT 11 - POPUP 4
	public void cheat11 (){

		SoftPopupCanvas.GetComponent<SoftPopup>().printSoftPopup("CHEAT 11");
		hideCheats ();

	}

	// CHEAT 12 - POPUP 5
	public void cheat12 (){

		SoftPopupCanvas.GetComponent<SoftPopup>().printSoftPopup("CHEAT 12");
		hideCheats ();

	}

	// CHEAT 13 - POPUP 6
	public void cheat13 (){

		SoftPopupCanvas.GetComponent<SoftPopup>().printSoftPopup("CHEAT 13");
		hideCheats ();

	}

	// CHEAT 14 - POPUP 7
	public void cheat14 (){

		SoftPopupCanvas.GetComponent<SoftPopup>().printSoftPopup("CHEAT 14");
		hideCheats ();

	}

}
