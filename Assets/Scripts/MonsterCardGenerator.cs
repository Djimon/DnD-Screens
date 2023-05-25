using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Text.RegularExpressions;

[System.Serializable]
public class MonsterData
{
    public string title;
    public string icon;
    public string[] contents;
    public string background_image;
}

[System.Serializable]
public class MonsterWrapper
{
    public MonsterData[] monsters;
}

public class MonsterCardGenerator : MonoBehaviour
{
    public TextAsset monsterJson; // Reference to the JSON file containing the monster data
    MonsterData[] monsters;

    private void Start()
    {
        Debug.Log("Start");
        // Parse the JSON data
        MonsterWrapper wrapper = JsonUtility.FromJson<MonsterWrapper>(monsterJson.text);
        monsters = wrapper.monsters;
        Debug.Log("JSON: " + monsters.Length);

        // Generate monster cards
        foreach (MonsterData monster in monsters)
        {        
            // Create a new ScriptableObject for the monster card
            MonsterCard card = ScriptableObject.CreateInstance<MonsterCard>();
            card.title = monster.title;
            card.icon = monster.icon;
            card.backgroundImage = monster.background_image;

            string propertyCategory = "";
            string propertyName;
            string propertyValue;

            // Process the contents of the monster card
            foreach (string content in monster.contents)
            {
                string[] contentParts = content.Split('|');
                

                //extra property for each Ability
                if (contentParts.Length >= 6 && contentParts[0].Trim() == "dndstats")
                {
                    propertyCategory = "abilities";

                    Dictionary<int, string> propertyNameMap = new Dictionary<int, string>()
                    {
                        { 1, "STR" },
                        { 2, "DEX" },
                        { 3, "CON" },
                        { 4, "INT" },
                        { 5, "WIS" },
                        { 6, "CHA" }
                    };

                    for (int i = 1; i <= 6; i++)
                    {
                        propertyName = propertyNameMap.ContainsKey(i) ? propertyNameMap[i] : "ERR";
                        propertyValue = contentParts[i].Trim();
                        //Debug.LogWarning("ability " + propertyName + ": " +propertyValue);
                        AddProperty(card, propertyCategory, propertyName, propertyValue);
                    }

                }
                else if (contentParts.Length > 1 && contentParts[0].Trim() == "section")
                {
                    propertyCategory = contentParts[1].Trim();
                }
                else if(contentParts.Length > 1 && contentParts[0].Trim() == "text")
                { 
                    string[] elements = contentParts[1].Split(new string[] { "</p>" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string element in elements)
                    {
                        string trimmedElement = element;
                        int startIndex = trimmedElement.IndexOf("<em>") + 4;
                        int endIndex = trimmedElement.IndexOf("</em>");
                        //Debug.LogWarning("Indices: " +startIndex +":" + endIndex);
                        propertyName = (startIndex == -1 || endIndex == -1 ? "other": trimmedElement.Substring(startIndex, endIndex - startIndex));

                        startIndex = trimmedElement.IndexOf("</em>") + 5;
                        propertyValue = trimmedElement.Substring(startIndex);

                        if (string.IsNullOrEmpty(propertyName))
                        {
                            propertyName = propertyCategory;
                        }

                        // Process the propertyName and propertyValue as needed
                        // Remove HTML artifacts from the property values
                        propertyCategory = RemoveHtmlTags(propertyCategory);
                        propertyName = RemoveHtmlTags(propertyName);
                        propertyValue = RemoveHtmlTags(propertyValue);

                        AddProperty(card, propertyCategory, propertyName, propertyValue);
                    }
                } else if (contentParts.Length >= 2)
                {
                    int contentLength = contentParts.Length;
                    propertyCategory = "general";
                    propertyName = contentParts[0].Trim();
                    propertyValue = contentParts[1].Trim();

                    if (propertyName == "property")
                    {
                        propertyCategory = "stats";
                        propertyName = contentParts[1].Trim();
                        propertyValue = contentParts[2].Trim(); 
                    }

                    AddProperty(card, propertyCategory, propertyName, propertyValue);
                }
            }

            // Optionally, you can save the ScriptableObject asset
            // Save the ScriptableObject asset as a file
            string assetPath = "Assets/content/monster/" + monster.title + ".asset";
            SaveScriptableObjectAsset(card, assetPath);

            // Do something with the generated monster card
            //Debug.Log("Generated monster card: " + card.title);

            // Access the properties of the monster card
            //foreach (var property in card.properties)
            //{
            //    Debug.Log("Property: " + property.name + ", Value: " + property.value);
            //}
        }
    }

    private static void AddProperty(MonsterCard card, string propertyCategory, string propertyName, string propertyValue)
    {
        // Create a new MonsterProperty object
        MonsterProperty property = new MonsterProperty();
        property.name = propertyName;
        property.value = propertyValue;
        property.category = propertyCategory;

        // Add the property to the monster card's list of properties
        card.properties.Add(property);
    }

    private void SaveScriptableObjectAsset(ScriptableObject scriptableObject, string assetPath)
    {
        // Create a new asset file at the specified path
        AssetDatabase.CreateAsset(scriptableObject, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    // Function to remove HTML tags using regular expressions
    string RemoveHtmlTags(string input)
    {
        return Regex.Replace(input, "<.*?>", string.Empty);
    }
}
