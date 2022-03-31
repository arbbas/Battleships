using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @reference - basic game code is adapted from Udemy tutorial 'Battleships 3D', available at: https://www.udemy.com/course/unity-game-tutorial-battleships-3d/ 
 * @version - 10-03-2022
 * @author - Enigma Studios
 */
public enum OccupationType
{
    EMPTY,
    CORVETTE,
    DESTROYER,
    CRUISER,
    BATTLESHIP,
    CARRIER,
}

/// <summary>
/// Store information when we generate a new tile to be stored in the playfield
/// </summary>
public class Tile
{
    public OccupationType type; //needs to be stored so that we can read it
    public CraftBehaviour spaceShipType;
    public CraftBehaviour placedship;
    // Constructor
    public Tile(OccupationType _occupationType, CraftBehaviour _spaceShipType)
    {
        type = _occupationType;
        spaceShipType = _spaceShipType;
    }

    /// <summary>
    /// If the type equals and of the ship types, it returns true otherwise it returns false
    /// </summary>
    /// <returns></returns>
    public bool IsBeingOccupied()
    {
        return type == OccupationType.CRUISER ||
            type == OccupationType.BATTLESHIP ||
            type == OccupationType.DESTROYER ||
            type == OccupationType.CARRIER ||
            type == OccupationType.CORVETTE;
            
    }
}
