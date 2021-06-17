using Assets.Scripts.VISAB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VISABController : MonoBehaviour
{
    public string hostAdress = "http://localhost";
    public int port = 2673;
    public int requestTimeout = 1;

    // Start is called before the first frame update
    void Awake()
    {
        VISABHelper.HostAdress = hostAdress;
        VISABHelper.Port = port;
        VISABHelper.RequestTimeout = requestTimeout;
    }

}
