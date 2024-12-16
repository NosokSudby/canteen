using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateSc : MonoBehaviour
{
    public bool Picked { get; private set; }

    public delegate void PickedEventHandler();
    public event PickedEventHandler OnPicked;

    public void SetPicked()
    {
        Picked = true;
        OnPicked?.Invoke();
    }
}
