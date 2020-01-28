using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicTile : MonoBehaviour
{

    enum TileTerrain { Ground, Water };
    TileTerrain myTileTerrain;

    private void Start()
    {
    }

    public void showMaterial (int limitMaterials)
    {
        int numMaterials = transform.childCount;

        for (int i = 0; i < numMaterials; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        int randomMaterialID = Random.Range(0, limitMaterials);
        transform.GetChild(randomMaterialID).gameObject.SetActive(true);

    }

}
