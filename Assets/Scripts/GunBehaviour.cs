using UnityEngine;
/*
*Author: Richard Wong Zhi Hui
*Date: 15/6/2025
*Description: Gun collection and Audio
*/
public class GunBehaviour : MonoBehaviour
{
    //Audioclip input field
    [SerializeField]
    AudioClip GunPickUpClip;
    //Audio position input field
    [SerializeField]
    Transform SoundPosition;
    public void Collect(PlayerBehaviour player)
    {
        AudioSource.PlayClipAtPoint(GunPickUpClip, SoundPosition.position);
        Destroy(gameObject);
    }
}
