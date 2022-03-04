using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Controls the behaviour of spaceship silhouettes
 * 
 * @author Enigma Studios
 * @version 02-03-2022
 */
public class GhostActions : MonoBehaviour
{
    //

    public LayerMask layerToCheck;

    // Raycast for hit information
    RaycastHit hit;

    // Get info from the checked tile
    TileInformation info;

    // Connection to the playfield
    PhysicalPlayfield playfield;

    /** Sets the currect playfield in GhostACtions to stop unathorised access to opponent playfield. 
	 * 
     * @param _pField - Pass in the current physical playfield.
	 */
    public void SetPlayingfield(PhysicalPlayfield _pField)
    {
        playfield = _pField;
    }

    /** Check if over a tile
	 * 
     * @return True or False over a tile.
	 */
    public bool TileHover()
    {
        info = GetTileInformation();

        if (info != null && !GameManager.instance.IsOccupied(info.xPosition, info.zPosition))
        {
            // Check if tile is occupied
            return true;
        }

        // clears the tile info
        info = null;
        return false;
    }

    /** Sets the currect playfield in GhostACtions to stop unathorised access to opponent playfield. 
	 * AB - must be public for the game manager as accessing this as component
     * @return hit.collider.GetComponent<TileInformation>() - Return info of the tile being checked.
	 */
    public TileInformation GetTileInformation()
    {
        // Shoots ray down
        Ray ray = new Ray(transform.position, -transform.up);

        // Physics check on the ray.
        if (Physics.Raycast(ray, out hit, 1f, layerToCheck))
        {
            Debug.DrawRay(ray.origin, ray.direction, Color.red);
            return hit.collider.GetComponent<TileInformation>();
        }

        // No tile found
        return null;
    }
}
