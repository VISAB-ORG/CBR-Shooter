using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScript : MonoBehaviour
{
    public bool isActive;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var spectatorCamera = GameObject.Find("SpectatorCamera").GetComponent<Camera>();
        if (isActive && spectatorCamera != null)
        {
            var worldPosition = spectatorCamera.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log($"Mouse is at {worldPosition}");
        }
    }
}
