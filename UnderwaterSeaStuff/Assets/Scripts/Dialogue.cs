using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Talky
{
    public string name;
    [TextArea(3, 10)]
    public string[] sentences;
}