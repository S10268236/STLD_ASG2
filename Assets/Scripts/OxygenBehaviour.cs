using UnityEngine;
/*
*Author: Richard Wong Zhi Hui
*Date: 15/6/2025
*Description: Oxygen collection and Audio
*/
public class OxygenBehaviour : MonoBehaviour
{
    //Audioclip input field
    [SerializeField]
    AudioClip OxygenCollectSound;
    //Audio Location input field
    [SerializeField]
    Transform SoundLocation;

    public void Collect(PlayerBehaviour player)
    {
        //Debug.Log("Collected Oxygen");
        //Plays audio at location even when object destroyed
        AudioSource.PlayClipAtPoint(OxygenCollectSound, SoundLocation.position);
        Destroy(gameObject);
    }
}
