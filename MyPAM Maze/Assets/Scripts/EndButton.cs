using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class EndButton : MonoBehaviour
{
    public GameObject player;

    public Scene currentScene;
    public int currentLevel;

    public float timer;
    public  GameObject timerText;
    public bool freezeTimer;

    public  GameObject levelCompleteUI;
    public  GameObject directionSetterUI;

    /*
     * When the level loads this script needs to work out which level is active and initialise certain GameObjects. 
     * Then it makes them active or not depending on which should be seen from the start and which should be hidden until the end panel is reached.
    */
    void Start()
    {
        directionSetterUI = GameObject.Find("Direction Selector");
        timerText = GameObject.Find("Timer");
        levelCompleteUI = GameObject.Find("Level Complete");
        player = GameObject.Find("Player");

        timer = 0;
        freezeTimer = false;

        currentScene = SceneManager.GetActiveScene();
        // All game levels are called "Level n" where n is the number of the level (currently it's the same as the build index).
        currentLevel = int.Parse(currentScene.name.Split(' ')[1]);

        directionSetterUI.SetActive(true);
        timerText.SetActive(true);
        levelCompleteUI.SetActive(false);
    }

    /*
     * This script needs to constantly increase the timer. If the timer is "frozen" then its actually only the display that is frozen, so it displays the time before being frozen.
     * This time is formatted into minutes, seconds, milliseconds and then displayed by a Text UI object.
     * If the timer is frozen, that means the level has been completed and the timer has been set to 0. The text object continues to display the last time before freezing.
     * It will continue to increase until it reaches 3 seconds, after which it will call the NextLevel() function.
    */ 
    void Update()
    {
        timer += Time.deltaTime;
        if (!freezeTimer)
        {
            int minutes = (int)timer / 60;
            int seconds = (int)timer % 60;
            int milliseconds = (int)(timer * 1000) % 1000;
            timerText.GetComponent<TMP_Text>().text = string.Format("{0}:{1:00}:{2:00}", minutes, seconds, milliseconds);
        }
        else
        {
            directionSetterUI.SetActive(false);

            if (timer > 3)
            {
                NextLevel();
            }
        }
    }

    /*
     * When the player has reached the end panel it enters the box collider and this function is called.
     * This freezes the timer text, sets the time to 0, and enables appropriate UI objects.
    */
    private void OnTriggerEnter(Collider other)
    {
        freezeTimer = true;
        directionSetterUI.SetActive(false);
        levelCompleteUI.SetActive(true);
        timer = 0;
    }

    /*
     * This function takes the current level number, increments it, then loads the level corresponding to the new value.
    */
    void NextLevel()
    {
        currentLevel++;
        string nextLevelName = "Level " + (currentLevel).ToString();
        SceneManager.LoadScene(nextLevelName);
    }

}
