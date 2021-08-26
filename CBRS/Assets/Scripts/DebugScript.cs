using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScript : MonoBehaviour
{
    public bool isActive;

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
