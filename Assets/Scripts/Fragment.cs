using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fragment : MonoBehaviour
{
    [HideInInspector] public new Renderer renderer;

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
    }
}
