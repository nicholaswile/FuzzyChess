using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface InputHandler 
{
    void ProcessInput(Vector3 inputPosition, GameObject selectedObject, Action callback);
}
