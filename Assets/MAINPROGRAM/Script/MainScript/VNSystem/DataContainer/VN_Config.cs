using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VN_Config
{
    public static VN_Config instance { get; private set; }

    public static string filePath => $"{FilePaths.root}";
}
