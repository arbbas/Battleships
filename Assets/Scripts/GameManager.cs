using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            TileInformation tileInfo = child.GetComponent<GhostActions>().GetTileInformation();
            players[playerTurn].grid[tileInfo.xPosition, tileInfo.zPosition] = new Tile(ship.type, ship);
        }
    }
        
}

