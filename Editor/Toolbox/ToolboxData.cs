using System.Collections.Generic;
using UnityEngine;

public class ToolboxData : DataObject
{
    public List<string> components = new List<string>();
    [HideInInspector]
    public int index;
    [HideInInspector]
    public bool shouldReimport;
}
