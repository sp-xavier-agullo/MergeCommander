using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode : MonoBehaviour {

    public int nodeNum;
    public enum NodeType {Locked,Complete,Current};

    public GameObject lockedFolder;
    public GameObject currentFolder;
    public GameObject clearFolder;

    // Use this for initialization
    void Start () {
		
	}

    public void setNode(NodeType type)
    {
        // COMPLETE NODE
        if (type == NodeType.Complete)
        {
            lockedFolder.SetActive(false);
            currentFolder.SetActive(false);
            clearFolder.SetActive(true);
        }

        // CURRENT NODE
        if (type == NodeType.Current)
        {
            lockedFolder.SetActive(false);
            currentFolder.SetActive(true);
            clearFolder.SetActive(false);
        }

        // LOCKED NODE
        if (type == NodeType.Locked)
        {
            lockedFolder.SetActive(true);
            currentFolder.SetActive(false);
            clearFolder.SetActive(false);
        }

    }

}
