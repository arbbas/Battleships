using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Script to automate placing tiles. Can configure for desired board size in future game modes.
 * Default is 10x10
 * @reference - basic game code is adapted from Udemy tutorial 'Battleships 3D', available at: https://www.udemy.com/course/unity-game-tutorial-battleships-3d/ 

 * @author Enigma Studios
 * @version 10-03-2022
 */
public class PhysicalPlayfield : MonoBehaviour
{
    // Toggle populating board with tiles
    public bool fill;

    // Connection to tile prefab
    public GameObject tPrefab;

    // Stores prefabs
    List<GameObject> tileList = new List<GameObject>();

    // List containing all the tile information for tiles in this playfield.
    List<TileInformation> tileInfoList = new List<TileInformation>();

    /// <summary>
    /// On starting, the playfield gets given information about what tiles it contains
    /// </summary>
    private void Start()
    {
        tileList.Clear();
        tileInfoList.Clear();

        foreach (Transform item in transform)
        {
            if (item != transform)
            {
                tileList.Add(item.gameObject);
            }
        }

        foreach (GameObject item in tileList)
        {
            if (item != transform)
            {
                tileInfoList.Add(item.GetComponent<TileInformation>());
            }
        }
    }

    /// <summary>
    /// Method to check if a tile is in this playfield or not.
    /// Used to prevent players placing ships on the opponents playfield.
    /// </summary>
    /// <param name="info"></param>
    /// <returns>true if tile is contained, otherwise false</returns>
    public bool RequestTile(TileInformation info)
    {
        return (tileInfoList.Contains(info));
    }


    // Fill list when fill bool is activated
    void OnDrawGizmos()
    {
        // Stops running to protect the grid
        if (tPrefab != null && fill)
        {
            // Deletes existing tiles to stop overflow
            for (int i = 0; i < tileList.Count; i++)
            {
                DestroyImmediate(tileList[i]);
            }

            // Empty the list
            tileList.Clear();

            // Create tile grid 10x10
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    // Logic for placing tile in correct place.
                    Vector3 pos =
                        new Vector3(transform.position.x + i,
                            0,
                            transform.position.z + j);

                    // Creates the new tile
                    GameObject tile =
                        Instantiate(tPrefab,
                        pos,
                        Quaternion.identity,
                        transform);

                    // Sets tile coordinates
                    tile.GetComponent<TileInformation>().SetTileData(i, j);
                    tileList.Add (tile);
                }
            }
        }
    }
}
