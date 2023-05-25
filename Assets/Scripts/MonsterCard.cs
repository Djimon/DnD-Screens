using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Monster", menuName = "Cards/Monster", order = 1)]
public class MonsterCard : ScriptableObject
{
    public string title;
    public string icon;
    public string backgroundImage;
    public List<MonsterProperty> properties = new List<MonsterProperty>();
}

[System.Serializable]
public class MonsterProperty
{
    public string name;
    public string value;
    public string category;
}