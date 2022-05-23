using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Scene1 : MonoBehaviour
{
    public Camera scriptedCamera;
    public FirstPersonController fpsController;
    public Canvas canvas; 

    // Scene controller
    public GameObject sceneControllerObject;
    private SceneController sceneController;

    // Objects for Scene 1
    public GameObject upperEyelid;
    public GameObject lowerEyelid;
    public GameObject phone;
    public GameObject husband;

    // Screens for Scene 1
    private GameObject phoneInterface;
    private Image lockedScreen;
    private Image messageScreen;
    private Image unlockingScreen;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Code is trying to run Scene 1 (Night)");

        // Initialize private game objects
        sceneController = sceneControllerObject.GetComponent<SceneController>();
        phoneInterface = canvas.transform.Find("Phone Interface").gameObject;
        lockedScreen = phoneInterface.transform.Find("Locked Screen").GetComponent<Image>();
        messageScreen = phoneInterface.transform.Find("Message Screen").GetComponent<Image>();
        unlockingScreen = phoneInterface.transform.Find("Unlocking Screen").GetComponent<Image>();

        // Initial state of passed-in objects
        scriptedCamera.gameObject.SetActive(true);
        fpsController.gameObject.SetActive(false);
        canvas.gameObject.SetActive(true);
        upperEyelid.gameObject.SetActive(false);
        upperEyelid.gameObject.SetActive(false);
        phone.gameObject.SetActive(true);
        husband.gameObject.SetActive(false);
        phoneInterface.gameObject.SetActive(false);
        lockedScreen.gameObject.SetActive(false);
        messageScreen.gameObject.SetActive(false);
        unlockingScreen.gameObject.SetActive(false);

        // Start Scene 1
        StartCoroutine(RunScene());
    }

    // Scene 1
    IEnumerator RunScene() {
        yield return PanMemoryPhotos();
        yield return WakeUpToPhoneBuzz();
        yield return UnlockPhone();
        yield return DiscoverInfidelity();
        Variables.currentLevel = 2;
        SceneManager.LoadScene("DayScene");
    }

    // Pan through the 8 photos of memories
    IEnumerator PanMemoryPhotos() 
    {
        // Fix initial camera position
        sceneController.MoveObjectToPosition(scriptedCamera.gameObject, new Vector3(-1.7f, 4.2f, -0.97f));
        sceneController.RotateObjectToAngle(scriptedCamera.gameObject, Quaternion.Euler(0f, 180f, 0f));

        // Move camera up right
        Vector3 start = scriptedCamera.transform.position;
        Vector3 end = start + new Vector3(-0.1f, 0.2f, 0f);
        yield return sceneController.MoveObjectOverTime(scriptedCamera.gameObject, start, end, 3.5f);

        // Move camera right
        start = end;
        end = start + new Vector3(-0.35f, 0f, 0f);
        yield return sceneController.MoveObjectOverTime(scriptedCamera.gameObject, start, end, 3.5f);

        // Move camera down right
        start = end;
        end = start + new Vector3(-0.3f, -0.4f, 0f);
        yield return sceneController.MoveObjectOverTime(scriptedCamera.gameObject, start, end, 3.5f);

        // Move camera up right
        start = end;
        end = start + new Vector3(-1f, 0.08f, 0f);
        yield return sceneController.MoveObjectOverTime(scriptedCamera.gameObject, start, end, 6);

        // Wait 1 second, fade to black, turn camera light off
        yield return sceneController.Wait(1);
        yield return sceneController.Fade(true, 0.5f);
        scriptedCamera.transform.Find("Point Light").GetComponent<Light>().intensity = 0;
    }

    // Wake up to the husband's phone buzzing
    IEnumerator WakeUpToPhoneBuzz() 
    {
        // Move camera to bed, eyes closed
        sceneController.MoveObjectToPosition(scriptedCamera.gameObject, new Vector3(-6.6f, 3.6f, 1.5f));
        sceneController.RotateObjectToAngle(scriptedCamera.gameObject, Quaternion.Euler(0f, 90f, 0f));

        // Prepare for blink
        upperEyelid.gameObject.SetActive(true);
        lowerEyelid.gameObject.SetActive(true);
        sceneController.HideBlackOverlay();

        // Wait 1 second, then blink 3 times
        yield return sceneController.Wait(1);
        yield return Blink(3);

        // Wait 1 second, then turn to husband's spot
        yield return sceneController.Wait(1);
        Quaternion start = scriptedCamera.transform.rotation;
        Quaternion end = Quaternion.Euler(10, 20, 0);
        yield return sceneController.RotateObjectOverTime(scriptedCamera.gameObject, start, end, 3);

        // Wait 2 seconds, then show text
        yield return sceneController.Wait(2);
        yield return sceneController.ShowText("Where is he...?", true);

        // Phone glow
        yield return sceneController.FlashObject(phone, 3, 2);
        // TODO: play ~ding~ audio

        // Show phone interface once the phone is clicked
        // yield return sceneController.WaitForClick("phone");
        yield return sceneController.WaitForKeyPress(KeyCode.E, "Press 'E' to interact");
        phoneInterface.SetActive(true);
        lockedScreen.gameObject.SetActive(true);
        unlockingScreen.gameObject.SetActive(false);
        messageScreen.gameObject.SetActive(false);
        yield return sceneController.Wait(1.5f);
        yield return sceneController.ShowText("What the f-", true);

        // Transition to unlocking screen
        lockedScreen.gameObject.SetActive(false);
        unlockingScreen.gameObject.SetActive(true);
    }

    // The unlocking phone puzzle
    IEnumerator UnlockPhone() 
    {
        string answer = "730";
        string guess = "";
        
        // Set up buttons
        Button[] numberButtons = {null, null, null, null, null, null, null, null, null, null};
        for (int i = 0; i < 10; i++) {
            Button b = unlockingScreen.transform.Find(i + "Button").GetComponent<Button>();
            numberButtons[i] = b;
        }
        Button backButton = unlockingScreen.transform.Find("Back Button").GetComponent<Button>();
        
        // Update current hint
        sceneController.SetHint("Find his passcode on a sticky note in a drawer.");

        // Detect button clicks
        yield return ShowDots(guess);
        yield return sceneController.ShowText("I saw his passcode on a sticky note in a drawer...", false);
        do {
            if(Input.GetMouseButtonDown(0)) {
                // Get the currently selected button
                GameObject curr = EventSystem.current.currentSelectedGameObject;

                // Number buttons
                for (int i = 0; i < 10; i++) {
                    if (curr != null && numberButtons[i].name.Equals(curr.name)) {
                        guess += "" + i;
                        yield return ShowDots(guess);
                        if (guess.Equals(answer)) {
                            yield return sceneController.ShowTopText("Correct passcode. Phone unlocked.", 1);
                            break;
                        } else if (guess.Length == 3) {
                            yield return sceneController.ShowTopText("Incorrect passcode. Try again.", 1);
                            guess = "";
                            yield return ShowDots(guess);
                        }
                    }
                }

                // Back button
                if (curr != null && curr.name.Equals("Back Button")) {
                    // Switch to FPS controller
                    sceneController.HideBottomText();
                    phoneInterface.SetActive(false);
                    sceneController.EnableFPSController(true);

                    // Show phone interface once the phone is clicked
                    yield return sceneController.WaitForPlayerInteract(phone);
                    phoneInterface.SetActive(true);
                    sceneController.EnableFPSController(false);
                    yield return sceneController.ShowText("I saw his passcode on a sticky note... where is it?", false);
                }
            }
            yield return null;
        } while (!guess.Equals(answer));
    }

    // Read cheating messages and faint
    IEnumerator DiscoverInfidelity() 
    {
        // Show cheating messages and wait for player to scroll to the bottom
        sceneController.HideBottomText();
        sceneController.SetHint("Scroll through the text messages.");
        unlockingScreen.gameObject.SetActive(false);
        messageScreen.gameObject.SetActive(true);
        yield return WaitForScroll();
        yield return sceneController.ShowText("Oh my goodness...", true);
        phoneInterface.gameObject.SetActive(false);
        sceneController.ResetHint();

        // Move camera to edge of bed
        sceneController.MoveObjectToPosition(scriptedCamera.gameObject, new Vector3(-6.6f, 3.9f, 2.1f));
        sceneController.RotateObjectToAngle(scriptedCamera.gameObject, Quaternion.Euler(30f, -15f, 0f));

        // Drop phone
        phone.GetComponent<Outline>().OutlineWidth = 0; 
        sceneController.MoveObjectToPosition(phone, new Vector3(-6.517f, 3.692f, 2.509f));
        sceneController.RotateObjectToAngle(phone, Quaternion.Euler(-148.346f, 33.063f, -90f));
        phone.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, 100), ForceMode.Force);
        yield return sceneController.Wait(2);

        // Husband appears and walks towards camera
        husband.gameObject.SetActive(true);
        Vector3 start = husband.transform.position;
        Vector3 end = new Vector3(-6.25f, 2.9f, 2.7f);
        yield return sceneController.MoveObjectOverTime(husband, start, end, 1);
        sceneController.ShowTopText("          \"Hey honey, what's wrong?\"");
        yield return sceneController.Wait(2);

        // Screen fizzles while fading to black
        StartCoroutine(sceneController.Fade(true, 0.3f));
        int shakeTimes = 10;
        float shakeAmount = 0.03f;
        Vector3 originalPos = scriptedCamera.transform.localPosition;
        while (shakeTimes > 0) {
			scriptedCamera.transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
			shakeTimes -= 1;
            yield return sceneController.Wait(0.1f);
		}
        scriptedCamera.transform.localPosition = originalPos;
        yield return sceneController.Wait(3);

        // Hide husband text, transition to next scene
        sceneController.HideTopText();
    }

    // Show dots to indicate length of current guess in the unlock phone puzzle
    IEnumerator ShowDots(string guess) {
        Image dot1 = unlockingScreen.transform.Find("Dot1").GetComponent<Image>();
        Image dot2 = unlockingScreen.transform.Find("Dot2").GetComponent<Image>();
        Image dot3 = unlockingScreen.transform.Find("Dot3").GetComponent<Image>();
        dot1.color = new Color(255f, 255f, 255f, 0f);
        dot2.color = new Color(255f, 255f, 255f, 0f);
        dot3.color = new Color(255f, 255f, 255f, 0f);
        if (guess.Length >= 1) dot1.color = new Color(255f, 255f, 255f, 255f);
        if (guess.Length >= 2) dot2.color = new Color(255f, 255f, 255f, 255f);
        if (guess.Length >= 3) dot3.color = new Color(255f, 255f, 255f, 255f);
        yield return null;
    }

    // Wait until player finishes reading the cheating messages
    IEnumerator WaitForScroll() {
        RawImage cheatingMessages = GameObject.Find("Cheating Messages").GetComponent<RawImage>();
        RectTransform transform = cheatingMessages.GetComponent<RectTransform>();
        while (transform.anchoredPosition.y < 750f) {
            yield return null;
        }
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
            if (currentBlink != n) {
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
                if (opening) {
                    upperEyelid.transform.position = Vector3.Lerp(originalUpperEyelidPosition, upper, duration);
                    lowerEyelid.transform.position = Vector3.Lerp(originalLowerEyelidPosition, lower, duration);
                } else {
                    upperEyelid.transform.position = Vector3.Lerp(endUpper, upper, duration);
                    lowerEyelid.transform.position = Vector3.Lerp(endLower, lower, duration);
                }
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
