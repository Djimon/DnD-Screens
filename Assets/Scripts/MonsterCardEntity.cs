using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MonsterCardEntity : MonoBehaviour
{
    [Header("Images & Icons")]
    public Image image;
    public Image AC_iocn;
    public Image HP_iocn;
    public Image speed_iocn;
    public Image speedFly_iocn;
    public Image speedSwim_iocn;
    [Space(5)]
    //basic information
    [Header("Basic information")]
    public TextMeshProUGUI title;
    public TextMeshProUGUI subtitle;
    public TextMeshProUGUI armorClass;
    public TextMeshProUGUI hitPoints;
    public TextMeshProUGUI speedWalk;

    [Space(5)]
   //abilities
    [Header("Ability Scores")]
    public TextMeshProUGUI STR;
    public TextMeshProUGUI STR_amount;
    public TextMeshProUGUI DEX;
    public TextMeshProUGUI DEX_amount;
    public TextMeshProUGUI CON;
    public TextMeshProUGUI CON_amount;
    public TextMeshProUGUI INT;
    public TextMeshProUGUI INT_amount;
    public TextMeshProUGUI WIS;
    public TextMeshProUGUI WIS_amount;
    public TextMeshProUGUI CHA;
    public TextMeshProUGUI CHA_amount;
    [Space(5)]
    //properties
    [Header("Properties")]
    

    [Space(5)]
    //features and actions
    [Header("Boni,Features, Traits and Actions")]
    public TextMeshProUGUI allStatsAndBoni;
    public TextMeshProUGUI allFeatures; //including traits
    public TextMeshProUGUI allActions; //inlcuding Legendary Acions


    private MonsterCard monster;

    private string imageUrl;
    private string imageFileName;
    private string imageFolderPath= "Assets/web/images";

    private string localImagePath;


    private void Start()
    {
        if(monster != null)
        {
            ChangeMonster(monster);
        }
    }

    public void ChangeMonster(MonsterCard _monster)
    {
        monster = _monster;

        imageUrl = _monster.backgroundImage;
        imageFileName = _monster.title + ".png";
        localImagePath = Path.Combine(imageFolderPath, imageFileName);
        
        Debug.Log("Titel: " +_monster.title);

        UpdateRender();

    }

    private void UpdateRender()
    {
        UpdateText();
        // Check if the image file already exists locally
        if (File.Exists(localImagePath))
        {
            Debug.Log("from Path: " + localImagePath);
            LoadImageFromFile();
        }
        else
        {
            Debug.Log("from URL: " + imageUrl);
            StartCoroutine(LoadImageFromURL());
        }
    }

    private void UpdateText()
    {
        title.text = monster.title;
        subtitle.text = monster.properties.FirstOrDefault(p => p.name == "subtitle" && p.category == "general").value;
        armorClass.text = monster.properties.FirstOrDefault(p => p.name == "Armor class" && p.category == "stats").value;
        hitPoints.text = monster.properties.FirstOrDefault(p => p.name == "Hit points" && p.category == "stats").value; ;
        speedWalk.text = monster.properties.FirstOrDefault(p => p.name == "Speed" && p.category == "stats").value;

        UpdateAbilityProperty("STR", STR, STR_amount);
        UpdateAbilityProperty("DEX", DEX, DEX_amount);
        UpdateAbilityProperty("CON", CON, CON_amount);
        UpdateAbilityProperty("INT", INT, INT_amount);
        UpdateAbilityProperty("WIS", WIS, WIS_amount);
        UpdateAbilityProperty("CHA", CHA, CHA_amount);

        allStatsAndBoni.text = gatherAllContent("stats","Armor class,Hit points,Speed");
        allFeatures.text = gatherAllContent("Traits");
        allActions.text = gatherAllContent("Actions,Legendary Actions");

    }

    private string gatherAllContent(string categories, string exclusion = "")
    {
        StringBuilder builder = new StringBuilder();

        string[] categoryList = categories.Split(',');
        string[] exclusionList = exclusion.Split(',');

        foreach (string category in categoryList)
        {
            foreach (MonsterProperty mp in monster.properties)
            {
                if (mp.category.Trim() == category.Trim() && (string.IsNullOrEmpty(exclusion) || !exclusionList.Contains(mp.name.Trim())))
                {
                    //builder.AppendLine(mp.name + ": " + mp.value);
                    builder.AppendLine("<b>" + mp.name + "</b>" + mp.value);
                }
            }
        }

        return builder.ToString();
    }

    private void UpdateAbilityProperty(string abilityName, TextMeshProUGUI text, TextMeshProUGUI amountText)
    {
        MonsterProperty property = monster.properties.FirstOrDefault(p => p.name == abilityName && p.category == "abilities");

        if (property != null)
        {
            int abilityScore = int.Parse(property.value);
            int abilityMultiplier = GetAbilityMultiplier(abilityScore);

            text.text = (abilityMultiplier >= 0 ?"+" : "") + abilityMultiplier.ToString();
            amountText.text = abilityScore.ToString();
        }
        else
        {
            text.text = "-";
            amountText.text = "-";
        }
    }

    private IEnumerator LoadImageFromURL()
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);

            // Save the downloaded image as a file
            SaveImageToFile(texture);

            // Set the sprite renderer's sprite
            image.sprite = SpriteFromTexture(texture);
        }
        else
        {
            Debug.LogError("Failed to download image from URL: " + imageUrl);
        }
    }

    private void SaveImageToFile(Texture2D texture)
    {
        byte[] bytes = texture.EncodeToPNG();

        // Create the directory if it doesn't exist
        Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, imageFolderPath));

        File.WriteAllBytes(localImagePath, bytes);
    }

    private void LoadImageFromFile()
    {
        byte[] bytes = File.ReadAllBytes(localImagePath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);

        // Set the sprite renderer's sprite
        image.sprite = SpriteFromTexture(texture);
    }

    private Sprite SpriteFromTexture(Texture2D texture)
    {
        Rect rect = new Rect(0, 0, texture.width, texture.height);
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        return Sprite.Create(texture, rect, pivot);
    }

    private int GetAbilityMultiplier(int abilityScore)
    {
        return Mathf.FloorToInt((abilityScore - 10)/2);
    }
}
