using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Script to process and control behaviour of the tile
 * 
 * @author H Newman
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
    public void HighlightActivate(int index){
        sprite.sprite = tileHighlights[index];
    }

    /** Sets the X and Z position to be called when generating the playfield
	 * 
	 * @param _xPosition  - Setter
     * @param _zPosition - Pass in the coordinates
	 */
    public void SetTileData(int _xPosition, int _zPosition){
        xPosition = _xPosition;
        zPosition = _zPosition;
    }

    // Reacts when mouse hovers over tile and calls method to show crosshair
    void OnMouseOver(){
        HighlightActivate(1);
    }

    // Only show highlight-icon when needed. Resets to frame.
    void OnMouseExit() {
        HighlightActivate(0);
    }
    

}
