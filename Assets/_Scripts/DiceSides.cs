using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceSides : MonoBehaviour
{
   bool grounded;
   public int sideValue;

    //marks the collision of the spheres that represent the sides of the die
    void OnTriggerStay(Collider collider)
    {
        if(collider.transform.tag == "floor")
        {
            grounded = true;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.transform.tag == "floor")
        {
            grounded = false;
        }
    }

    public bool Grounded()
    {
        return grounded;
    }
}
