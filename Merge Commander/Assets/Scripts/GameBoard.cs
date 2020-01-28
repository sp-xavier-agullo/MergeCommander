using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBoard : MonoBehaviour
{

    [SerializeField] GameObject tilePrefab;

    [SerializeField] int numTilesX;
    [SerializeField] int numTilesY;

    [Header("Number of materials 1 to 7")]
    [SerializeField] int numMaterials;

    [SerializeField] float tileWidth;

    private LogicTile[,] logicTileArray;

    // Start is called before the first frame update
    void Start()
    {
        PopulateTiles();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            DestroyBoard();
            PopulateTiles();
        }
    }

    void PopulateTiles ()
    {
        LogicTile[,] logicTileArray = new LogicTile[numTilesX, numTilesY];
        //int tileColor = 1;

        // Populate board and tile array

        for (int i = 0; i < numTilesX; i++)
        {
            for (int j = 0; j < numTilesY; j++)
            {

                GameObject myTile = Instantiate(tilePrefab, transform);

                myTile.transform.localPosition = new Vector3((tileWidth / 2) + i * tileWidth, -(tileWidth / 2) + j * -tileWidth);

                // Tinting even tiles
                /*
                if (tileColor % 2 == 0)
                {
                    myTile.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 0.8f, 1);
                }

                tileColor = i + j;*/

                // Show material
                myTile.GetComponent<LogicTile>().showMaterial(numMaterials);

                // Filling logic tile array
                logicTileArray[i, j] = myTile.GetComponent<LogicTile>();


            }
        }

        //Positioning the whole board

        float offsetX = 0 - ((tileWidth * numTilesX) / 2);
        float offsetY = 0 + ((tileWidth * numTilesY) / 2);

        transform.localPosition = new Vector3(offsetX, offsetY);

    }

    void DestroyBoard()
    {
        int numTiles = transform.GetChildCount();

        for (int i = 0; i < numTiles; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
