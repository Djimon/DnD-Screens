using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MonsterCardEntity : MonoBehaviour
{
    public MonsterCard monster;

    public Image image;


    public string imageUrl;
    public string imageFileName;
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
}
