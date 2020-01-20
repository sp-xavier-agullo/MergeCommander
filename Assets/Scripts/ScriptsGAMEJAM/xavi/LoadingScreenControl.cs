using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreenControl : MonoBehaviour {

	public Text percentageNum;
	public Text percentageText;
	public GameObject buttonGo;

	AsyncOperation asyncOp;

	public void Start () {

		//Set bar to 0%
		percentageNum.text = "0%";

		buttonGo.SetActive(false);

		//Start loading coroutine
		StartCoroutine (LoadingScreen());
	}

	IEnumerator LoadingScreen() {

		//Start loading
		asyncOp = SceneManager.LoadSceneAsync (1);
		asyncOp.allowSceneActivation = false;

		while (asyncOp.isDone == false) {

			//Update percentage number
			int percentageLoading = (int)((100f / 0.9f) * asyncOp.progress); //async progress maxs at 0.9
			percentageNum.text = (percentageLoading.ToString ()) + "%";

			//Next scene if loadign complete and at least 3 seconds
			//if (asyncOp.progress == 0.9f && secondsSpanned > 3f) {
			if (asyncOp.progress >= 0.9f) {
				percentageNum.gameObject.SetActive(false);
				percentageText.gameObject.SetActive(false);
				buttonGo.SetActive(true);
				//asyncOp.allowSceneActivation = true;
			}
			yield return null;
		}
	}

    public void OnClickedPlay()
        {
            SceneManager.LoadScene("MainMenu");
        }

}
