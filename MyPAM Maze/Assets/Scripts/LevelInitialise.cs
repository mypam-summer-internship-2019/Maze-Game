using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInitialise : MonoBehaviour
{

    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        player.transform.position = transform.position + new Vector3(0, 0.49f, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
