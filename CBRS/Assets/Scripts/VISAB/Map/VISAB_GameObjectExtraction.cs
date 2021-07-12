using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VISAB_GameObjectExtraction : MonoBehaviour
{
    public Vector3 spawnPoint = new Vector3();
    public Camera minimapCam = new Camera();
    static int resWidth = 1024;
    static int resHeight = 1024;

    private bool flag = false;

    private List<GameObject> extractObjects;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(ExtractionRoutine());
        StartCoroutine(MappingExtractionRoutine());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SnapshotObject(GameObject obj)
    {
        minimapCam.FocusOn(obj, 2f);

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
            Debug.Log("Snapshot taken");

        }
    }

    IEnumerator MappingExtractionRoutine()
    {

        if (!flag)
        {
            yield return new WaitForSeconds(4);

            spawnPoint = GameObject.Find("SnapSpawn").transform.position;

            int oldMask = minimapCam.cullingMask;

            VISAB_Extraction_API mappedGameObj = new VISAB_Extraction_API();

            minimapCam = GameObject.Find("Minimap Camera").GetComponent<Camera>();
            minimapCam.gameObject.SetActive(true);

            foreach (KeyValuePair<string, string> dic in mappedGameObj.getDict())
            {
                yield return new WaitForSeconds(2);

                if (dic.Value != null)
                {
                    var instObj = Resources.Load(dic.Value) as GameObject;
                    Instantiate(instObj, spawnPoint, Quaternion.identity);

                    instObj.layer = LayerMask.NameToLayer("Shootable");

                    minimapCam.orthographic = false;
                    minimapCam.cullingMask = 1 << LayerMask.NameToLayer("Shootable");


                    SnapshotObject(instObj);
                    yield return new WaitForSeconds(1);
                    instObj.SetActive(false);
                }
                else
                {
                    var spawnedObj = GameObject.Find(dic.Key);

                    minimapCam.orthographic = true;

                    if (spawnedObj.name.Contains("Player"))
                    {
                        minimapCam.orthographic = false;
                        minimapCam.cullingMask = 1 << LayerMask.NameToLayer("Shootable");
                    }

                    SnapshotObject(spawnedObj);
                    minimapCam.cullingMask = oldMask;
                }
            }
            flag = true;
        }


    }

}
