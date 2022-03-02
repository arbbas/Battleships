using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Placing behaviour for spaceships
 * 
 * @author H Newman
 * @version 01-03-2022
 */
public class PlaceManager : MonoBehaviour
{

    // Activate or deactivates the placing mode
    public bool isPlacing; 
    // Checks if tile is available
    bool canPlace;

    // instance of playfield where tiles are stored. 
    PhysicalPlayfield playfield;
    
    // Only checks tiles
    public LayerMask layerChecker;

    // Contains the spaceship ghosts and prefabs to pass to the playfield. 
    [System.Serializable]
    public class SpaceshipsToPlace{

        public GameObject spaceshipGhost;
        public GameObject spaceshipPrefab;
        public int noToPlace = 1;
        [HideInInspector]public int placedAmount = 0;
    }

    // Create multiple ships in the list
    public List<SpaceshipsToPlace> spaceshipList = new List<SpaceshipsToPlace>();
    int currentSpaceship;

    // Performs raycast
    RaycastHit hit;
    // Vector Position where hit
    Vector3 hitPont;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Creates ray when placing
        if(isPlacing){
            // Shoots mouse position to ray
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // Checks information about tile under ray
            if(Physics.Raycast(ray,out hit,Mathf.Infinity,layerChecker)){
                // Check if the tile is on correct playfield

                // return

                hitPont = hit.point;
            }

            if(Input.GetMouseButtonDown(0) && canPlace){
            // Place spaceship
            }

        if(Input.GetMouseButtonDown(1)){
            // Rotate spaceship
        }


        // Place Ghost

        }

    }
}
