using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClosePopup : MonoBehaviour {

	//referencing 'close button'
	public	Button _closeButton;

	void Start () {
		StartCoroutine(assignCloseButton());
	}

	IEnumerator assignCloseButton() {

		yield return new WaitForSeconds(0.5f);

		//Reference to PopupCanvas and PopupManager Script
		GameObject popupCanvasInstance = GameObject.Find ("PopupCanvas");
		PopupManager popupManagerScript = (PopupManager)popupCanvasInstance.GetComponent (typeof(PopupManager)); 

		//Assigning script to close button
		_closeButton.onClick.AddListener (popupManagerScript.closePopup);
	}
}
