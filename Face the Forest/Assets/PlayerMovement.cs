using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public Rigidbody rb;
    public float forwardForce;
    public float sidewaysForce;
    public bool moveRight = false;
    public bool moveLeft = false;
    public bool moveForward = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("d"))
        {
            moveLeft = true;
        }
        if (Input.GetKeyUp("d"))
        {
            moveLeft = false;
        }
        if (Input.GetKey("a"))
        {
            moveRight = true;
        }
        if (Input.GetKeyUp("a"))
        {
            moveRight = false;
        }
        if (Input.GetKey("w"))
        {
            moveForward = true;
        }
        if (Input.GetKeyUp("w"))
        {
            moveForward = false;
        }
    }

    void FixedUpdate() // FixedUpdate is used for physics updates
    {

        if (moveRight == true) //Look into supporting smoothing, ability to change keybinds, etc.
        {
            // Add a right force
            rb.AddForce(-sidewaysForce * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
        }

        if (moveLeft == true)
        {
            // Add a left force
            rb.AddForce(sidewaysForce * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
        }
        
        if (moveForward == true)
        {
            // Add a forward force
            rb.AddForce(0, 0, forwardForce * Time.deltaTime, ForceMode.VelocityChange);
        }
    }
}
