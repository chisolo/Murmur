using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialArchive : IArchive
{
    public List<string> completes;
    public void Default()
    {
        completes = new List<string>();
    }
    public void SetData(string tutorial)
    {
        completes.Add(tutorial);
    }
    public bool IsComplete(string tutorial)
    {
        return completes.Contains(tutorial);
    }
}
