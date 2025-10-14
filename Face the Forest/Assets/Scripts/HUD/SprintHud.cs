using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SprintHud : MonoBehaviour
{

    public Image staminaBar;
    public Image staminaBackground;
    public GameObject Player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Init stamina bar to default values
        staminaBar.fillAmount = 1f;

        // Find our Player
        Player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // update how much stamina we have
        staminaBar.fillAmount = Player.GetComponent<PlayerMovement>().currentStamina / Player.GetComponent<PlayerMovement>().maxStamina;
        if(staminaBar.fillAmount < 1f)
        {
            // TODO: Make a nicer looking fade
            staminaBar.enabled = true;
            staminaBackground.enabled = true;
        }
        else
        {
            staminaBar.enabled = false;
            staminaBackground.enabled = false;
        }
    }
}
