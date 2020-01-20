using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoftPopup : MonoBehaviour {

    //Game object reference in memory
    public GameObject softPopupPrefab;

    //Number of popups currently in scene
    private GameObject[] _currentSoftPopups;

    // Test function popup
    void Update()
    {

        if (Input.GetKeyUp(KeyCode.S))
        {
            printSoftPopup("This is a test for soft pop-up");
        }

    }

    public void printSoftPopup(string inputText)
    {
        printSoftPopup(inputText, 1.0f, 2.5f, null);
    }
    
    public void printSoftPopup(string inputText, Vector3 worldPosition)
    {
        printSoftPopup(inputText, 1.0f, 2.5f, worldPosition);
    }
    
    public void printSoftPopup(string inputText, float fadeDuration, float stayDuration, Vector3? customWorldPosition)
    {
            //Assign arguments
            string _inputText = inputText;

            // Offsetting all previous instances upwards to make room for the new one
            _currentSoftPopups = GameObject.FindGameObjectsWithTag("SoftPopup");

            if (_currentSoftPopups != null)
            {
                foreach (GameObject instantiatedPopupPrefab in _currentSoftPopups)
                {
                    instantiatedPopupPrefab.transform.position += new Vector3(0, 90, 0);
                }
            }

            //Instantiating a new prefab and destroying it after 4 seconds 
            GameObject softPopupInstance = Instantiate(softPopupPrefab);
            softPopupInstance.transform.SetParent(transform, false);
        
            if (customWorldPosition != null)
            {
                softPopupInstance.transform.position = customWorldPosition.GetValueOrDefault();
            }
        
            //Assign Text to input text
            Text softPopupText = softPopupInstance.GetComponentInChildren<Text>();

            if (softPopupText != null)
            {
                softPopupText.text = _inputText;
            }

            StartCoroutine(fadeInfadeOut(softPopupInstance, fadeDuration, stayDuration));

    }

    IEnumerator fadeInfadeOut(GameObject myInstance, float duration, float stayDuration)
    {
        CanvasGroup instanceCanvas = myInstance.GetComponent<CanvasGroup>();
        instanceCanvas.alpha = 0;
        LeanTween.alphaCanvas(instanceCanvas, 1f, duration).setEase(LeanTweenType.easeOutQuart);
        yield return new WaitForSeconds(stayDuration);
        LeanTween.alphaCanvas(instanceCanvas, 0f, duration).setEase(LeanTweenType.easeOutCubic);
        yield return new WaitForSeconds(2f);
        Destroy(myInstance);
    }
}
