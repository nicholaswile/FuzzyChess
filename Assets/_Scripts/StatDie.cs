using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatDie : MonoBehaviour
{
    Rigidbody rb;
    int result;
     

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("e"))
        {
            rollStat();
        }
    }
    void rollStat()
    {
        result = Random.Range(1, 7);

        if (result == 1)
        {
            Debug.Log("Static 1");
            rb.transform.rotation = Quaternion.Euler(0, 180, -90);
        }
        else if (result == 2)
        {
            Debug.Log("Static 2");
            rb.transform.rotation = Quaternion.Euler(-90f, 180, -90);
        }
        else if (result == 3)
        {
            Debug.Log("Static 3");
            rb.transform.rotation = Quaternion.Euler(0, 90, -90);
        }
        else if (result == 4)
        {
            Debug.Log("Static 4");
            rb.transform.rotation = Quaternion.Euler(0, -90, -90);
        }
        else if (result == 5)
        {
            Debug.Log("Static 5");
            rb.transform.rotation = Quaternion.Euler(90f, 0, 90);
        }
        else if (result == 6)
        {
            Debug.Log("Static 6");
            rb.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
    }
}
