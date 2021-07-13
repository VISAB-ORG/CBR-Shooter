using Assets.Scripts.VISAB.Map;
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
        StartCoroutine(MappingExtractionRoutine());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SnapshotObject(GameObject obj, float offset, float rotation, int width, int height)
    {
        minimapCam.FocusOn(obj, offset, rotation);

        if (minimapCam.targetTexture == null)
        {
            minimapCam.targetTexture = new RenderTexture(width, height, 24);
        }
        else
        {
            resWidth = minimapCam.targetTexture.width;
            resHeight = minimapCam.targetTexture.height;
        }

        if (minimapCam.gameObject.activeInHierarchy)
        {
            Texture2D snapshot = new Texture2D(width, height, TextureFormat.ARGB32, false);
            minimapCam.Render();
            RenderTexture.active = minimapCam.targetTexture;
            snapshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            byte[] bytes = snapshot.EncodeToPNG();
            string fileName = MinimapExtensionMethods.SnapshotName(resWidth, resHeight);
            System.IO.File.WriteAllBytes(fileName, bytes);
            Debug.Log(obj.name + "Snapshot taken");

        }
    }

    public IEnumerator SnapInstantiatedObj(Settings obj)
    {
        yield return new WaitForSeconds(2);
        int oldMask = minimapCam.cullingMask;
        spawnPoint = GameObject.Find("SnapSpawn").transform.position;

        minimapCam = GameObject.Find("Minimap Camera").GetComponent<Camera>();
        minimapCam.gameObject.SetActive(true);

        var loadedObj = Resources.Load(obj.PrefabPath) as GameObject;
        loadedObj = Instantiate(loadedObj, spawnPoint, Quaternion.identity);

        loadedObj.SetActive(true);

        loadedObj.layer = LayerMask.NameToLayer("Shootable");

        minimapCam.orthographic = false;
        minimapCam.cullingMask = 1 << LayerMask.NameToLayer("Shootable");

        SnapshotObject(loadedObj, obj.CamOffset, obj.Rotation, obj.SizeWidth, obj.SizeHeight);
        yield return new WaitForSeconds(1);
        loadedObj.SetActive(false);

    }

    public IEnumerator SnapExistingObj(Settings obj)
    {
        yield return new WaitForSeconds(2);
        int oldMask = minimapCam.cullingMask;

        var spawnedObj = GameObject.Find(obj.GameObjectID);

        minimapCam.orthographic = true;

        if (spawnedObj.name.Contains("Player"))
        {
            minimapCam.orthographic = false;
            minimapCam.cullingMask = 1 << LayerMask.NameToLayer("Shootable");
        }

        SnapshotObject(spawnedObj, obj.CamOffset, obj.Rotation, obj.SizeWidth, obj.SizeHeight);

        minimapCam.cullingMask = oldMask;
    }

    IEnumerator MappingExtractionRoutine()
    {

        if (!flag)
        {
            yield return new WaitForSeconds(4);

            spawnPoint = GameObject.Find("SnapSpawn").transform.position;

            Debug.Log("Location" + spawnPoint);

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
                    instObj = Instantiate(instObj, spawnPoint, Quaternion.identity);

                    instObj.SetActive(true);

                    instObj.layer = LayerMask.NameToLayer("Shootable");

                    minimapCam.orthographic = false;
                    minimapCam.cullingMask = 1 << LayerMask.NameToLayer("Shootable");


                    SnapshotObject(instObj, 2f, 45f, 1024, 1024);
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

                    SnapshotObject(spawnedObj, 2f, 45f, 1024, 1024);
                    minimapCam.cullingMask = oldMask;
                }
            }
            flag = true;
            Debug.Log("Job Done. Numbers of GameObjects snapshotted: " + mappedGameObj.getDict().Count);
        }


    }

}
