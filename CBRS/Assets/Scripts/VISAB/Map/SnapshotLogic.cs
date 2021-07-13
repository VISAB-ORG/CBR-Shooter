using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MinimapExtensionMethods
{
    public static Bounds GetBoundsWithChildren(this GameObject gameObject)
    {
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();

        Bounds bounds = renderers.Length > 0 ? renderers[0].bounds : new Bounds();

        for (int i = 1; i < renderers.Length; i++)
        {
            if (renderers[i].enabled)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }
        }
        return bounds;
    }
    public static void FocusOn(this Camera cam, GameObject focusedObject, float marginPercentage, Vector3 rotationAngle)
    {
        Bounds bounds = focusedObject.GetBoundsWithChildren();
        float maxExtent = bounds.extents.magnitude;
        float minDistance = (maxExtent * marginPercentage) / Mathf.Sin(Mathf.Deg2Rad * cam.fieldOfView / 0.5f);
        cam.transform.position = bounds.center + Vector3.up * minDistance;
        cam.transform.Rotate(rotationAngle.x, rotationAngle.y, rotationAngle.z, Space.Self);

        Debug.Log("GameObj: " + focusedObject + ", coordinates: " + focusedObject.transform.position);
        Debug.Log("Camera: " + cam + ", coordinates: " + cam.transform.position);
        cam.nearClipPlane = minDistance - maxExtent;
        Debug.Log(bounds);
    }

    public static void CenterOn(this Camera cam, GameObject obj)
    {
        Bounds b = obj.GetBoundsWithChildren();
        Vector3 v = obj.transform.position;

        cam.transform.position = v + (Vector3.up*10);
        Debug.Log(cam.transform.position);
        //cam.transform.LookAt(obj.transform.position);
    }

    public static string SnapshotName(int width, int height)
    {
        return string.Format("{0}/Snapshots/minimap_{1}x{2}_{3}.png", Application.dataPath, width, height, System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }

}

