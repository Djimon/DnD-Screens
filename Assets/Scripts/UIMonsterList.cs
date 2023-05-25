using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMonsterList : MonoBehaviour
{
    public GameObject monsterButtonPrefab;
    public Transform buttonContainer;
    public string monsterFolderPath;
    public Image monsterImage;
    public MonsterCard selectedMonster; // Optional reference to store the currently selected monster
    public MonsterCardEntity monsterCardEntity;


    private bool isMouseInsideWindow = false;
    private float scrollSpeed = 0.05f;

    private void Start()
    {
        
        LoadMonsterList();
    }

    private void Update()
    {
        // Check if the mouse is inside the game window
        Vector3 mousePosition = Input.mousePosition;
        isMouseInsideWindow = mousePosition.x >= 0 && mousePosition.x < Screen.width &&
                              mousePosition.y >= 0 && mousePosition.y < Screen.height;

        // Handle mouse wheel scrolling if the mouse is inside the window and the scroll view is active
        if (isMouseInsideWindow && gameObject.activeInHierarchy)
        {
            float scrollDelta = Input.mouseScrollDelta.y;
            ScrollRect scrollRect = GetComponent<ScrollRect>();
            scrollRect.verticalNormalizedPosition += scrollDelta * scrollSpeed;
        }
    }

    private void LoadMonsterList()
    {
        // Clear existing buttons
        ClearButtons();

        // Get all monster assets in the folder
        string monsterFolderPath = "Assets/content/monster";
        string[] monsterFiles = Directory.GetFiles(monsterFolderPath, "*.asset");

        // Sort the monster files alphabetically
        System.Array.Sort(monsterFiles);

        // Calculate the height of each button
        float buttonHeight = monsterButtonPrefab.GetComponent<RectTransform>().rect.height;

        // Calculate the total height of the content
        float contentHeight = buttonHeight * monsterFiles.Length + buttonHeight;
        //6900 ?

        // Set the height of the content
        RectTransform contentTransform = buttonContainer.GetComponent<RectTransform>();
        contentTransform.sizeDelta = new Vector2(contentTransform.sizeDelta.x, contentHeight);

        for (int i = 0; i < monsterFiles.Length; i++)
        {
            // Load the ScriptableObject
            string monsterName = Path.GetFileNameWithoutExtension(monsterFiles[i]);
            string monsterpath = monsterFiles[i];

            // Create a new button
            GameObject button = Instantiate(monsterButtonPrefab, buttonContainer);
            button.GetComponentInChildren<TextMeshProUGUI>().text = monsterName;

            // Set the button's position
            RectTransform buttonTransform = button.GetComponent<RectTransform>();
            float buttonY = -buttonHeight * i - buttonHeight + contentHeight / 2;
            Debug.Log("Y = " +buttonY);
            buttonTransform.anchoredPosition = new Vector2(buttonTransform.anchoredPosition.x, buttonY);

            // Add click event listener
            button.GetComponent<Button>().onClick.AddListener(() => ChangeMonster(monsterpath));
        }

        ScrollRect scrollRect = gameObject.GetComponent<ScrollRect>();
        scrollRect.normalizedPosition = new Vector2(0, 1);

    }

    public void ChangeMonster(string monsterFile)
    {
        string name = Path.GetFileNameWithoutExtension(monsterFile);
        // Handle the selected monster (e.g., load the monster's details)
        Debug.Log("Selected monster: " + name);
        // Optionally assign the selected monster reference
        //selectedMonster = GameObject.Find(monsterFile);

        MonsterCard monsterCard = UnityEditor.AssetDatabase.LoadAssetAtPath<MonsterCard>(monsterFile);
        monsterCardEntity.ChangeMonster(monsterCard);
        //selectedMonster = monsterCard;
    }

    private void ClearButtons()
    {
        // Destroy all existing buttons
        foreach (Transform child in buttonContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
