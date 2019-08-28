using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public Texture2D map;

    public ColourToPrefab[] colourMappings;

    public int mapX;
    public int mapY;

    public char direction;
    public int pathLength;
    public Vector3 startCoordinates;
    public float progress;
    public float scaledMovement;

    public bool inMovingMode;

    public GameObject DirectionSelector;

    public List<Vector3> previousPositions;

    public int holdDownCount;

    public List<Color> impassableColours;

    private void Start()
    {
        ChangeState(false);
        map = Camera.main.GetComponent<LevelGenerator>().map;
        DirectionSelector = GameObject.Find("Direction Selector");
    }

    private void Update()
    {
        impassableColours = Camera.main.GetComponent<WallManager>().impassableColours;
        //MovingMode() and ChoosingMode() make up the state machine with 2 states
        //If the player is not in one, they must be in the other
        if (inMovingMode)
        {
            MovingMode();
        }
        else
        {
            //The camera should be at the player position when in choosing mode, especially after the scene first loads
            Camera.main.transform.position = transform.position + new Vector3 (0,19.5f,0);
            // If a direction has been selected (upper/lowercase u,d,l,r) then Moving mode is entered.
            if (direction != 'z')
            {
                ChangeState(true);
            }
        }

        // If 1000 milliseconds pass with a button pressed then it is counted as a hold, and the player position is reset to the previous position, and they reenter Choosing Mode.
        if (holdDownCount > 50)
        {
            if (previousPositions.Count > 0)
            {
                ChangeState(false);
                transform.position = previousPositions[previousPositions.Count - 1];
                previousPositions.RemoveAt(previousPositions.Count - 1);
                holdDownCount = 0;
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    private void FixedUpdate()
    {
        // This bit increments each frame while the mouse is being held down.
        // When the button is released the count resets.
        // This is in FixedUpdate() to keep the required hold time consistent (50 FixedUpdates per second = 0.02 seconds)
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            holdDownCount++;
        }
        else
        {
            holdDownCount = 0;
        }
    }

    /*
     * The player is always in one of two states, Moving or Choosing. They cannot change their direction while Moving and cannot move while Choosing.
     * It is a public function so can be accessed by other scripts.
     * When moving mode is entered, progress is reset, the direction selector UI is disabled, the player is recentered on their square, and the current position becomes a point on the list of previous positions.
     * From there it gets the length of the path in the direction chosen.
     * If the player is Choosing then the direction is set to the null value ('z') and MyPAM targets are set to 0 to keep it centered in preperation.
    */ 
    public void ChangeState(bool goIntoMovingMode)
    {
        inMovingMode = goIntoMovingMode;
        if (goIntoMovingMode == true)
        {
            DirectionSelector.SetActive(false);
            progress = 0;

            startCoordinates = transform.position;
            mapX = Mathf.RoundToInt(startCoordinates.x/2);
            mapY = Mathf.RoundToInt(startCoordinates.z/2);
            transform.position = new Vector3(mapX*2, 0.5f, mapY*2);     
            startCoordinates = transform.position;                  
            previousPositions.Add(startCoordinates);

            GetPathLength();
        }
        else
        {
            DirectionSelector.SetActive(true);

            UDP_Handling.Xtarget = 0;
            UDP_Handling.Ytarget = 0;
            direction = 'z';
        }
    }

    /*
     * This function controls which movement function and direction (+/- 1) is used for moving.
    */ 
    void MovingMode()
    {
        switch (direction)
        {
            case ('U'):
            case ('u'):
                MoveVertical(1);
                break;
            case ('D'):
            case ('d'):
                MoveVertical(-1);
                break;
            case ('R'):
            case ('r'):
                MoveHorizontal(1);
                break;
            case ('L'):
            case ('l'):
                MoveHorizontal(-1);
                break;
            //if no direction has been set then there has been a coding error, which is notified by a log message
            default:
                Debug.Log("Error, no direcftion set");
                return;
        }
    }

    /*
     * These two functions are very similar and serve a similar puporse but there are subtle structural differences which would make refactorying complex and difficult to read through.
     * One is for vertical movement (z axis relating to the y axis on the MyPAM), the other is for horizontal movement (x axis).
     * Progress is out of an arbitrary value of 100, relating to the convention of using % (/100) to measure progress.
     * The movement of the joystick from centre to the correct edge will move the character the entire way along the path and increase progress to 50%.
     * The camera remains stationary for this movement. Moving the camera back to the player will require moving the joystick back to the centre of the workspace.
     * This arbritrary requirement means each path requires two arm motions so that the joystick is always centered after a move so that the next move can use the full MyPAM work space as well.
     * Once progress reaches 100% the player exits movement mode and goes into choosing mode (see above).
    */ 
    void MoveVertical(int posOrNegative)
    {
        scaledMovement = (float)UDP_Handling.Y2pos / 105;
        if (posOrNegative * scaledMovement < 0)
        {
            scaledMovement = 0;
        }
        if (progress < 49.5f)
        {
            UDP_Handling.Xtarget = 0;
            UDP_Handling.Ytarget = posOrNegative * 105;
            transform.position = startCoordinates + new Vector3(0, 0, scaledMovement * pathLength * 2);
            progress = Mathf.Abs(scaledMovement * 50);
        }
        else
        {
            UDP_Handling.Xtarget = 0;
            UDP_Handling.Ytarget = 0;
            Camera.main.transform.position = startCoordinates + new Vector3(0, 19.5f, posOrNegative * (1 - Mathf.Abs(scaledMovement)) * pathLength * 2);
            progress = 50 + (1 - Mathf.Abs(scaledMovement)) * 50;
        }
        if (progress > 99.5f)
        {
            ChangeState(false);
        }
    }

    void MoveHorizontal(int posOrNegative)
    {
        scaledMovement = (float)UDP_Handling.X2pos / 150;
        if (posOrNegative * scaledMovement < 0)
        {
            scaledMovement = 0;
        }
        if (progress < 49.5f)
        {
            UDP_Handling.Xtarget = posOrNegative * 150;
            UDP_Handling.Ytarget = 0;
            transform.position = startCoordinates + new Vector3(scaledMovement * pathLength * 2, 0, 0);
            progress = Mathf.Abs(scaledMovement) * 50;
        }
        else
        {
            UDP_Handling.Xtarget = 0;
            UDP_Handling.Ytarget = 0;
            Camera.main.transform.position = startCoordinates + new Vector3(posOrNegative * (1 - Mathf.Abs(scaledMovement)) * pathLength * 2, 19.5f, 0);
            progress = 50 + (1 - Mathf.Abs(scaledMovement)) * 50;
        }
        if (progress > 99.5f)
        {
            ChangeState(false);
        }
    }

    /* 
     * This function returns an integer value of the number of tiles along the path in the direction that the player has chosen.
     * While the next tile is not a wall, it increments the path length then repeats.
    */ 
    int GetPathLength()
    {
        //Counting starts at 1 with the first valid tile

        NextTile();

        //If the first adjacent tile is a wall then it will return 0 for path length 

        Color pixelColour = map.GetPixel(mapX, mapY);
        pathLength = 0;

        // && pathLength < 200 is a safety feature to prevent the game crashing when something goes wrong - saves time when in the editor.
        while (!impassableColours.Contains(pixelColour) && pathLength < 200)
        {
            pathLength++;
            NextTile(); 
            pixelColour = map.GetPixel(mapX, mapY);
        }
        return pathLength;
    }

    /*
     * This function updates the coordinates mapX and mapY to the next tile along the path.
     * We know that the tile the player is currently on is not a wall, so the first tile that is read is the next tile along.
     * If this tile is not a wall, the path has a length of at least 1.
    */ 
    void NextTile()
    {
        switch (direction)
        {
            // Both upper and lowercase characters are allowed for more forgiving programming.
            case ('U'):
            case ('u'):
                mapY++;
                break;
            case ('D'):
            case ('d'):
                mapY--;
                break;
            case ('R'):
            case ('r'):
                mapX++;
                break;
            case ('L'):
            case ('l'):
                mapX--;
                break;
            //if no direction has been set then there has been a coding error, which is notified by a log message
            default:
                Debug.Log("Error, no direcftion set");
                return;
        }
    }

    /*
     * Public direction setter function.
    */
    public void SetDirection(char dir)
    {
        direction = dir;
    }
}
