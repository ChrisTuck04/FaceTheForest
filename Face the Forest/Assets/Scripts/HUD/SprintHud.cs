using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SprintHud : MonoBehaviour
{

    public Image staminaBackground;
    public GameObject Player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Init stamina bar to default values
        staminaUI.fillAmount = 1f;

        // Find our Player
        Player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // update how much stamina we have
        staminaUI.fillAmount = Player.GetComponent<PlayerMovement>().currentStamina / Player.GetComponent<PlayerMovement>().maxStamina;
        if(staminaUI.fillAmount < 1f)
        {
            // TODO: Make a nicer looking fade
            staminaUI.enabled = true;
            staminaBackground.enabled = true;
        }
        else
        {
            staminaUI.enabled = false;
            staminaBackground.enabled = false;
        }
    }
}
