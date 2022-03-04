using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Types of tile states
/// </summary>
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
