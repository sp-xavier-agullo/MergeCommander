using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour {

	#region Fields
	//Game object reference in memory
	public GameObject levelEndPopupPrefab;
    public GameObject optionsPopupPrefab;

    public GameObject darknessLayerPrefab;

	private GameObject _darknessLayerInstance;


	//List of Popups
	List<GameObject> activePopups = new List<GameObject>();

	// TODO find a better way to do this
	public bool blocked = false;  // only applies to events

	#endregion


	void Start()
	{

	}

    // Test function popup
	void Update() {

        if (Input.GetKeyUp(KeyCode.P))
        {
            showLevelEndPopup(true);
        }

	}


	#region Public methods


	/* SHOW LEVEL END POPUP
	 * ------------------------------------------------------------------*/

	public void showLevelEndPopup (bool isVictory) {

		darkenBackground ();

        GameObject popupInstance = showAPopupFall (levelEndPopupPrefab);
        popupInstance.GetComponent<LevelEndPopup>().showWin(isVictory);

        // Hide previous popups
        hideDisplayPopupQueue ();

    }

    /* SHOW OPTIONS POPUP
 * ------------------------------------------------------------------*/

    public void showOptionsPopup()
    {

        darkenBackground();
        showAPopupFall(optionsPopupPrefab);

        // Hide previous popups
        hideDisplayPopupQueue();
    }


    /* CLOSE POPUP
	 * ------------------------------------------------------------------*/

    public void closePopup () {

		// Select the top pop-up and get object reference
		int _listLength = activePopups.Count -1;

		if (_listLength >= 0) {
			GameObject topPopup = activePopups [_listLength];
		
			// Remove from list
			activePopups.Remove(activePopups[_listLength]);

			//animation
			float timeDelay = 0.2f;

			LeanTween.scale (topPopup, new Vector3 (0.8f, 0.8f, 0.8f), timeDelay).setEase (LeanTweenType.easeOutSine);

			CanvasGroup myCanvas = topPopup.GetComponent<CanvasGroup> ();
			myCanvas.alpha = 1f;

			LeanTween.alphaCanvas (myCanvas, 0.2f, timeDelay);

			// Destroy the popup according to animation time
			//float destroyDelay = topPopupAnim.GetCurrentAnimatorStateInfo(0).length;
			Destroy (topPopup, timeDelay);

			hideDisplayPopupQueue ();
			lightenBackground ();

		}


	}

	/* CLOSE POPUP
	 * ------------------------------------------------------------------*/

	public void closeAllPopups () {

		// If no popups active, stop function
		if (activePopups.Count == 0) {
			return;
		}

		//Clear all other popups
		for (int i = 0; i < activePopups.Count; i++) {
			Destroy (activePopups [i]);
		}
		activePopups.Clear ();
		lightenBackground ();

	}

    #endregion

    #region Helper methods

    /* SHOW POPUP STANDARD
	 * ------------------------------------------------------------------*/

    GameObject showAPopup (GameObject PopupPrefabType) {

		GameObject _popupPrefabType = PopupPrefabType;

		//Instantiating a new prefab
		GameObject popupInstance = Instantiate (_popupPrefabType);
		popupInstance.transform.SetParent (transform);
		popupInstance.transform.localPosition = new Vector3 (0, 200, 0);

		//animation
		float timeDelay = 1f;

		CanvasGroup myCanvas = popupInstance.GetComponent<CanvasGroup> ();
		myCanvas.alpha = 0.7f;

		popupInstance.SetActive(true);

		LeanTween.alphaCanvas (myCanvas, 1, timeDelay/2);
		popupInstance.transform.localScale = new Vector3 (0.8f,0.8f,0.8f);
		LeanTween.scale (popupInstance, new Vector3 (1f, 1f, 1f), 1f).setEase (LeanTweenType.easeOutElastic);


		//Adding to active list of popups
		activePopups.Add (popupInstance);

		return (popupInstance);
	}

    /* SHOW POPUP FALL
	 * ------------------------------------------------------------------*/

    GameObject showAPopupFall (GameObject PopupPrefabType)
    {

        GameObject _popupPrefabType = PopupPrefabType;

        //Instantiating a new prefab
        GameObject popupInstance = Instantiate(_popupPrefabType);
        popupInstance.transform.SetParent(transform);
        popupInstance.transform.localPosition = new Vector3(0, 1000, 0);

        //animation
        float timeDelay = 1f;

        CanvasGroup myCanvas = popupInstance.GetComponent<CanvasGroup>();
        myCanvas.alpha = 0.8f;

        popupInstance.SetActive(true);

        LeanTween.alphaCanvas(myCanvas, 1, timeDelay / 2);

        LeanTween.moveLocalY(popupInstance, 200, 0.5f).setEase(LeanTweenType.easeOutBack);
        LeanTween.scale(popupInstance, new Vector3(1.05f, 1.05f, 1.05f), 0.5f).setEase(LeanTweenType.linear);
        //LeanTween.moveLocalY(popupInstance, 260, 1f).setEase(LeanTweenType.easeOutElastic);

        //Adding to active list of popups
        activePopups.Add(popupInstance);

        return (popupInstance);
    }


    public void darkenBackground () {

		int numPopups = activePopups.Count;

		if (numPopups == 0) {

			//Instantiating a new prefab
			_darknessLayerInstance = Instantiate (darknessLayerPrefab);
			_darknessLayerInstance.transform.SetParent (transform);
			_darknessLayerInstance.transform.localPosition = new Vector3(0f,0f,0f);
			_darknessLayerInstance.SetActive (true);

			CanvasGroup myCanvasGroup = _darknessLayerInstance.GetComponent<CanvasGroup> ();

			//Making darkness sprite cover all screen
			RectTransform rect = _darknessLayerInstance.GetComponent<RectTransform> ();
			rect.sizeDelta = new Vector2(300, 300);


			myCanvasGroup.alpha = 0f;

			LeanTween.alphaCanvas (myCanvasGroup, 1, 0.5f);

		}
	}
		

	public void lightenBackground () {

		int numPopups = activePopups.Count;
		//AudioSource audioSrc0 = GameObject.Find("Manager").GetComponent<AudioSource>();

		if (numPopups == 0) {

			//Transition Mask
			//transitionProgress = 0f;

			CanvasGroup myCanvasGroup = _darknessLayerInstance.GetComponent<CanvasGroup> ();

			myCanvasGroup.alpha = 1f;

			LeanTween.alphaCanvas (myCanvasGroup, 0, 0.5f);

			Destroy (_darknessLayerInstance, 0.5f);
		}
			
	}

	private void hideDisplayPopupQueue () {

		// HIDE ALL POPUPS NOT ON TOP
		for (int i = 0; i < activePopups.Count; i++) {
			activePopups [i].SetActive (false);
		}
		if (activePopups.Count > 0) {
			activePopups [activePopups.Count - 1].SetActive (true);
		}

	}
	#endregion
}
