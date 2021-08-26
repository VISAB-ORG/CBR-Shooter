using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedController : MonoBehaviour
{
    public float speed = 1;

    public static float Speed { get; private set; }

    void Awake()
    {
        Time.timeScale = speed;
        Speed = speed;
    }

}
