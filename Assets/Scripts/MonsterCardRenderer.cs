using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterCardRenderer : MonoBehaviour
{
    public Text titleText;
    public Image backgroundImage;
    public Transform propertiesContainer;
    public GameObject propertyPrefab;
    public ScriptableObject Monster;

    public void RenderMonsterCard(MonsterCard monsterData)
    {
        titleText.text = monsterData.title;
        // Load and set the background image

        // Clear previous properties
        foreach (Transform child in propertiesContainer)
        {
            Destroy(child.gameObject);
        }

        // Render properties
        foreach (MonsterProperty propertyData in monsterData.properties)
        {
            GameObject propertyObject = Instantiate(propertyPrefab, propertiesContainer);
            Text propertyNameText = propertyObject.GetComponentInChildren<Text>();
            Text propertyValueText = propertyObject.transform.GetChild(1).GetComponent<Text>();

            propertyNameText.text = propertyData.name;
            propertyValueText.text = propertyData.value;

            // Remove HTML artifacts from property values
            propertyValueText.text = RemoveHtmlTags(propertyValueText.text);
        }
    }

    // Function to remove HTML tags using regular expressions
    private string RemoveHtmlTags(string input)
    {
        return System.Text.RegularExpressions.Regex.Replace(input, "<.*?>", string.Empty);
    }
}
