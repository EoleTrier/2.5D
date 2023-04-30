using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Mechanism : MonoBehaviour
{
    public float timer;
    public bool playOnce;
    [HideInInspector] public float myTimer;
    [HideInInspector] public bool IsActive;
    public List<Interactibles> m_InteractibleList;

    private void Start()
    {
        IsActive = Time.time <= 0;
    }
}