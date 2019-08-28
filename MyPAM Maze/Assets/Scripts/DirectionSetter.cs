using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DirectionSetter : MonoBehaviour
{
    public float rotationSpeed;
    public Transform pointerTransform;
    public float deltaPointerPosition;
    public float pointerPosition;

    public Image[] imageArray = new Image[4];
    public Color defaultColour;
    public Color highlightedColour;

    public GameObject player;

    public System.DateTime timeAtClick;
    public double totalTime;


    private void Start()
    {
        player = GameObject.Find("Player");
    }

    void OnEnable()
    {
        pointerTransform = gameObject.transform.Find("Pointer");
        pointerTransform.eulerAngles = new Vector3(0, 0, 0);

        // Right/Down/Left/Right corresponds to 0/1/2/3 as a clockwise rotation.
        // Up is last as its condition is more difficult, so has been assigned to the else condition {see end of FixedUpdate()}.
        imageArray[0] = GameObject.Find("Right").GetComponent<Image>();
        imageArray[1] = GameObject.Find("Down").GetComponent<Image>();
        imageArray[2] = GameObject.Find("Left").GetComponent<Image>();
        imageArray[3] = GameObject.Find("Up").GetComponent<Image>();
    }


    void FixedUpdate()
    {

        // transform.eulerAngles is actually always 0 to 360 even if the editor shows it going above 360 or negative.
        pointerPosition = pointerTransform.eulerAngles.z;

        // Increasingly negative rotation is clockwise, rotation speed is degrees per second.
        deltaPointerPosition = -Time.deltaTime * rotationSpeed;
        pointerTransform.eulerAngles += new Vector3(0, 0, deltaPointerPosition);

        // Resets the pointer position every 360 degrees so the float stays within the limits 0-360, making conditions easier to write.
        if (pointerPosition < 0  )
        {
            pointerTransform.eulerAngles = new Vector3 (0,0,360);
        }

        // Right
        if (pointerPosition < 315 && pointerPosition > 225)
        {
            SetPointer(0,'R');
        }
        // Down
        else if (pointerPosition < 225 && pointerPosition > 135) 
        {
            SetPointer(1,'D');
        }
        // Left
        else if (pointerPosition < 135 && pointerPosition > 45)
        {
            SetPointer(2,'L');
        }
        // Up 
        // This covers the case for both <45 and >315 at the same time
        else
        {
            SetPointer(3,'U');
        }
    }

    /*
     * This function highlights the direction arrow that is being pointed at and will request a click check using the CheckForClick() function.
     * If a click is returned as true, it forwards the direction to the PlayerMovement script and sets the player to Moving Mode.
    */
    void SetPointer(int numberToHighlight, char direction)
    {
        foreach (Image image in imageArray)
        {
            image.color = defaultColour;
        }
        imageArray[numberToHighlight].color = highlightedColour;

        if (CheckForClick() == true)
        {
            player.GetComponent<PlayerMovement>().SetDirection(direction);
            player.GetComponent<PlayerMovement>().ChangeState(true);
        }
    }
   
    /*
     * This function calculates the time between any mouse button being pressed down for the first frame and any button being released for the first frame.
     * If the time is greater than 5 milliseconds (if there has been an input) and less than 700 milliseconds, then it has been a click and not a hold.
     * This means if the button is held down (to undo a move) then it won't try to start a new move first.
     * Therefore a click has been differentiated from a hold.
    */
    bool CheckForClick()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            timeAtClick = System.DateTime.Now;
        }
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
        {
            totalTime = (System.DateTime.Now - timeAtClick).TotalMilliseconds;
        }

        if (totalTime < 700 && totalTime > 5)
        {
            totalTime = 0;
            return (true);
        }
        else
        {
            return (false);
        }
    }
}
