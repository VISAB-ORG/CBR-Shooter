using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MinimapCamera : MonoBehaviour
{
    static Camera snapCam = new Camera();

    // default values for texture
    static int resWidth = 256;
    static int resHeight = 256;


    public static void SetupCamera()
    {
        snapCam = GameObject.Find("Snapshot Camera").GetComponent<Camera>();
        if (snapCam.targetTexture == null)
        {
            snapCam.targetTexture = new RenderTexture(resWidth, resHeight, 24);
        }
        else
        {
            resWidth = snapCam.targetTexture.width;
            resHeight = snapCam.targetTexture.height;
        }
        TakeSnapshot();

        snapCam.gameObject.SetActive(false);
    }


    public static void TakeSnapshot()
    {
        snapCam.gameObject.SetActive(true);
        if (snapCam.gameObject.activeInHierarchy)
        {
            Texture2D snapshot = new Texture2D(resWidth, resHeight, TextureFormat.RGB48, false);
            snapCam.Render();
            RenderTexture.active = snapCam.targetTexture;
            snapshot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            byte[] bytes = snapshot.EncodeToPNG();
            string fileName = SnapshotName();
            System.IO.File.WriteAllBytes(fileName, bytes);
            Debug.Log("Snapshot taken");
            snapCam.gameObject.SetActive(false);
        }
    }

    static string SnapshotName()
    {
        return string.Format("{0}/Snapshots/minimap_{1}x{2}_{3}.png", Application.dataPath, resWidth, resHeight, System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }
}
