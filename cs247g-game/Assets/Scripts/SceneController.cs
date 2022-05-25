using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{    
    public Camera scriptedCamera; // Animation
    public FirstPersonController fpsController; // Player
    public GameObject canvas; // 2D interactions (phone screen, menus)

    // Objects used in multiple story scenes
    private GameObject pausePanel;
    private GameObject hintPanel;
    private TextMeshProUGUI topText;
    private TextMeshProUGUI bottomText;
    private Image returnIcon;
    private Image blackOverlay;

    // Scenes
    public GameObject scene1;
    public GameObject scene2;
    public GameObject scene3;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize private game objects
        pausePanel = canvas.transform.Find("Pause Panel").gameObject;
        hintPanel = canvas.transform.Find("Hint").gameObject;
        topText = canvas.transform.Find("Top Text").GetComponent<TextMeshProUGUI>();
        bottomText = canvas.transform.Find("Bottom Text").GetComponent<TextMeshProUGUI>();
        returnIcon = canvas.transform.Find("Return Key").GetComponent<Image>();
        blackOverlay = canvas.transform.Find("Black Overlay").GetComponent<Image>();
        
        // Button listeners
        Button bResumeGame = pausePanel.transform.Find("ButtonResumeGame").GetComponent<Button>();
        Button bSaveGame = pausePanel.transform.Find("ButtonSaveGame").GetComponent<Button>();
        Button bHint = pausePanel.transform.Find("ButtonHint").GetComponent<Button>();
        Button bMainMenu = pausePanel.transform.Find("ButtonMainMenu").GetComponent<Button>();
        bResumeGame.onClick.AddListener(() => ShowPausePanel(false));
        bSaveGame.onClick.AddListener(() => SaveGame());
        bHint.onClick.AddListener(() => Hint());
        bMainMenu.onClick.AddListener(() => MainMenu());

        // Initial state of passed in objects
        pausePanel.gameObject.SetActive(false);
        hintPanel.gameObject.SetActive(false);
        topText.gameObject.SetActive(false);
        bottomText.gameObject.SetActive(false);
        returnIcon.gameObject.SetActive(false);
        blackOverlay.gameObject.SetActive(false);

        // Get the current story scene the player is in
        Debug.Log("Current level: " + Variables.currentLevel);
        switch (Variables.currentLevel) {
            case 1:
                scene1.gameObject.SetActive(true);
                break;
            case 2:
                scene2.gameObject.SetActive(true);
                break;
            case 3:
                scene3.gameObject.SetActive(true);
                break;
        }
    }

    // Wait for x seconds
    public IEnumerator Wait(float x)
    {
        yield return new WaitForSeconds(x);
    }

    // Show text on the canvas with a typewriter effect
    // Character: 0 = player, 1 = rouen
    public IEnumerator ShowText(string fullText, bool waitForKeyPress, int character)
    {
        // Set color of text
        switch(character) {
            case 0: // player - white
                bottomText.color = new Color(1, 1, 1, 0.8f);
                break;
            case 1: // rouen - red
                bottomText.color = new Color(0.5f, 0, 0, 1);
                break;
        }

        // Typewriter effect
        Debug.Log("Setting active");
        bottomText.gameObject.SetActive(true);
        Debug.Log("Set active.");
        for (int i = 0; i < fullText.Length + 1; i++)
        {
            // bottomText.gameObject.SetActive(true);
            string currentText = fullText.Substring(0, i);
            bottomText.text = currentText;
            yield return new WaitForSeconds(0.05f);
        }

        // Wait until E is hit
        if (waitForKeyPress)
        {
            returnIcon.gameObject.SetActive(true);
            while(!Input.GetKeyDown(KeyCode.E))
            {
                yield return null; 
            }
            returnIcon.gameObject.SetActive(false);
            bottomText.gameObject.SetActive(false);
        }
    }

    // Wait for an object to be clicked
    public IEnumerator WaitForClick(string objectName)
    {
        while(true) {
            if(Input.GetMouseButtonDown(0)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  
                RaycastHit hit;  
                if (Physics.Raycast(ray, out hit)) { 
                    if (hit.transform.name.Equals(objectName)) {
                        break;
                    }
                }
            }
            yield return null;
        }
    }

    // Wait for the player to interact with an object
    public IEnumerator WaitForPlayerInteract(GameObject obj, float range)
    {
        while(true) {
            // If object is in range, show message
            Vector3 playerPos = fpsController.transform.position;
            Vector3 objectPos = obj.transform.position;
            float distance = Vector3.Distance(playerPos, objectPos);
            bool inRange = distance < range? true: false;
            if (inRange) {
                ShowBottomText("Press 'E' to interact");
            } else {
                HideBottomText();
            }

            // If 'E' key is pressed when in range, break
            if(inRange && Input.GetKeyDown(KeyCode.E)) {
                break;
            }
            yield return null;
        }
    }

    // Wait for the player to get near an object
    public IEnumerator WaitForPlayerToGetNear(GameObject obj, float range)
    {
        while(true) {
            Vector3 playerPos = fpsController.transform.position;
            Vector3 objectPos = obj.transform.position;
            float distance = Vector3.Distance(playerPos, objectPos);
            if (distance < range) break;
            yield return null;
        }
    }

    // Wait for the 'E' key to be pressed
    public IEnumerator WaitForKeyPress(KeyCode keyCode, string message)
    {
        ShowBottomText(message);
        while(!Input.GetKeyDown(keyCode))
        {
            yield return null;
        }
        HideBottomText();
    }

    // Flash an object x times, assuming it has the Outline.cs script attached
    public IEnumerator FlashObject(GameObject obj, int x, int width)
    {
        for (int i = 0; i < x; i++) {
            HighlightObject(obj, width);
            yield return Wait(0.3f);
            if (i < x - 1) {
                DehighlightObject(obj);
                yield return Wait(0.3f);
            }
        }
    }

    // Highlight an object, assuming it has the Outline.cs script attached
    public void HighlightObject(GameObject obj, int width)
    {
        obj.GetComponent<Outline>().OutlineWidth = width;
    }

    // Dehighlight an object, assuming it has the Outline.cs script attached
    public void DehighlightObject(GameObject obj)
    {
        obj.GetComponent<Outline>().OutlineWidth = 0;
    }

    // Move an object to a new position
    public void MoveObjectToPosition(GameObject obj, Vector3 pos)
    {
        obj.transform.position = pos;
    }

    // Rotate an object to a new angle
    public void RotateObjectToAngle(GameObject obj, Quaternion angle)
    {   
        obj.transform.rotation = angle;
    }

    // Move an object from start to end in t seconds
    public IEnumerator MoveObjectOverTime(GameObject obj, Vector3 start, Vector3 end, float t)
    {
        obj.transform.position = start;
        Vector3 currentPos = start;
        float ratio = 0f;
        while (ratio < 1)
        {
            ratio += Time.deltaTime / t;
            obj.transform.position = Vector3.Lerp(currentPos, end, ratio);
            yield return null;
        }
    }

    // Rotate an object from start to end in t seconds
    public IEnumerator RotateObjectOverTime(GameObject obj, Quaternion start, Quaternion end, float t)
    {
        obj.transform.rotation = start;
        Quaternion currentAngle = start;
        float ratio = 0f;
        while (ratio < 1)
        {
            ratio += Time.deltaTime / t;
            obj.transform.rotation = Quaternion.Slerp(currentAngle, end, ratio);
            yield return null;
        }
    }

    // Show or hide the black overlay
    public void ShowBlackOverlay(bool visible) 
    {
        blackOverlay.color = new Color(0, 0, 0, 1);
        blackOverlay.gameObject.SetActive(visible);
    }

    // Fade to black/transparent
    public IEnumerator Fade(bool black, float speed)
    {
        Color c;
        if (black) c = new Color(0, 0, 0, 0); // black
        else c = new Color(0, 0, 0, 1); // transparent
        blackOverlay.color = c;
        blackOverlay.gameObject.SetActive(true);

        // Transparent to black
        if (black) {
            while (blackOverlay.color.a < 1) {
                c.a += (speed * Time.deltaTime);
                blackOverlay.color = c;
                yield return null;
            }
        } 
        
        // Black to transparent
        else {
            while (blackOverlay.color.a > 0) {
                c.a -= (speed * Time.deltaTime);
                blackOverlay.color = c;
                yield return null;
            }
        }
    }

    // Enable/disable the FPS controller
    public void EnableFPSController(bool state) {
        if (state) {
            fpsController.gameObject.SetActive(true);
            scriptedCamera.gameObject.SetActive(false);
        } else {
            scriptedCamera.gameObject.SetActive(true);
            fpsController.gameObject.SetActive(false);
        }
    }

    // Show str as top text for x seconds
    public IEnumerator ShowTopText(string str, float x) {
        topText.text = str;
        topText.gameObject.SetActive(true);
        yield return Wait(x);
        topText.gameObject.SetActive(false);
    }

    // Show str as top text
    public void ShowTopText(string str) {
        topText.text = str;
        topText.gameObject.SetActive(true);
    }

    // Hide top text
    public void HideTopText() {
        topText.gameObject.SetActive(false);
    }

    // Show str as bottom text
    public void ShowBottomText(string str) {
        bottomText.text = str;
        bottomText.gameObject.SetActive(true);
    }

    // Hide bottom text
    public void HideBottomText() {
        bottomText.gameObject.SetActive(false);
    }

    // Show/hide the pause panel
    public void ShowPausePanel(bool visible)
    {
        pausePanel.gameObject.SetActive(visible);
    }

    // Show/hide the hint panel
    public void ShowHintPanel(bool visible)
    {
        Debug.Log("Hello: " + visible);
        hintPanel.gameObject.SetActive(visible);
    }

    // Set hint
    public void SetHint(string hint)
    {
        Variables.currentHint = hint;
    }

    // Reset hint
    public void ResetHint()
    {
        Variables.currentHint = "Currently not available.";
    }

    // Save game
    void SaveGame() 
    {
        Debug.Log("Save game clicked");
    }

    // View hint
    void Hint()
    {
        ShowHintPanel(true);
    
        // Set hint text
        TextMeshProUGUI hintText = hintPanel.transform.Find("HintMessage").GetComponent<TextMeshProUGUI>();
        hintText.text = Variables.currentHint;

        // Close hint panel
        Button hintClose = hintPanel.transform.Find("HintClose").GetComponent<Button>();
        hintClose.onClick.AddListener(() => ShowHintPanel(false));
    }

    // Return to main menu
    void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            ShowPausePanel(true);
        }
    }
}
