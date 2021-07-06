using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VISAB_Extraction_API
{
    private static Dictionary<string, string> mapping = new Dictionary<string, string>();

    public static void AddItem(string gameObjID, string hasToBeInstantiated)
    {
        mapping.Add(gameObjID, hasToBeInstantiated);
    }

    public static void AddDict(Dictionary<string, string> inputMap)
    {
        foreach (KeyValuePair<string, string> d in inputMap)
        {
            mapping.Add(d.Key, d.Value);
        }
    }

    public static Dictionary<string, string> getDict()
    {
        return mapping;
    }
}
