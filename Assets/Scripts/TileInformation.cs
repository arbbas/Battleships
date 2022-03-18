using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Script to process and control behaviour of the tile
 * @reference - basic game code is adapted from Udemy tutorial 'Battleships 3D', available at: https://www.udemy.com/course/unity-game-tutorial-battleships-3d/ 

 * @author Enigma Studios
 * @version 01-03-2022
 */
public class TileInformation : MonoBehaviour
{
    // Grid Position Variables
    public int xPosition;

    public int zPosition;

    // Is tile shot or not
    bool shot;

    // To Change sprite highlight based on Tile
    public SpriteRenderer sprite;

    // Sprite Array to Store Highlight-Icons
    public Sprite[] tileHighlights; // 0 = Frame, 1 = Crosshair, 2 = Miss, 3 = Hit

    /** Adds Functionality to Mouse Over Tile to Return Correct Highlight Icon
	 * 
	 * @param index - The assosiated number to highlight icon.
	 */





    public void HighlightActivate(int index, bool _shot)
    {
        sprite.sprite = tileHighlights[index];

        //COLOUR THE SPRITE
        if (index == 2)
        {
            sprite.color = Color.blue;
        }

        if (index == 3)
        {
            sprite.color = Color.red;
        }



        shot = _shot;



    }

    /** Sets the X and Z position to be called when generating the playfield
	 * 
	 * @param _xPosition  - Setter
     * @param _zPosition - Pass in the coordinates
	 */
    public void SetTileData(int _xPosition, int _zPosition)
    {
        xPosition = _xPosition;
        zPosition = _zPosition;
    }

    // Reacts when mouse hovers over tile and calls method to show crosshair
    void OnMouseOver()
    {
        if (GameManager.instance.gameState == GameManager.GameStates.SHOOTING)
        {
            if (!shot)
            {
               HighlightActivate(1, false);
            }
            if (Input.GetMouseButton(0))
            {
                //  GAME MANAGER CHECK THIS COORDINATE
                GameManager.instance.CheckShoot(xPosition, zPosition, this);
            }
        }
    }

    // Only show highlight-icon when needed. Resets to frame.
    void OnMouseExit()
    {
        HighlightActivate(0, false);
    }
}
