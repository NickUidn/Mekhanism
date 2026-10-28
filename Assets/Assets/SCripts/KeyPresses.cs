using System;
using UnityEngine;

public  class KeyPresses : MonoBehaviour
{
    public event Action OnEscapePress;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnEscapePress?.Invoke();
        }
    }
}
