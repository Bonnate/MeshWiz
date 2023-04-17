using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventReceiverWebGL : MonoBehaviour
{
    public static List<string> _FileNames = new List<string>();

    public void GetFileName(string fileName)
    {
        _FileNames.Clear();

        _FileNames.Add(fileName);
    }

    public void GetFileNames(string fileNames)
    {
        _FileNames.Clear();

        foreach(string fileName in  fileNames.Split(','))
            _FileNames.Add(fileName);
    }    

    private void Clear()
    {
        _FileNames.Clear();
    }
}
