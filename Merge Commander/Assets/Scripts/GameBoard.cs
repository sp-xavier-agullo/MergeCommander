using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{

    [SerializeField] GameObject tilePrefab;

    [SerializeField] int numTilesX;
    [SerializeField] int numTilesY;

    [SerializeField] float tileWidth;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numTilesX; i++)
        {
            for (int j = 0; j < numTilesY; j++)
            {

            GameObject myTile = Instantiate(tilePrefab,transform);
            myTile.transform.localPosition = new Vector3(i * tileWidth, j * tileWidth );
            }
        }

        float offsetX = 0-((tileWidth * numTilesX) / 2);
        float offsetY = 0-((tileWidth * numTilesY) / 2);

        transform.localPosition = new Vector3(offsetX, offsetY);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
