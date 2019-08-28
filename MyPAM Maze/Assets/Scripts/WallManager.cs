using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * This script stores a single instance of impassableColours for each scene (since its attached to the only camera).
 * The colour 0,0,0,1 (black) will always be a wall so will always be impassable.
 * If a scene has barriers, their colours will be added to this list in the script attached to that colour barrier.
*/ 
public class WallManager : MonoBehaviour
{
    public List<Color> impassableColours;

    // Start is called before the first frame update
    void Start()
    {
        impassableColours.Add(new Color(0, 0, 0, 1));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
