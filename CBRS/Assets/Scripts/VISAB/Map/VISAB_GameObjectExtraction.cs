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
        StartCoroutine(ExtractionRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator ExtractionRoutine()
    {

        if (!flag)
        {
            yield return new WaitForSeconds(4);

            spawnPoint = GameObject.Find("SnapSpawn").transform.position;

            int oldMask = minimapCam.cullingMask;

            var visabObj = GameObject.FindGameObjectsWithTag("VISAB");

            var healthObj = Resources.Load("Prefabs/Health") as GameObject;

            healthObj = Instantiate(healthObj, spawnPoint, Quaternion.identity);

            var players = GameObject.FindGameObjectsWithTag("Player");

            //extractObjects = new List<GameObject>();

            foreach (GameObject p in players)
            {
                var p_child = p.transform.Find("Player").gameObject;
                extractObjects.Add(p_child);
            }

            extractObjects.AddRange(visabObj);

            extractObjects.Add(healthObj);

            Debug.Log("List length:" + extractObjects.Count);

            minimapCam = GameObject.Find("Minimap Camera").GetComponent<Camera>();
            minimapCam.gameObject.SetActive(true);

            foreach (GameObject obj in extractObjects)
            {
                minimapCam.orthographic = true;

                if (obj.name.Contains("Player"))
                {
                    minimapCam.orthographic = false;
                    minimapCam.cullingMask = 1 << LayerMask.NameToLayer("Shootable");
                }

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
                yield return new WaitForSeconds(3);
                // reset camera adjustments
                minimapCam.cullingMask = oldMask;
                
            }
            // Extraction done
            flag = true;
            Debug.Log("Everything done. In Total " + extractObjects.Count + " GameObjects were snapshotted");
            minimapCam.gameObject.SetActive(false);
        }
    }
}
