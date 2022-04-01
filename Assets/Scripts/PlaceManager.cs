using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Placing behaviour for spaceships
 * 
 * @reference - basic game code is adapted from Udemy tutorial 'Battleships 3D', available at: https://www.udemy.com/course/unity-game-tutorial-battleships-3d/ 
 * 
 * @author Enigma Studios
 * @version 11-03-2022
 */
public class PlaceManager : MonoBehaviour
{
    //Access point for the placing manager. 
    public static PlaceManager instance;

    // Activate or deactivates the placing mode
    bool isPlacing;

    // Checks if tile is available
    bool canPlace;

    // instance of playfield where tiles are stored.
    PhysicalPlayfield playfield;

    // Only checks tiles
    public LayerMask layerChecker;

    // Reference to the ready button.
    public Button readyButton;

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

    /// <summary>
    /// Called before start.
    /// Makes this place manager the publicly exposed.
    /// </summary>
    private void Awake()
    {
        instance = this;
    }




    /// <summary>
    /// Before the first frame:
    /// text displaying amount of ships to place is updated
    /// ghost ship is activated
    /// 'ready' button is set to inactive
    /// </summary>
    void Start()
    {
        UpdateAmountText();

        ShipGhostActivation(1);
        //ShipGhostActivation(currentSpaceship);

        readyButton.interactable = false;
    }

    /// <summary>
    /// Method for determining which board the player can place their ships on. 
    /// Sets the ready button to inactive when first invoked
    /// And resets the ships in the placedShip list (as player 2 has not placed anything yet!)
    /// </summary>
    /// <param name="playfield"></param>
    public void SetPlayfield(PhysicalPlayfield playfield)
    {
        this.playfield = playfield;
        readyButton.interactable = false;

        ClearAllShips();
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
                if(!playfield.RequestTile(hit.collider.GetComponent<TileInformation>()))
                {
                    return;
                }


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

    bool CheckForSpaceshipsAuto(Transform tr)
    {
        foreach (Transform child in tr.transform)
        {
            GhostActions ghost = child.GetComponent<GhostActions>();
            if (!ghost.TileHover())
            {
                //Tints red
                //child.GetComponent<MeshRenderer>().material.color = new Color32(255, 0, 0, 125);
                return false;
            }
            // Tints back to white
            //child.GetComponent<MeshRenderer>().material.color = new Color32(0, 0, 0, 100);
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
        AllSpaceshipsPlaced();

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
    /// Method for checking if a player has placed all their ships. 
    /// Used to activate the 'ready' button to switch control to player 2.
    /// </summary>
    /// <returns> boolean value </returns>
    bool AllSpaceshipsPlaced()
    {
        foreach (var ship in spaceshipList)
        {
            if (ship.placedAmount != ship.noToPlace)
            {
                return false;
            }
        }

        readyButton.interactable = true;
        return true;
    }

    /// <summary>
    /// Method for updating the UI display of the amount of ships to place. 
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

        //THE FOLLOWING DISABLES THE READY BUTTON UNTIL ALL THE SHIPS ARE PLACED
        readyButton.interactable = false;
    }

    public void AutoPlaceShip()
    {
        //CLEAR ALL SHIPS FIRST
        ClearAllShips();
        //GameManager.instance.DestroyAllShips();

        bool posFound = false;

        //LOOP THROUGH ALL SPACESHIPS FIRST
        for (int i = 0; i < spaceshipList.Count; i++)
        {
            //Debug.Log("Locate where script breaks");
            //LOOP THROUGH EVERY SPACESHIP AMOUNT
            for (int j = 0; j < spaceshipList[i].placedAmount; j++)
            {
                Debug.Log("Locate where script breaks 1");
                if (spaceshipList[i].placedAmount <= 0)
                {
                    print("error - negative ship amount in the place manager");
                    return;
                }
                posFound = false;

                //WHILE NO POSITION FOUND
                while(!posFound)
                {
                    //STAY HERE UNTIL FINDS A POSSIBLE POSITION - OR A USEFUL POSITION
                    currentSpaceship = i;
                    //PICK A RANDOM START POSITION
                    int xPos = Random.Range(0, 10);
                    int zPos = Random.Range(0, 10);     //THESE FIGURES ARE BASED ON THE GRID 
                    //CREATE A GHOST
                    GameObject tempGhost = Instantiate(spaceshipList[currentSpaceship].spaceshipGhost);
                    tempGhost.SetActive(true);
                    //SET GHOST PLAYFIELD????? - WE PROBABLY WON'T NEED THIS 

                    //SET POSITION OF GHOST SPACESHIPS
                    tempGhost.transform.position = new Vector3(playfield.transform.position.x + xPos, 0, playfield.transform.position.z + zPos);

                    Vector3[] rot = {Vector3.zero, new Vector3(0, 90, 0), new Vector3(0, 180, 0), new Vector3(0, 270, 0) };

                    //CHECK FOR ALL ROTATIONS
                    for (int r = 0; r < rot.Length; r++)
                    {
                        List<int> indexList = new List<int> { 0, 1, 2, 3 };
                        int rr = indexList[Random.Range(0, indexList.Count)];

                        tempGhost.transform.rotation = Quaternion.Euler(rot[rr]);
                        if (CheckForSpaceshipsAuto(tempGhost.transform))
                        {
                            PlaceAutoSpaceShip(tempGhost);
                            posFound = true;
                        }
                        else
                        {
                            Destroy(tempGhost);
                            indexList.Remove(r);
                        }
                    }
                }

            }
        }
        //readyButton.interactable = true;
        CheckForSpaceships();                           //CHECK THIS LINE IS CORRECT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //AllSpaceshipsPlaced();
    }


    public void PlaceAutoSpaceShip(GameObject temp)
    {
    GameObject newSpaceShip = Instantiate(spaceshipList[currentSpaceship].spaceshipPrefab, temp.transform.position, temp.transform.rotation);
    GameManager.instance.UpdatesGrid(temp.transform, newSpaceShip.GetComponent<CraftBehaviour>(), newSpaceShip);
    spaceshipList[currentSpaceship].placedAmount++;

    Destroy(temp);
    UpdateAmountText();


        
    }
}
