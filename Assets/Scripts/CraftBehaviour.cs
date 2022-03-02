using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Controls the behaviour of spaceship silhouettes
 * 
 * @author Enigma Studios
 * @version 02-03-2022
 */
public class CraftBehaviour : MonoBehaviour
{
    // int to determine ship damage
    public int spaceshipLength;
    // Tracks hits on spaceship
    int hitCount;

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
    public void TakeDamage()
    {
        hitCount--;
        if (isDestroyed())
        {
            // Keeps log of spaceship status to game manager

            // meshrenderer - unhides ship
        }
    }
}
