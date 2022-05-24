using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Scene2 : MonoBehaviour
{
    public Camera scriptedCamera;
    public FirstPersonController fpsController;
    public Canvas canvas; 

    // Scene controller
    public GameObject sceneControllerObject;
    private SceneController sceneController;

    // Scene 2 objects
    public InventoryObject inventory;
    public GameObject upperEyelid;
    public GameObject lowerEyelid;
    public GameObject blackOverlay;
    private GameObject inventoryScreen;
    private GameObject adScreen;
    // private bool adSolved;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Code is trying to run Scene 2 (Day)");

        // Initialize private game objects
        sceneController = sceneControllerObject.GetComponent<SceneController>();
        inventoryScreen = canvas.transform.Find("Inventory Screen").gameObject;
        blackOverlay = canvas.transform.Find("Black Overlay").gameObject;
        adScreen = canvas.transform.Find("Ad").gameObject;
        // adSolved = false;

        // Initialize state of passed-in objects
        scriptedCamera.gameObject.SetActive(true);
        fpsController.gameObject.SetActive(false);
        inventoryScreen.gameObject.SetActive(false);
        adScreen.gameObject.SetActive(false);

        // Start Scene 2
        StartCoroutine(RunScene());
    }

    IEnumerator RunScene() {
        // To wait for x seconds: sceneController.Wait(x);
        // To show bottom text: sceneController.ShowText(str, speed);
        // To move objects: 
        //     1) sceneController.MoveObjectToPosition(obj, pos)
        //     2) sceneController.MoveObjectOverTime(obj, start, end, duration);
        // Etc. Refer to SceneController.cs for more useful methods :D

        yield return WakeUp();
        
        // Transition to scene 3 by calling: 
        //     1) Variables.currentLevel = 3;
        //     2) SceneManager.LoadScene("NightScene");
        yield return null;
    }

    // Player wakes up in the bedroom. Code adapted from Scene 1 WakeUpToPhoneBuzz()
    IEnumerator WakeUp()
    {
        // Move camera to bed, eyes closed
        sceneController.MoveObjectToPosition(scriptedCamera.gameObject, new Vector3(-6.6f, 3.6f, 1.5f));
        sceneController.RotateObjectToAngle(scriptedCamera.gameObject, Quaternion.Euler(0f, 90f, 0f));

        // Prepare for blink
        upperEyelid.gameObject.SetActive(true);
        lowerEyelid.gameObject.SetActive(true);
        // sceneController.HideBlackOverlay();

        // Wait 1 second, then blink 3 times
        yield return sceneController.Wait(1);
        yield return Blink(3);
        yield return sceneController.Wait(1);

        // Check for husband
        Quaternion start = scriptedCamera.transform.rotation;
        Quaternion end = Quaternion.Euler(10, 20, 0);
        yield return sceneController.RotateObjectOverTime(scriptedCamera.gameObject, start, end, 3);
        yield return sceneController.Wait(1);
        end = start;
        start = scriptedCamera.transform.rotation;
        yield return sceneController.RotateObjectOverTime(scriptedCamera.gameObject, start, end, 3);
        yield return sceneController.Wait(1);

        // Return to FPS
        upperEyelid.gameObject.SetActive(false);
        lowerEyelid.gameObject.SetActive(false);
        inventoryScreen.gameObject.SetActive(true);
        sceneController.EnableFPSController(true);
        sceneController.SetHint("Interact with the TV.");
        yield return null;
    }

    public IEnumerator OpenAd()
    {
        bool open = true;

        Debug.Log("Opened ad!");
        blackOverlay.gameObject.SetActive(true);
        adScreen.gameObject.SetActive(true);
        sceneController.SetHint("Find and collect the credit card. It's not in the bedroom.");
        yield return sceneController.ShowText("Looks like I need to pay with a credit card...", true, 0);
        do
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Get the currently selected button
                GameObject curr = EventSystem.current.currentSelectedGameObject;
                // Back button, close ad
                if (curr != null && curr.name.Equals("Back Button"))
                {
                    adScreen.gameObject.SetActive(false);
                    blackOverlay.gameObject.SetActive(false);
                    open = false;
                    Debug.Log("Closing ad!");
                }
                if (curr != null && curr.name.Equals("Pay Button"))
                {
                    if (checkInventory("Credit Card"))
                    {
                        sceneController.ResetHint();
                        yield return sceneController.ShowTopText("Thank you for your payment! Your order will arrive soon.", 1);
                        yield return sceneController.Wait(1);
                        yield return SpawnRouen();
                        break;
                    }
                    else
                    {
                        yield return sceneController.ShowTopText("You need a credit card to pay!", 1);
                    }
                }
            }
            yield return null;
        } while (open);
    }

    

    // Check the player's inventory for an item
    bool checkInventory(string item)
    {
        // Debug.Log(inventory);
        for (int i = 0; i < inventory.Container.Count; i++)
        {
            if (inventory.Container[i].item.name == item)
            {
                return true;
            }
        }
        return false;
    }

    IEnumerator SpawnRouen()
    {   
        // Move the scripted camera over to the player
        sceneController.MoveObjectToPosition(scriptedCamera.gameObject, fpsController.transform.position);
        sceneController.RotateObjectToAngle(scriptedCamera.gameObject, fpsController.transform.rotation);
        // Return to scripted camera
        adScreen.gameObject.SetActive(false);
        blackOverlay.gameObject.SetActive(false);
        scriptedCamera.gameObject.SetActive(true);
        fpsController.gameObject.SetActive(false);
        // Now, rotate the scripted camera to wherever Rouen has been spawned
        // Alternatively, we can leave the player with the fpsController instead
        // of scripted for a potentially scarier effect.
        yield return null;
    }

    // Eye blink effect
    // https://github.com/tetreum/eyeblink/blob/master/EyeBlink.cs
    IEnumerator Blink(int n)
    {
        Vector3 originalUpperEyelidPosition = upperEyelid.transform.position;
        Vector3 originalLowerEyelidPosition = lowerEyelid.transform.position;
        Vector3 endUpper, endLower;
        int currentBlink = 1;
        while (currentBlink <= n)
        {
            // Determine end position of the eyelids
            endUpper = originalUpperEyelidPosition;
            endLower = originalLowerEyelidPosition;
            endUpper.y += (0.1f * currentBlink);
            endLower.y -= (0.1f * currentBlink);

            // Open eyelids
            yield return moveEyelids(endUpper, endLower, true, 0.70f);

            // Close eyelids
            if (currentBlink != n)
            {
                yield return moveEyelids(originalUpperEyelidPosition, originalLowerEyelidPosition, false, 0.70f);
            }
            currentBlink++;
        }
        upperEyelid.gameObject.SetActive(false);
        lowerEyelid.gameObject.SetActive(false);

        // Helper function to move eyelids
        IEnumerator moveEyelids(Vector3 upper, Vector3 lower, bool opening, float speed)
        {
            float elapsedTime = 0;
            while (elapsedTime < speed)
            {
                float duration = (elapsedTime / speed);
                if (opening)
                {
                    upperEyelid.transform.position = Vector3.Lerp(originalUpperEyelidPosition, upper, duration);
                    lowerEyelid.transform.position = Vector3.Lerp(originalLowerEyelidPosition, lower, duration);
                }
                else
                {
                    upperEyelid.transform.position = Vector3.Lerp(endUpper, upper, duration);
                    lowerEyelid.transform.position = Vector3.Lerp(endLower, lower, duration);
                }
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
