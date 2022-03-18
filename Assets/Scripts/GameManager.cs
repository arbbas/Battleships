using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @reference - basic game code is adapted from Udemy tutorial 'Battleships 3D', available at: https://www.udemy.com/course/unity-game-tutorial-battleships-3d/ 

 * @version 11-03-2020
 * @author Enigma Studios
 * 
 */
public class GameManager : MonoBehaviour
{
    public static GameManager instance;



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
        public bool[,] hitGrid = new bool[10,10]; //checks whether the grid has been shot and shown meaning cannot be shot again
        public PhysicalPlayfield physicalPlayfield; //links playfields to this script so that the information can be passed over
        public LayerMask placementLayer; //layer that we will place on

        public List<GameObject> placedSpaceshipList = new List<GameObject>(); //store all physical ships when they are placed

        //Reference for the position of the camera
        public GameObject camPosition;

        //Reference for the placing panel
        public GameObject placePanel;

        //Reference for the shooting confirmation panel
        public GameObject shootPanel;



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

    //ROCKET
    public GameObject rocketPrefab;
    float amplitude = 3f;
    float cTime;


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

        //first player placing functionality is activated.
        players[playerTurn].placePanel.SetActive(true);

        gameState = GameStates.IDLE;


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
        foreach(Transform child in shipTransform)
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
                if(players[playerTurn].grid[i,y].type == OccupationType.BATTLESHIP)
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
                if(seperator == 9)
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
        foreach(GameObject ship in players[playerTurn].placedSpaceshipList)
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
        switch(gameState)
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


                    PlaceManager.instance.SetPlayfield(players[playerTurn].physicalPlayfield);

                    gameState = GameStates.IDLE;
                }
                break;

            case GameStates.SHOOTING:
                {

                }
                break;
        }
    }

    /// <summary>
    /// Deactivates all the panels in the game UI
    /// </summary>
    void HideAllPanels()
    {
        players[0].placePanel.SetActive(false);
        players[0].shootPanel.SetActive(false);

        players[1].placePanel.SetActive(false);
        players[1].shootPanel.SetActive(false);

    }

    public void P1PlaceShips()
    {
        gameState = GameStates.PLAYER1DEPLOY;

        

    }

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
        if(playerTurn == 0)
        {
            //hide ships
            HideAllMyShips();

            //switch active player to p2
            SwitchPlayer();
            //move the camera to p2 board
            StartCoroutine(CameraMovement(players[playerTurn].camPosition));
            //activate p2 placing panel
            players[playerTurn].placePanel.SetActive(true);
            //Return
            return;
        }

        if(playerTurn == 1)
        {
            //hide ships
            HideAllMyShips();

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
    void UnideAllMyShips()
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
    IEnumerator CameraMovement (GameObject camObj)
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
        while(t < duration)
        {
            t+=Time.deltaTime;
            Camera.main.transform.position = Vector3.Lerp(startPosition, endPosition, t / duration);
            Camera.main.transform.rotation = Quaternion.Lerp(startRotation, endRotation, t / duration);
                // LERP INTERPOLATES BETWEEN THE 2 VECTOR POINTS WE HAVE ESTABLISHED BETWEEN PLAYER 1 AND 2 AS WELL AS FOR THE ROTATION POINTS
            yield return null;
        }

        movingCamera = false;
    }






    //-----------BATTLE MODE---------------

    //SHOOT PANEL BUTTONS
    public void ShotButton()
    {
        UnideAllMyShips();
        players[playerTurn ].shootPanel.SetActive(false);
        gameState = GameStates.SHOOTING;
    }

    int Opponent()
    {
        int me = playerTurn;
        me++;
        me %= 2;
        int opponent = me;
        return opponent;
    }

    public void CheckCoordinate(int x, int z, TileInformation info)
    {
        int opponent = Opponent();

        //if the tile is not the opponents tile
        if (!players[opponent].physicalPlayfield.RequestTile(info))
        {
            print("Don't Shoot Yourself!");
            return;
        }

        //IF PLAYER HAS SHOT THIS COORDINATE ALREADY?
        if (players[opponent].hitGrid[x, z] == true)
        {
            print("You have shot here already!");
            return;
        }

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

















        }
        else
        {
            //NOT HIT A SHIP
            info.HighlightActivate(2, true);
        }
        //REVEAL TILE
        players[opponent].hitGrid[x, z] = true;

        // CHECK IF A PLYER HAS WON
        if (players[opponent].placedSpaceshipList.Count == 0)
        {
            print("You Win!");
        }
        //HIDE MY SHIPS

        //SWITCH PLAYERS

        //ACTIVATE THE CORRECT PANELS

        //SWITCH GAMESTATE TO IDLE
    }
    
}

