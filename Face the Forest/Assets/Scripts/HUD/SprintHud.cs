using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SprintHud : MonoBehaviour
{
    public float totalStamina = 1f;

    public Image staminaUI;
    public float currentStamina;
    public GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Init stamina bar to default values
        staminaUI.fillAmount = 1f;
        currentStamina = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        // If we're running, update the stamina bar.
        // TODO: Tell movement to add: player.GetCompontent<PlayerMovement>().running == true
        if(true)
        {
            staminaUI.fillAmount = currentStamina;
        }
    }
}
