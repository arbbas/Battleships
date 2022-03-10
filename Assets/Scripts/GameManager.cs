using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @reference - basic game code is adapted from Udemy tutorial 'Battleships 3D', available at: https://www.udemy.com/course/unity-game-tutorial-battleships-3d/ 

 * @version 10-03-2020
 * @author Enigma Studios
 * 
 */
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        instance = this; //responsible for access to the static game manager instance in this script

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
        public bool[,] hitGrid = new bool[10,10]; //checks whether the grid has been shot and shown meaning cannot be shot again
        public PhysicalPlayfield physicalPlayfield; //links playfields to this script so that the information can be passed over
        public LayerMask placementLayer; //layer that we will place on

        public List<GameObject> placedSpaceshipList = new List<GameObject>(); //store all physical ships when they are placed

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
    }


    int playerTurn; //variable to use for keep track whether it is player 1 or 2s go
    //creates two players immediately when game spins up
    public Player[] players = new Player[2];

    /// <summary>
    /// When start is called, tell the placing manager which board the player is allowed to put their ships on.
    /// </summary>
    private void Start()
    {
        PlaceManager.instance.SetPlayfield(players[playerTurn].physicalPlayfield);
        
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

}

