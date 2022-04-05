using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/**
 * @reference - basic game code is adapted from Udemy tutorial 'Battleships 3D', available at: https://www.udemy.com/course/unity-game-tutorial-battleships-3d/ 

 * @version 31-03-2020
 * @author Enigma Studios
 * 
 */
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public AudioSource Laser;

    public void PlayLaser()
    {
        Laser.Play();
    }

[System.Serializable]
    public class Player
    {
        public enum PlayerType
        {
            HUMAN,
            COMPUTER
        }

        public PlayerType playerType; //access the enum
        public Tile[,] grid = new Tile[10, 10]; //created a 2d array so that the tiles can be populated on x,y coordinates
        public bool[,] hitGrid = new bool[10, 10]; //checks whether the grid has been shot and shown meaning cannot be shot again
        public PhysicalPlayfield physicalPlayfield; //links playfields to this script so that the information can be passed over
        public LayerMask placementLayer; //layer that we will place on

        public List<GameObject> placedSpaceshipList = new List<GameObject>(); //store all physical ships when they are placed

        //Reference for the position of the camera
        public GameObject camPosition;

        //Reference for the placing panel
        public GameObject placePanel;

        //Reference for the shooting confirmation panel
        public GameObject shootPanel;

        //Reference for the win panels

        public GameObject WinPanels;



        //Between turn panels to prompt player before grid is updated
        //Constructor that runs whenever we make a new player
        public Player()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int y = 0; y < 10; y++)
                {
                    OccupationType oT = OccupationType.EMPTY; //whenever a new game is started, resets all tiles to empty
                    grid[i, y] = new Tile(oT, null); //no ships placed yet so null
                    hitGrid[i, y] = false; //no hits yet so set to false
                    //^^^^^^ MAKES FRESH GAME ^^^^^^
                }

            }
        }

        //public List<GameObject> placedShipList = new List<GameObject>();
    }



    int playerTurn; //variable to use for keep track whether it is player 1 or 2s go
    //creates two players immediately when game spins up
    public Player[] players = new Player[2];

    //-----Attributes for Fin's shot generator mechanism-----------------------------------------------

    // Reference for the remaining shots of the players turn
    [SerializeField]
    public int remainingShots;

    //Reference for the text objects displaying remaining shots in game.
    public Text P1shotsText;
    public Text P2shotsText;

    //Reference for the text displaying players shots this turn. Used for the reroll button.
    public Text P1LoadingShotsText;
    public Text P2LoadingShotsText;
    
    //Reference for the panels containing both players shooting information
    public GameObject P1ShotsPanel;
    public GameObject P2ShotsPanel;

    //Boolean value used to track if the players have used their reroll or not
    bool P1RerollUsed;
    bool P2RerollUsed;

    // Reference to the reroll buttons - this is used to deactivate them if they have been used already.
    public GameObject P1RerollButton;
    public GameObject P2RerollButton;

    //Random number generator - needed for the 'dice roll' for the shots.
    System.Random ran = new System.Random();

    //Attributes for the Power-Up feature

    //References for the power up buttons each player can use.
    public GameObject P1PowerUpButton;
    public GameObject P2PowerUpButton;

    //Bools to track if the players have used their power up or not
    bool P1PowerUpUsed;
    bool P2PowerUpUsed;

    //References for the 4 banners that display what turn it currently is
    public GameObject P1DeploymentUI;
    public GameObject P2DeploymentUI;
    public GameObject P1FirePhaseUI;
    public GameObject P2FirePhaseUI;

    public GameObject P1ShotCountUI;
    public GameObject P2ShotCountUI;

    //--------------------------------------------------------------------------------------------


    public enum GameStates
    {
        PLAYER1DEPLOY,
        PLAYER2DEPLOY,
        SHOOTING,
        IDLE
    }

    //Accessor for getting the current game state
    public GameStates gameState;

    //Camera for the field overview shot during gameplay
    public GameObject battleCamera;

    bool movingCamera;

    public GameObject placingCanvas;


    bool isShooting;    //PROTECT COROUTINE

    //-----------Fin's-Laser-Feature-----------------------------------------------------------------------------------------------
    public GameObject laserPrefab;

    //ROCKET
    public GameObject rocketPrefab;
    float amplitude = 3f;                   //THIS IS HOW HIGH THE ROCKET FLIES BEFORE SHOOTING - 3F MEANS IT WILL HAVE A HEIGHT OF 3M OVER THE PLAYFIELD
    float cTime;                            //THIS IS THE TIME BNETWEEN THE START AND END POINT OF THE ROCKET - THIS WILL MAKE USE OF THE LERP FUNCTIONALITY 

    // Fire FX objects
    public GameObject firePrefab;
    private List<GameObject> playerFires;


    private void Awake()
    {
        instance = this; //responsible for access to the static game manager instance in this script

    }

    /// <summary>
    /// When start is called, first all panels are deactivated, then the first players placing panel UI is activated
    /// </summary>
    private void Start()
    {
        HideAllPanels();

        //RANDOMISE PLAYER HERE
        players[0].WinPanels.SetActive(false);
        players[1].WinPanels.SetActive(false);

        //first player placing functionality is activated.
        players[playerTurn].placePanel.SetActive(true);

        //Deactivate RemainingShots panels
        P1ShotsPanel.SetActive(false);
        P2ShotsPanel.SetActive(false);

        gameState = GameStates.IDLE;

        P1RerollUsed = false;
        P2RerollUsed = false;


    }




    /// <summary>
    /// Function to add the ships into the list 
    /// </summary>
    /// <param name="placedSpaceship"></param>
    void AddSpaceshipToSpaceshipList(GameObject placedSpaceship)
    {
        players[playerTurn].placedSpaceshipList.Add(placedSpaceship);
    }

    /// <summary>
    /// Takes the ships transform, iterate through the child components of the ship ghosts and see if we can add them to
    /// the grid as tile info
    /// </summary>
    /// <param name="shipTransform"></param>
    /// <param name="ship"></param>
    /// <param name="placedship"></param>
    public void UpdatesGrid(Transform shipTransform, CraftBehaviour ship, GameObject placedship)
    {
        //loop through all the child components of ship transform and get information about their tile info
        foreach (Transform child in shipTransform)
        {
            TileInformation tileInfo = child.GetComponent<GhostActions>().GetTileInformation(); //updates tile array
            players[playerTurn].grid[tileInfo.xPosition, tileInfo.zPosition] = new Tile(ship.type, ship);
        }

        AddSpaceshipToSpaceshipList(placedship);
        DebuggingGrid();
    }

    /// <summary>
    /// Create string of information about OccupationTypes and loop through grid to check for information 
    /// </summary>
    ///

    public bool IsOccupied(int xpos, int zpos)
    {
        return players[playerTurn].grid[xpos, zpos].IsBeingOccupied();
    }

    void DebuggingGrid()
    {
        string s = "";
        int seperator = 0; //see between lines if something has been added (visual aid)
        for (int i = 0; i < 10; i++) // x axis
        {
            s += " - "; // seperates the strings
            for (int y = 0; y < 10; y++)// y axis
            {
                string type = "0"; //Occupation Type
                if (players[playerTurn].grid[i, y].type == OccupationType.BATTLESHIP)
                {
                    type = "Bat";
                }
                if (players[playerTurn].grid[i, y].type == OccupationType.CARRIER)
                {
                    type = "Car";
                }
                if (players[playerTurn].grid[i, y].type == OccupationType.CORVETTE)
                {
                    type = "Cor";
                }
                if (players[playerTurn].grid[i, y].type == OccupationType.CRUISER)
                {
                    type = "Cru";
                }
                if (players[playerTurn].grid[i, y].type == OccupationType.DESTROYER)
                {
                    type = "Des";
                }

                s += type;
                seperator = y % 10;
                if (seperator == 9)
                {
                    s += " - ";
                }
            }

            s += "\n";
        }

        print(s);
    }

    /// <summary>
    /// Method for deleting all ships that are currently placed on the board.
    /// Used for the 'reset' button during deployment phase.
    /// </summary>
    public void DestroyAllShips()
    {
        foreach (GameObject ship in players[playerTurn].placedSpaceshipList)
        {
            Destroy(ship);
        }
        players[playerTurn].placedSpaceshipList.Clear();

        InitGrid();
    }

    /// <summary>
    /// Method for reinitialising the board.
    /// Used after deleting ships from the grid using the Reset button
    /// </summary>
    void InitGrid()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                OccupationType t = OccupationType.EMPTY;
                players[playerTurn].grid[i, j] = new Tile(t, null);


            }
        }
    }


    //Battle Preparation

    /// <summary>
    /// Every time update is called, it checks what State the game is currently in.
    /// </summary>
    private void Update()
    {
        switch (gameState)
        {
            case GameStates.IDLE:
                {
                    
                }
                break;

            //Set the active players playfield,
            //switch into IDLE state
            case GameStates.PLAYER1DEPLOY:
                {
                    players[playerTurn].placePanel.SetActive(false);

                    //Set the banner that says what turn it is on.

                    P1DeploymentUI.SetActive(true);

                    PlaceManager.instance.SetPlayfield(players[playerTurn].physicalPlayfield);

                    //WE WILL ALSO START THE COROUTINE HERE (IENUMERATOR) SO THAT THE CAMERA MOVES TO THE CORRECT START POSITION
                    StartCoroutine(CameraMovement(players[playerTurn].camPosition));

                    gameState = GameStates.IDLE;
                }
                break;

            //Set the active players playfield.
            //switch into IDLE state 
            case GameStates.PLAYER2DEPLOY:
                {
                    players[playerTurn].placePanel.SetActive(false);

                    //Set the banner that says what turn it is on.
                    P2DeploymentUI.SetActive(true);

                    PlaceManager.instance.SetPlayfield(players[playerTurn].physicalPlayfield);

                    gameState = GameStates.IDLE;
                }
                break;

            case GameStates.SHOOTING:
                {


                    if(playerTurn == 0)
                    {
                        P1ShotsPanel.SetActive(true);
                    }
                    else
                    {
                        P2ShotsPanel.SetActive(true);
                    }


                }
                break;
        }
        UpdateShotText(P1shotsText);
        UpdateShotText(P2shotsText);
    }

    /// <summary>
    /// Deactivates all the panels in the game UI.
    /// Added in the 4 UI that 
    /// </summary>
    void HideAllPanels()
    {
        players[0].placePanel.SetActive(false);
        players[0].shootPanel.SetActive(false);

        players[1].placePanel.SetActive(false);
        players[1].shootPanel.SetActive(false);

        P1DeploymentUI.SetActive(false);
        P2DeploymentUI.SetActive(false);
        P1FirePhaseUI.SetActive(false);
        P2FirePhaseUI.SetActive(false);
    }

    public void P1PlaceShips()
    {
        gameState = GameStates.PLAYER1DEPLOY;

    }

    /// <summary>
    /// Change state to deployment
    /// </summary>
    public void P2PlaceShips()
    {
        gameState = GameStates.PLAYER2DEPLOY;

    }

    /// <summary>
    /// Method for called when 'Ready' button is pressed. 
    /// First transfers to player 2 deployment phase
    /// Then transitions into player 1 deployment phase
    /// </summary>
    public void EndDeploymentPhase()
    {
        if (playerTurn == 0)
        {
            //hide ships
            HideAllMyShips();

            //Disable the turn banner
            P1DeploymentUI.SetActive(false);

            //switch active player to p2
            SwitchPlayer();
            //move the camera to p2 board
            StartCoroutine(CameraMovement(players[playerTurn].camPosition));
            //activate p2 placing panel
            players[playerTurn].placePanel.SetActive(true);
            //Return
            return;
        }

        if (playerTurn == 1)
        {
            //hide ships
            HideAllMyShips();

            //Disable the turn banner
            P2DeploymentUI.SetActive(false);

            //switch active player to p1
            SwitchPlayer();
            //move the camera to p1 board
            StartCoroutine(CameraMovement(battleCamera));
            //activate p1 SHOOTING panel
            players[playerTurn].shootPanel.SetActive(true);
            //Unhide player 1 ships (maybe)

            //Deactivate placing canvas
            placingCanvas.SetActive(false);
            //Return
            //Call method to generate first set of shot numbers
            ShotCountDiceRoll();
            UpdateShotText(P1LoadingShotsText);
            UpdateShotText(P2LoadingShotsText);
        }
    }

    /// <summary>
    /// Method for deactivating the mesh renderer, so opponent cant see our pieces
    /// </summary>
    void HideAllMyShips()
    {
        foreach (var ship in players[playerTurn].placedSpaceshipList)
        {
            ship.GetComponent<MeshRenderer>().enabled = false;

        }
    }

    /// <summary>
    /// Method for activating the mesh renderer, so player can see their own pieces
    /// </summary>
    void UnhideAllMyShips()
    {
        foreach (var ship in players[playerTurn].placedSpaceshipList)
        {
            ship.GetComponent<MeshRenderer>().enabled = true;

        }
    }

    void SwitchPlayer()
    {
        playerTurn++;   //INCREMENT ACTIVE PLAYER BY 1
        playerTurn %= 2;    //THIS RESETS IT TO ZERO WHEN ACTIVE PLAYER IS 2 SO IT DOESN'T INCREMENT FURTHER
    }


    //TIME BASED ACTION FOR THE MOVEMENT OF THE CAMERA AS THE PLAYERS SWITCH
    //THE IENUMERATOR IS A COROUTINE
    IEnumerator CameraMovement(GameObject camObj)
    {
        if (movingCamera)
        {
            yield break;
        }

        movingCamera = true;

        //CAMERA MOVE TIME
        float t = 0; //START TIME
        float duration = 0.5f;  //TIME DURATION OF CAMERA MOVEMENT MADE HERE

        //BELOW WILL DETERMINE THE CAMERA POSITIONING (START)
        Vector3 startPosition = Camera.main.transform.position;
        //ROTATION OF THE CAMERA THROUGH USE OF A QUTERNION BELOW(START)
        Quaternion startRotation = Camera.main.transform.rotation;

        //BELOW WILL DETERMINE THE CAMERA POSITIONING (END)
        Vector3 endPosition = camObj.transform.position;
        //ROTATION OF THE CAMERA THROUGH USE OF A QUTERNION BELOW(END)
        Quaternion endRotation = camObj.transform.rotation;


        //ALL OF THE ABOVE INFO NOW GOES INTO A WHILE LOOP SO THAT WE CAN EXCECUTE THE MOVEMENT
        while (t < duration)
        {
            t += Time.deltaTime;
            Camera.main.transform.position = Vector3.Lerp(startPosition, endPosition, t / duration);
            Camera.main.transform.rotation = Quaternion.Lerp(startRotation, endRotation, t / duration);
            // LERP INTERPOLATES BETWEEN THE 2 VECTOR POINTS WE HAVE ESTABLISHED BETWEEN PLAYER 1 AND 2 AS WELL AS FOR THE ROTATION POINTS
            yield return null;
        }

        movingCamera = false;
    }






    //-----------BATTLE MODE---------------

    /// <summary>
    /// Player presses the shoot panel button. 
    /// Their ships become visible, the panel becomes hidden and the game state changes to SHOOTING.
    /// Fin note 30/03/22 - added functionality to hide the power up button if the power up has already been used.
    /// 31/03/22 - added functionality to enable the UI to say what turn it is.
    /// </summary>
    public void ShotButton()
    {
        //UnhideAllMyShips();
        players[playerTurn].shootPanel.SetActive(false);
        gameState = GameStates.SHOOTING;

        if (playerTurn == 0)
        {
            if (P1PowerUpUsed)
            {
                P1PowerUpButton.SetActive(false);
            }
            P1FirePhaseUI.SetActive(true);
        }
        else
        {
            if (P2PowerUpUsed)
            {
                P2PowerUpButton.SetActive(false);
            }
            P2FirePhaseUI.SetActive(true);
        }

    }

    /// <summary>
    /// Method for returning the number representing the opponent. 
    /// Used for determining legal gameboard is being shot at.
    /// </summary>
    /// <returns></returns>
    int Opponent()
    {
        int me = playerTurn;
        me++;
        me %= 2;
        int opponent = me;
        return opponent;
    }


    public void CheckShoot(int x, int z, TileInformation info)
    {
        StartCoroutine(CheckCoordinate(x, z, info));
    }


    IEnumerator CheckCoordinate(int x, int z, TileInformation info)
    {

          
         
            if (isShooting)
            {
              PlayLaser();
              yield break;
            }
            isShooting = true;


            int opponent = Opponent();

            //if the tile is not the opponents tile
            if (!players[opponent].physicalPlayfield.RequestTile(info))
            {
                print("Don't Shoot Yourself!");
                isShooting = false;
                yield break;
            }

            //IF PLAYER HAS SHOT THIS COORDINATE ALREADY?
            if (players[opponent].hitGrid[x, z] == true)
            {
                print("You have shot here already!");
                isShooting = false;
                yield break;
            }

            //-----------------------Fin-Laser-----------------------------------------------------------

            //Generating the laser shot
            Vector3 targetPosition = info.gameObject.transform.position;
            Vector3 laserStartPosition = new Vector3(targetPosition.x, (targetPosition.y + 5), targetPosition.z);
        GameObject laser = Instantiate(laserPrefab, laserStartPosition, new Quaternion(0f, 0f, 0f, 0f));

       
        /*
            //SHOOTING A ROCKET
            //the below is the start position of where the rocket will be shot from
            Vector3 startPosition = Vector3.zero;
            //the below is where the rocket reaches
            Vector3 endPosition = info.gameObject.transform.position;
            //INSTANTIATE A ROCKET
            GameObject rocket = Instantiate(rocketPrefab, startPosition, Quaternion.identity);

            //MOVE THE ROCKET INSIDE AN ARC 
            while (MovesInArcToTile(startPosition, endPosition, 0.5f, rocket))
            {
                yield return null;
            }
            Destroy(rocket);
            cTime = 0;
        */


            //CHECK IF THE TILE IS ALREADY OCCUPIED
            if (players[opponent].grid[x, z].IsBeingOccupied())
            {
                //DO DAMAGE TO THE SPACESHIP
                bool destroyed = players[opponent].grid[x, z].spaceShipType.TakeDamage();
                if (destroyed)
                {
                    players[opponent].placedSpaceshipList.Remove(players[opponent].grid[x, z].spaceShipType.gameObject);
                }
                //HIGHLIGHT THE TILE IN A DIFFERENT WAY
                info.HighlightActivate(3, true);

                //Reduce shot count by 1
                remainingShots -= 1;
                Debug.Log("Reduced shot count by 1 - Hit");

                // hit FX
                Vector3 firePosition = info.gameObject.transform.position;
                GameObject blast = Instantiate(firePrefab, firePosition, new Quaternion(0f, 0f, 0f, 0f));

            }
            else
            {
                //NOT HIT A SHIP
                info.HighlightActivate(2, true);
                //Reduce shot count by 1
                remainingShots -= 1;
                Debug.Log("Reduced shot count by 1 - Miss");
            }
            //REVEAL TILE
            players[opponent].hitGrid[x, z] = true;
            

            // CHECK IF A PLAYER HAS WON
            if (players[opponent].placedSpaceshipList.Count == 0)
            {
                HideAllPanels();
                print("You Win!");
                P1ShotCountUI.SetActive(false);
                P2ShotCountUI.SetActive(false);
                 yield return new WaitForSeconds(3);

                players[playerTurn].WinPanels.SetActive(true);
                yield break;
                Debug.Log("Check if game won");
            }
            yield return new WaitForSeconds(2f);


            
            //Update the remaining shots of the player in question
            if(playerTurn == 0)
            {                         
                UpdateShotText(P1shotsText);
                Debug.Log("Update P1 text");
            }
            else
            {
                UpdateShotText(P2shotsText);
                Debug.Log("Update P2 text");

            }

        //If the player has no more shots, end the turn
        if (remainingShots == 0)
            {

                yield return new WaitForSeconds(2f);

                //Deactivate the shots panel for the player
                if (playerTurn == 0)
                {
                    P1ShotsPanel.SetActive(false);
                    P1FirePhaseUI.SetActive(false);
                    Debug.Log("Deactivated P1 Panel");

                }
            else
                {
                    P2ShotsPanel.SetActive(false);
                    P2FirePhaseUI.SetActive(false);
                    Debug.Log("Deactivated P2 Panel");
                }
                //End the players turn
                EndPlayerTurn();
            }
        else
        {
            isShooting = false;
        }

    }

    /// <summary>
    /// Method used to handle end of one players shooting turn and prepare for player 2.
    /// </summary>
    void EndPlayerTurn()
    {
        //HIDE MY SHIPS
        HideAllMyShips();
        //SWITCH PLAYERS
        SwitchPlayer();
        //ACTIVATE THE CORRECT PANELS
        players[playerTurn].shootPanel.SetActive(true);

        //SWITCH GAMESTATE TO IDLE
        gameState = GameStates.IDLE;
        isShooting = false;


        //Generate Shot count for next player
        ShotCountDiceRoll();
       //Prepare the text for the next players loading screen
        if(playerTurn == 0)
        {
            UpdateShotText(P1LoadingShotsText);
            if(P1RerollUsed)
            {
                P1RerollButton.SetActive(false);
            }
        }
        else
        {
            UpdateShotText(P2LoadingShotsText);
            if (P2RerollUsed)
            {
                P2RerollButton.SetActive(false);
            }
        }
    }

    bool MovesInArcToTile(Vector3 startPosition, Vector3 endPosition, float speed, GameObject rocket)
    {
        cTime += speed * Time.deltaTime;        //Time.deltatime makes the movement of the rocket smooth
        Vector3 nextPos = Vector3.Lerp(startPosition, endPosition, cTime);
        nextPos.y = amplitude * Mathf.Sin(Mathf.Clamp01(cTime) * Mathf.PI);     //THIS USES THE CALCULATION OF A CIRCLE FOR THE ARCING MOVEMENT 
        rocket.transform.LookAt(nextPos);
        return endPosition != (rocket.transform.position = Vector3.Lerp(rocket.transform.position, nextPos, cTime));

    }



    //---------------------------------------Fin's Multi-Shot Mechanic-----------------------------------------------------------------------------------

    /// <summary>
    /// Method used to update the amount of shots displayed in the text field.
    /// </summary>
    void UpdateShotText(Text remainingShotsText)
    {
        remainingShotsText.text = remainingShots.ToString();
    }

    /// <summary>
    /// Use the c# Random to generate a number of shots between an upper and lower bound.
    /// </summary>
    /// <returns></returns>
    public void ShotCountDiceRoll()
    {
        remainingShots = ran.Next(1, 6);
    }


    /// <summary>
    /// Method called when the reroll button is pressed.
    /// </summary>
    public void rerollButton()
    {
        ShotCountDiceRoll();

        if(playerTurn == 0)
        {
            UpdateShotText(P1LoadingShotsText);
            P1RerollUsed = true;
            P1RerollButton.SetActive(false);
        }
        else
        {
            UpdateShotText(P2LoadingShotsText);
            P2RerollUsed = true;
            P2RerollButton.SetActive(false);
        }
    }

    /// <summary>
    /// Method called when the power-up button is clicked.
    /// Adds one more shot for the player.
    /// Changes boolean tracking use of power-up to true.
    /// Disables the power-up button.
    /// </summary>
    public void usePowerUpButton()
    {
        if (remainingShots == 0)
        {
            return;
        }

        remainingShots += 1;

        if (playerTurn == 0)
        {
            UpdateShotText(P1shotsText);
            P1PowerUpUsed = true;
            P1PowerUpButton.SetActive(false);

        }
        else
        {
            UpdateShotText(P2shotsText);
            P2PowerUpUsed = true;
            P2PowerUpButton.SetActive(false);
        }
    }


}

