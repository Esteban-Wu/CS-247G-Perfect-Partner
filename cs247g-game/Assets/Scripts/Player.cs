using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public InventoryObject inventory;
    bool inRange = false;
    private Collider collision;
    
    // Putting this here for now
    public GameObject pausePanel;
    void Start()
    {
        // Button listeners
        Button bResumeGame = pausePanel.transform.Find("ButtonResumeGame").GetComponent<Button>();
        Button bSaveGame = pausePanel.transform.Find("ButtonSaveGame").GetComponent<Button>();
        Button bHint = pausePanel.transform.Find("ButtonHint").GetComponent<Button>();
        Button bMainMenu = pausePanel.transform.Find("ButtonMainMenu").GetComponent<Button>();
        bResumeGame.onClick.AddListener(() => ShowPausePanel(false));
        bSaveGame.onClick.AddListener(() => SaveGame());
        bHint.onClick.AddListener(() => Hint());
        bMainMenu.onClick.AddListener(() => MainMenu());
    }
    
    public void OnTriggerEnter(Collider other)
    {
        collision = other;
        inRange = true;
    }

    public void OnTriggerExit(Collider other)
    {
        collision = null;
        inRange = false;
    }

    private void OnApplicationQuit()
    {
        inventory.Container.Clear();
    }

    void Update()
    {
        // Temporarily putting this over here, should move it to another script that is attached
        // to an object that never gets disabled in the entire Day Scene.
        // Bring up the puase menu
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Debug.Log("Escape button pressed");
            ShowPausePanel(true);
        }


        var item = collision.GetComponent<Item>();
        if (inRange && item && Input.GetKeyDown(KeyCode.E))
        {
            inventory.AddItem(item.item, 1);
            Destroy(collision.gameObject);
            collision = null;
            inRange = false;
        }
    }

    // ---------- MOVE THESE SOMEWHERE ELSE ----------
    // Show/hide the pause panel
    void ShowPausePanel(bool visible)
    {
        pausePanel.gameObject.SetActive(visible);
    }
    // Save game
    void SaveGame() 
    {
        Debug.Log("Save game clicked");
    }
    // View hint
    void Hint()
    {
        Debug.Log("Hint clicked");
    }
    // Return to main menu
    void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    // -----------------------------------------------
}
