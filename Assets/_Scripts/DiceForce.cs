using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceForce : MonoBehaviour
{
    Rigidbody rb;
    public int result;
    Vector3 startpos;

    bool diceRolled;
    bool onGround;

    //array of children "side" objects
    public DiceSides[] sides;

    void Start()
    {
        startpos = transform.position;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        diceRolled = false;
        onGround = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Click the mouse to roll the die
        if (Input.GetMouseButtonDown(1))
        {
            Roll();
        }

        //check to see if the die has been rolled and is currently motionless on the ground
        if (rb.IsSleeping() && !onGround && diceRolled)
        {
            onGround = true;
            rb.useGravity = false;
            SideCheck();

        }
        
        //checks to see if the dice is stuck on its edge, re-rolls if this is the case
        if (rb.IsSleeping() && onGround && result == 0)
        {
            ResetDice();
            Roll();

        }
    }

    //drops the die and adds torque to allow it to spin
    void Roll()
    {
        if (!diceRolled && !onGround)
        {
            diceRolled = true;
            rb.useGravity = true;
            rb.AddTorque(Random.Range(100, 600), Random.Range(100, 600), Random.Range(100, 600));
            rb.AddTorque(Random.onUnitSphere * 100);
        }
    }

    //resets die to original position and state, last side that was rolled will remain on top
    void ResetDice()
    {
        transform.position = startpos;
        diceRolled = false;
        onGround = false;
    }

    //checks the collision of the spheres that represent each side *sphere represent opposite sides (side3 represents 4)*
    void SideCheck()
    {
        result = 0;
        foreach (DiceSides side in sides)
        {
            if (side.Grounded())
            {
                result = side.sideValue;
                Debug.Log("Rolled a " + result);
                ResetDice();
            }
        }
    }

}