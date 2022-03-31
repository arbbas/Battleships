using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Controls the behaviour of spaceship silhouettes
 * @reference - basic game code is adapted from Udemy tutorial 'Battleships 3D', available at: https://www.udemy.com/course/unity-game-tutorial-battleships-3d/ 

 * @author Enigma Studios
 * @version 02-03-2022
 */
public class CraftBehaviour : MonoBehaviour
{
    // int to determine ship damage 
    public int spaceshipLength;
    // Tracks hits on spaceship
    int hitCount;
    public OccupationType type;

    void Start()
    {
        hitCount = spaceshipLength;
    }

    bool isDestroyed()
    {
        // if size is zero spaceship is destroyed
        return hitCount <= 0;
    }

    /** Check if hit, makes sure not destroyed
	 * 
     * @return True or False hit.
	 */
    public bool isHit()
    {
        return hitCount < spaceshipLength && hitCount > 0;
    }

    // Method to damage spaceship
    public bool TakeDamage()
    {
        hitCount--;
        if (isDestroyed())
        {
            // Keeps log of spaceship status to game manager

            // meshrenderer - unhides ship
            GetComponent<MeshRenderer>().enabled = true;
            return true;
        }
        return false;
    }
}
