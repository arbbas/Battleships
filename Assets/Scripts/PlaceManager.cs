using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Placing behaviour for spaceships
 * 
 * @author Enigma Studios
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
    public class SpaceshipsToPlace
    {
        public GameObject spaceshipGhost;

        public GameObject spaceshipPrefab;

        public int noToPlace = 1;

        [HideInInspector]
        public int placedAmount = 0;
    }

    // Create multiple ships in the list
    public List<SpaceshipsToPlace>
        spaceshipList = new List<SpaceshipsToPlace>();

    int currentSpaceship = 3;

    // Performs raycast
    RaycastHit hit;

    // Vector Position where hit
    Vector3 hitPoint;

    // Start is called before the first frame update
    void Start()
    {
        ShipGhostActivation(1);
        ShipGhostActivation(currentSpaceship);
    }

    // Update is called once per frame
    void Update()
    {
        // Creates ray when placing
        if (isPlacing)
        {
            // Shoots mouse position to ray
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Checks information about tile under ray
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerChecker))
            {
                // Check if the tile is on correct playfield
                // return
                hitPoint = hit.point;
            }

            if (Input.GetMouseButtonDown(0) && canPlace)
            {
                // Place spaceship
                DeploySpaceship();
            }

            if (Input.GetMouseButtonDown(1))
            {
                // Rotate spaceship
                RotateSpaceship();
            }
            // Place Ghost
            PlaceSpaceshipGhost();
        }
    }

    /** Method to activate only the selected ghost
	 * 
     * @param int ID assosiated with spaceship ghost.
	 */
    void ShipGhostActivation(int id)
    {
        if (id != -1)
        {
            if (spaceshipList[id].spaceshipGhost.activeInHierarchy)
            {
                // Make sure action isn't repeated
                return;
            }
        }
        // Deactivate all spaceship ghosts
        for (int i = 0; i < spaceshipList.Count; i++)
        {
            spaceshipList[i].spaceshipGhost.SetActive(false);
        }

        // return when don't want activated spaceship ghosts
        if (id == -1)
        {
            return;
        }

        // Activate required spaceship ghost
        spaceshipList[id].spaceshipGhost.SetActive(true);
    }

    // Places ghost
    void PlaceSpaceshipGhost()
    {
        if (isPlacing)
        {
            canPlace = CheckForSpaceships();
            // Place current spaceship ghost
            spaceshipList[currentSpaceship].spaceshipGhost.transform.position = new Vector3(Mathf.Round(hitPoint.x), 0, Mathf.Round(hitPoint.z));
        }
        else {
            // Deactivate all ghosts
            ShipGhostActivation(-1);
        }
    }

    /** Checks for other spaceship ghosts for placing to avoid duplicate tile occupancies
	 * 
     * @return true or false - over a spaceship or not.
	 */
    bool CheckForSpaceships()
    {
        foreach(Transform child in spaceshipList[currentSpaceship].spaceshipGhost.transform)
        {
            GhostActions ghost = child.GetComponent<GhostActions>();
            if(!ghost.TileHover())
            {
                //Tints red
                child.GetComponent<MeshRenderer>().material.color = new Color32(255, 0, 0, 125);
                return false;
            }
            // Tints back to white
            child.GetComponent<MeshRenderer>().material.color = new Color32(0, 0, 0, 100);
        }
        return true;
    }

    // Rotate 90 degrees
    void RotateSpaceship()
    {
        spaceshipList[currentSpaceship].spaceshipGhost.transform.localEulerAngles += new Vector3(0, 90f, 0);
    }

    // Deploys spaceship on grid
    void DeploySpaceship()
    {
        // Takes current hitpoint and rounds vector to position
        Vector3 position = new Vector3(Mathf.Round(hitPoint.x), 0, Mathf.Round(hitPoint.z));
        Quaternion rotation = spaceshipList[currentSpaceship].spaceshipGhost.transform.rotation;
        GameObject nuShip = Instantiate(spaceshipList[currentSpaceship].spaceshipPrefab, position, rotation);

        // Update grid

        //  Deactivate deployment

        // Deactivate all ghosts

        // Check if all spaceships are placed
    }

}
