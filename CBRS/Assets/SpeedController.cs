using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedController : MonoBehaviour
{
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = speed;
    }

}
