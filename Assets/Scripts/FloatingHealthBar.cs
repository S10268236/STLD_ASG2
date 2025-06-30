using UnityEngine;
using UnityEngine.UI;
/*
*Author: Richard Wong Zhi Hui
*Date: 15/6/2025
*Description: Enemy Health Bars 
*/

public class FloatingHealthBar : MonoBehaviour
{
    //Slider input field
    [SerializeField]
    private Slider slider;
    //Camera input field
    [SerializeField]
    Camera HPcamera;
    //Target position transform input field
    [SerializeField]
    private Transform target;
    //How much to push up HealthBar so it is visible
    [SerializeField]
    private Vector3 offset;
    /// <UpdateHealthBar summary>
    /// Make slider show fraction of Max health to show current health
    /// </summary>
    /// <param name="currentValue"></param>
    /// <param name="maxValue"></param>
    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        slider.value = currentValue / maxValue;
    }
    // Update is called once per frame
    void Update()
    {
        //Set the healthbar to always face camera and be in camera's rotation
        transform.rotation = HPcamera.transform.rotation;
        transform.position = target.position + offset;
    }
}
