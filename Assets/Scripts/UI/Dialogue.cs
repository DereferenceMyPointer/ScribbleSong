using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "UI/Dialogue")]
public class Dialogue : ScriptableObject
{
    public string[] sentences;
    public new string name;
}
