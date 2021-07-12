using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VISAB_Extraction_API
{
    private Dictionary<string, string> mapping = new Dictionary<string, string>();

    public VISAB_Extraction_API()
    {
        AddItem("Crate", "Prefabs/WeaponsCrate/WeaponsCrate");
        AddItem("Health", "Prefabs/Health");
        AddItem("Environment", null);
        AddItem("Jane Doe/Player", null);
        AddItem("John Doe/Player", null);

    }

    public void AddItem(string gameObjID, string hasToBeInstantiated)
    {
        mapping.Add(gameObjID, hasToBeInstantiated);
    }

    public void AddDict(Dictionary<string, string> inputMap)
    {
        foreach (KeyValuePair<string, string> d in inputMap)
        {
            mapping.Add(d.Key, d.Value);
        }
    }

    public Dictionary<string, string> getDict()
    {
        return mapping;
    }

}
