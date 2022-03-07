using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        public Text amountStart;

        [HideInInspector]
        public int placedAmount = 0;
    }

    // Create multiple ships in the list
    public List<SpaceshipsToPlace>
        spaceshipList = new List<SpaceshipsToPlace>();

    int currentSpaceship = 0;

    // Performs raycast
    RaycastHit hit;

    // Vector Position where hit
    Vector3 hitPoint;

    // Start is called before the first frame update
    void Start()
    {
        UpdateAmountText();

        ShipGhostActivation(1);
        //ShipGhostActivation(currentSpaceship);
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
        // Takes all child transform information from the game manager and takes ghost array to update grid, nuShip brought over to game manager
        GameManager.instance.UpdatesGrid(spaceshipList[currentSpaceship].spaceshipGhost.transform, nuShip.GetComponent<CraftBehaviour>(), nuShip);
        //Update placed amount of the current Spaceship
        spaceshipList[currentSpaceship].placedAmount++;
        //  Deactivate deployment
        isPlacing = false;
        // Deactivate all ghosts
        ShipGhostActivation(-1); //passes in a negative index to deactivate this
        // Check if all spaceships are placed

        // Update the UI display of how many ships are left to be placed.
        UpdateAmountText();
    }

    /// <summary>
    /// This function is to address the behaviour of the ship selection buttons and ties in with ghost behaviour
    /// </summary>
    /// <param name="index"></param>
    public void PlacingSpaceshipsButton(int index)
    {
        //makes sure if the ship has been placed
        if (AllSpaceshipsPlaced(index))
        {
            //if ships have not been placed, no return
            print("No more Spaceships to place");
            return;
        }

        //Activating SpaceShipGhost
        currentSpaceship = index;
        ShipGhostActivation(currentSpaceship);
        isPlacing = true; //activating is placing

    }

    bool AllSpaceshipsPlaced(int index)
    {
        //if placed amount is not the same as noToPlace return true so we can place ship
        return spaceshipList[index].placedAmount == spaceshipList[index].noToPlace; 
        
    }

    /// <summary>
    /// Method for updating the UI display of the amount of ships to lace. 
    /// </summary>
    void UpdateAmountText()
    {
        for (int i = 0; i < spaceshipList.Count; i++)
        {
            spaceshipList[i].amountStart.text = (spaceshipList[i].noToPlace - spaceshipList[i].placedAmount).ToString();
        }
    }

    /// <summary>
    /// Method called from the clear button to delete all placed ships.
    /// </summary>
    public void ClearAllShips()
    {
        GameManager.instance.DestroyAllShips();
        
        foreach( var ship in spaceshipList)
        {
            ship.placedAmount = 0;
        }
        UpdateAmountText();
    }
}
