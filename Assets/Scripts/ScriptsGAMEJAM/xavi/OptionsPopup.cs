using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsPopup : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    // Change Scene
    public void OnClickedPlay()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
