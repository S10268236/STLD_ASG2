using UnityEngine;
/*
*Author: Richard Wong Zhi Hui
*Date: 15/6/2025
*Description: Door opening and closing
*/
public class DoorBehaviour : MonoBehaviour
{
    /// <summary>
    /// Opening and closing door
    /// </summary>

    //Set default state of door to be closed
    public bool Closed = true;
    //Variable for vectors for when closed
    private Vector3 closedRotation;
    //Variable for vectors for when open
    private Vector3 openRotation;
    AudioSource doorOpenAudio;

    void Start()
    {
        //Closed vectors
        closedRotation = transform.eulerAngles;
        //Add 90 degrees rotation to open door
        openRotation = closedRotation + new Vector3(0, 90, 0);
        doorOpenAudio = GetComponent<AudioSource>();
    }
    public void Interact()
    {
        //Check whether Door is open or closed
        if (Closed)
        {
            //Play Open audio
            doorOpenAudio.Play();
            //Set transform vectors to the open vectors
            transform.eulerAngles = openRotation;
            Closed = false;
        }
        else
        {
            //Set transform vectors to the closed vectors
            transform.eulerAngles = closedRotation;
            Closed = true;
        }
    }
}
