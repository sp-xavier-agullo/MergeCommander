using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodesManager : MonoBehaviour {

    public List<GameObject> nodeList;
    public MenuManager myMenuManager;

    public Text levelDisplayText;
    public GameObject buttonLevel;
    public GameObject endingText;

    public GameObject playerCharacter;

    // Use this for initialization
    void Start () {
        updateNodes();
        updateLevelDisplayText();

        updatePlayerPosition();
    }
	
    public void updateNodes ()
    {
        for (int i = 0; i < nodeList.Count; i++)
        {
            // Past node (complete)
            if (nodeList[i].GetComponent<MapNode>().nodeNum < myMenuManager.currentLevel)
            {
                nodeList[i].GetComponent<MapNode>().setNode(MapNode.NodeType.Complete);
            }

            // Current node (current)
            if (nodeList[i].GetComponent<MapNode>().nodeNum == myMenuManager.currentLevel)
            {
                nodeList[i].GetComponent<MapNode>().setNode(MapNode.NodeType.Current);
            }

            // Future node (locked)
            if (nodeList[i].GetComponent<MapNode>().nodeNum > myMenuManager.currentLevel)
            {
                nodeList[i].GetComponent<MapNode>().setNode(MapNode.NodeType.Locked);
            }
        }

        Debug.Log("Level is: " + myMenuManager.currentLevel + " nodes have been updated");
    }

   public void updateLevelDisplayText()
    {
        levelDisplayText.text = "Level " + myMenuManager.currentLevel.ToString();

        if (myMenuManager.currentLevel>9)
        {
                levelDisplayText.text = "Well done";
                buttonLevel.SetActive(false);
                endingText.SetActive(true);
        }
    }

    public void updatePlayerPosition ()
    {
        int currentLevel = myMenuManager.currentLevel;
        playerCharacter.transform.localPosition = nodeList[currentLevel-1].transform.localPosition;

        LeanTween.moveLocal(playerCharacter, nodeList[currentLevel].transform.localPosition, 2f).setEase(LeanTweenType.easeInOutQuad);
    }

}
