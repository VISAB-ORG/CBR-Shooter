using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TestScript : MonoBehaviour
{
    public Camera minimapCam = new Camera();
    static int resWidth = 1024;
    static int resHeight = 1024;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            //var scene = GameObject.Find("Player/Player");
            //var focusedObj = GameObject.Find("Environment");
            //var focusedObj = GameObject.Find("John Doe/Player");
            var focusedObj = GameObject.Find("Jane Doe/Player");

            if (focusedObj.gameObject.name.Contains("Player"))
            {
                minimapCam.orthographic = false;
                minimapCam.cullingMask = 1 << LayerMask.NameToLayer("Shootable");
            }


            minimapCam = GameObject.Find("Minimap Camera").GetComponent<Camera>();
            minimapCam.gameObject.SetActive(true);
            minimapCam.FocusOn(focusedObj, 2f);
            //minimapCam.CenterOn(scene);
            //minimapCam.transform.LookAt(scene.transform);
            Debug.Log(focusedObj.transform);

            if (minimapCam.targetTexture == null)
            {
                minimapCam.targetTexture = new RenderTexture(resWidth, resHeight, 24);
            }
            else
            {
                resWidth = minimapCam.targetTexture.width;
                resHeight = minimapCam.targetTexture.height;
            }

            

            if (minimapCam.gameObject.activeInHierarchy)
            {
                Texture2D snapshot = new Texture2D(resWidth, resHeight, TextureFormat.ARGB32, false);
                minimapCam.Render();
                RenderTexture.active = minimapCam.targetTexture;
                snapshot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
                byte[] bytes = snapshot.EncodeToPNG();
                string fileName = MinimapExtensionMethods.SnapshotName(resWidth, resHeight);
                System.IO.File.WriteAllBytes(fileName, bytes);
                Debug.Log("Minimap Snapshot taken");
                minimapCam.gameObject.SetActive(false);
            }
        }
    }

   
}
