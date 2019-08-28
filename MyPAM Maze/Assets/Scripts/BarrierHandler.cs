using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierHandler : MonoBehaviour
{
    public GameObject barrier;
    public string colour;
    public GameObject player;
    public List<Color> impassableColours;

    public float timer;
    public bool runTimer;
    public Vector3 barrierPosition;
 
    /*
     * When the level is loaded the barriers are up, they are not in the process of lowering, the colour of this barrier needs to be added to the list of impassable colours,
     * the corresponding barrier needs to be found, and their initial transform.position needs to be stored.
    */
    void Start()
    {
        timer = 0;
        runTimer = false;
        player = GameObject.Find("Player");

        colour = gameObject.name.Split(' ')[0];
        barrier = GameObject.Find(colour + " Barrier(Clone)");
        barrier.SetActive(true);
        barrierPosition = barrier.transform.position;

        Camera.main.GetComponent<WallManager>().impassableColours.Add(gameObject.GetComponent<Renderer>().material.color);
    }

    /* 
     * If the timer is running then the barrier is lowered (total time is 2 seconds).
     * When the time is past 2 seconds the object is removed and the corresponding colour is removed from the list of impassable colours.
    */
    void Update()
    {
        if (runTimer)
        {
            timer += Time.deltaTime;
            barrier.transform.position -= new Vector3(0, Time.deltaTime, 0);
        }
        if (timer > 2f)
        {
            barrier.SetActive(false);
            Camera.main.GetComponent<WallManager>().impassableColours.Remove(gameObject.GetComponent<Renderer>().material.color);
        }

    }

    /*
     * When the button is "pressed" it activates the timer, which leads to the barrier disapearing and the colour no longer being impassable#
    */
    private void OnTriggerEnter(Collider other)
    {
        runTimer = true;
    }
}
