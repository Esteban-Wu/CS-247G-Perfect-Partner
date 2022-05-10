using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    public Camera scriptedCamera;
    public FirstPersonController fpsController;
    public Canvas canvas;
    public GameObject upperEyelid;
    public GameObject lowerEyelid;
    public GameObject phone;
    public GameObject phoneInterface;
    public GameObject husband;

    private Image blackOverlay;
    private TextMeshProUGUI bottomText;
    private TextMeshProUGUI topText;
    private Image screen1;
    private Image screen2;
    private Image lockScreen;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize private game objects
        blackOverlay = canvas.transform.Find("Black Overlay").GetComponent<Image>();
        bottomText = canvas.transform.Find("Bottom Text").GetComponent<TextMeshProUGUI>();
        topText = canvas.transform.Find("Top Text").GetComponent<TextMeshProUGUI>();

        // Start Scene 1
        StartCoroutine(Scene1());
    }

    // This is a coroutine for Scene 1.
    IEnumerator Scene1() 
    {
        yield return PanMemoryPhotos();
        yield return WakeUpToPhoneBuzz();
        yield return UnlockPhone();
        yield return DiscoverInfidelity();

        // Debug.Log("Switch to first-person controller");
        // scriptedCamera.gameObject.SetActive(false);
        // fpsController.gameObject.SetActive(true);
        yield return null;
    }

    // Method to through the 8 photos of memories.
    IEnumerator PanMemoryPhotos() 
    {
        // Enable/disable the passed-in game objects
        scriptedCamera.gameObject.SetActive(true);
        fpsController.gameObject.SetActive(false);
        canvas.gameObject.SetActive(true);
        blackOverlay.gameObject.SetActive(false);
        bottomText.gameObject.SetActive(false);
        topText.gameObject.SetActive(false);
        phoneInterface.gameObject.SetActive(false);
        husband.gameObject.SetActive(false);
        upperEyelid.gameObject.SetActive(false);
        upperEyelid.gameObject.SetActive(false);
        
        // Fix initial camera position
        scriptedCamera.transform.position = new Vector3(-1.7f, 4.2f, -0.97f);
        scriptedCamera.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        // Move camera up right
        Vector3 start = scriptedCamera.transform.position;
        Vector3 end = start + new Vector3(-0.1f, 0.2f, 0f);
        yield return MoveCameraToPosition(start, end, 3);
        // Move camera right
        start = end;
        end = start + new Vector3(-0.35f, 0f, 0f);
        yield return MoveCameraToPosition(start, end, 3);
        // Move camera down right
        start = end;
        end = start + new Vector3(-0.3f, -0.4f, 0f);
        yield return MoveCameraToPosition(start, end, 3);
        // Move camera up right
        start = end;
        end = start + new Vector3(-1f, 0.08f, 0f);
        yield return MoveCameraToPosition(start, end, 5);
        // Wait 3 seconds, then fade to black
        yield return Wait(2);
        yield return Fade(true, 0.5f);
    }

    // Method to wake up to the husband's phone buzzing.
    IEnumerator WakeUpToPhoneBuzz() 
    {
        // Move camera to bed, eyes closed
        scriptedCamera.transform.position = new Vector3(-6.6f, 3.6f, 1.5f);
        scriptedCamera.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        upperEyelid.gameObject.SetActive(true);
        lowerEyelid.gameObject.SetActive(true);
        blackOverlay.gameObject.SetActive(false);

        // Wait 3 seconds, then blink 3 times (opening)
        yield return Wait(3);
        yield return Blink(3, true);

        // Wait 1 second, then turn to husband's spot
        yield return Wait(1);
        Quaternion start = scriptedCamera.transform.rotation;
        Quaternion end = Quaternion.Euler(10, 20, 0);
        yield return RotateCameraToAngle(start, end, 3);

        // Wait 2 seconds, then show text
        yield return Wait(2);
        yield return ShowText("Where is he...?");

        // Phone glow
        phone.GetComponent<Outline>().OutlineWidth = 2; 
        yield return Wait(0.3f);
        phone.GetComponent<Outline>().OutlineWidth = 0;
        yield return Wait(0.3f);
        phone.GetComponent<Outline>().OutlineWidth = 2;
        yield return Wait(0.3f);
        phone.GetComponent<Outline>().OutlineWidth = 0;
        yield return Wait(0.3f);
        phone.GetComponent<Outline>().OutlineWidth = 2;
        yield return Wait(0.3f);
        // TODO: play ~ding~ audio

        // Show phone interface once the phone is clicked
        yield return WaitForClick("phone");
        phoneInterface.SetActive(true);
        screen1 = phoneInterface.transform.Find("Screen 1").GetComponent<Image>();
        screen2 = phoneInterface.transform.Find("Screen 2").GetComponent<Image>();
        lockScreen = phoneInterface.transform.Find("Lock Screen").GetComponent<Image>();
        screen1.gameObject.SetActive(true);
        screen2.gameObject.SetActive(false);
        lockScreen.gameObject.SetActive(false);
        yield return Wait(1.5f);
        yield return ShowText("What the f-");

        // Transition to lock screen
        screen1.gameObject.SetActive(false);
        lockScreen.gameObject.SetActive(true);
    }

    // The unlock phone puzzle.
    IEnumerator UnlockPhone() {
        string answer = "730";
        string guess = "";

        // Set up buttons
        Button[] numberButtons = {null, null, null, null, null, null, null, null, null, null};
        for (int i = 0; i < 10; i++) {
            Button b = lockScreen.transform.Find(i + "Button").GetComponent<Button>();
            numberButtons[i] = b;
        }
        Button cancelButton = lockScreen.transform.Find("CancelButton").GetComponent<Button>();
        
        // Detect button clicks
        yield return ShowDots(guess);
        do {
            if(Input.GetMouseButtonDown(0)) {
                for (int i = 0; i < 10; i++) {
                    GameObject curr = EventSystem.current.currentSelectedGameObject;
                    if (curr != null && numberButtons[i].name.Equals(curr.name)) {
                        guess += "" + i;
                        yield return ShowDots(guess);
                        if (guess.Equals(answer)) {
                            yield return ShowTopText("Correct passcode. Phone unlocked.", 1);
                            break;
                        } else if (guess.Length == 3) {
                            yield return ShowTopText("Incorrect passcode. Try again.", 1);
                            guess = "";
                            yield return ShowDots(guess);
                        }
                    }
                }
            }
            yield return null;
        } while (!guess.Equals(answer));
    }

    // Read cheating messages and faint.
    IEnumerator DiscoverInfidelity() {
        // Show cheating messages and wait for player to scroll to the bottom
        lockScreen.gameObject.SetActive(false);
        screen2.gameObject.SetActive(true);
        yield return WaitForScroll();
        yield return ShowText("Oh my goodness...");
        phoneInterface.gameObject.SetActive(false);

        // Move camera to edge of bed
        scriptedCamera.transform.position = new Vector3(-6.6f, 3.9f, 2.1f);
        scriptedCamera.transform.rotation = Quaternion.Euler(30f, -15f, 0f);

        // Drop phone
        phone.GetComponent<Outline>().OutlineWidth = 0; 
        phone.transform.position = new Vector3(-6.517f, 3.692f, 2.509f);
        phone.transform.rotation = Quaternion.Euler(-148.346f, 33.063f, -90f);
        phone.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, 100), ForceMode.Force);
        yield return Wait(2);

        // Husband appears and walks towards camera
        husband.gameObject.SetActive(true);
        Vector3 start = husband.transform.position;
        Vector3 end = new Vector3(-6.25f, 2.9f, 2.7f);
        yield return MoveObjectToPosition(husband, start, end, 1);
        topText.gameObject.SetActive(true);
        topText.text = "          \"Hey honey, what's wrong?\"";
        yield return Wait(2);

        // Screen fizzles while fading to black
        StartCoroutine(Fade(true, 0.3f));
        int shakeTimes = 10;
        float shakeAmount = 0.03f;
        Vector3 originalPos = scriptedCamera.transform.localPosition;
        while (shakeTimes > 0) {
			scriptedCamera.transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
			shakeTimes -= 1;
            yield return Wait(0.1f);
		}
        scriptedCamera.transform.localPosition = originalPos;
        yield return Wait(3);

        // Hide husband text, transition to next scene
        topText.gameObject.SetActive(false);
        // TODO: transition to scene 2
    }

    // Show dots to indicate length of current guess in the unlock phone puzzle.
    IEnumerator ShowDots(string guess) {
        Image dot1 = lockScreen.transform.Find("Dot1").GetComponent<Image>();
        Image dot2 = lockScreen.transform.Find("Dot2").GetComponent<Image>();
        Image dot3 = lockScreen.transform.Find("Dot3").GetComponent<Image>();
        dot1.color = new Color(255f, 255f, 255f, 0f);
        dot2.color = new Color(255f, 255f, 255f, 0f);
        dot3.color = new Color(255f, 255f, 255f, 0f);
        if (guess.Length >= 1) dot1.color = new Color(255f, 255f, 255f, 255f);
        if (guess.Length >= 2) dot2.color = new Color(255f, 255f, 255f, 255f);
        if (guess.Length >= 3) dot3.color = new Color(255f, 255f, 255f, 255f);
        yield return null;
    }

    // Wait until player finishes reading the cheating messages.
    IEnumerator WaitForScroll() {
        RawImage cheatingMessages = GameObject.Find("Cheating Messages").GetComponent<RawImage>();
        RectTransform transform = cheatingMessages.GetComponent<RectTransform>();
        while (transform.anchoredPosition.y < 750f) {
            yield return null;
        }
    }

    // Method for the eye blink effect. TODO (Katherine): modify to support blink closing.
    // https://github.com/tetreum/eyeblink/blob/master/EyeBlink.cs
    IEnumerator Blink(int n, bool opening)
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
            if (!(opening && currentBlink == n)) yield return moveEyelids(originalUpperEyelidPosition, originalLowerEyelidPosition, false, 0.70f);

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

    // Method to display str as top text for x seconds.
    IEnumerator ShowTopText(string str, float x) {
        topText.text = str;
        topText.gameObject.SetActive(true);
        yield return Wait(x);
        topText.gameObject.SetActive(false);
    }

    // Method to wait for x seconds.
    IEnumerator Wait(float x)
    {
        yield return new WaitForSeconds(x);
    }

    // Method to move the scripted camera from start to end in t seconds.
    IEnumerator MoveCameraToPosition(Vector3 start, Vector3 end, float t)
    {
        scriptedCamera.transform.position = start;
        Vector3 currentPos = start;
        float ratio = 0f;
        while (ratio < 1)
        {
            ratio += Time.deltaTime / t;
            scriptedCamera.transform.position = Vector3.Lerp(currentPos, end, ratio);
            yield return null;
        }
    }

    // Method to rotate the scripted camera from start to end in t seconds.
    IEnumerator RotateCameraToAngle(Quaternion start, Quaternion end, float t)
    {
        scriptedCamera.transform.rotation = start;
        Quaternion currentAngle = start;
        float ratio = 0f;
        while (ratio < 1)
        {
            ratio += Time.deltaTime / t;
            scriptedCamera.transform.rotation = Quaternion.Slerp(currentAngle, end, ratio);
            yield return null;
        }
    }

    // Method to move an object from start to end in t seconds.
    IEnumerator MoveObjectToPosition(GameObject obj, Vector3 start, Vector3 end, float t)
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

    // Method to fade to black/white.
    IEnumerator Fade(bool black, float speed)
    {
        Color c;
        if (black) c = new Color(0, 0, 0, 0);
        else c = new Color(0, 0, 0, 1);
        blackOverlay.color = c;
        blackOverlay.gameObject.SetActive(true);

        // White to black
        if (black) {
            while (blackOverlay.color.a < 1) {
                c.a += (speed * Time.deltaTime);
                blackOverlay.color = c;
                yield return null;
            }
        } 
        
        // Black to white
        else {
            while (blackOverlay.color.a > 0) {
                c.a -= (speed * Time.deltaTime);
                blackOverlay.color = c;
                yield return null;
            }
        }
    }

    // Method to show text on the canvas with a typewriter effect.
    IEnumerator ShowText(string fullText)
    {
        // Typewriter effect
        bottomText.gameObject.SetActive(true);
        for (int i = 0; i < fullText.Length + 1; i++)
        {
            string currentText = fullText.Substring(0, i);
            bottomText.text = currentText;
            yield return new WaitForSeconds(0.05f);
        }

        // Wait until space bar is hit
        while(!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }
        bottomText.gameObject.SetActive(false);
    }


    // Waits for an object named objectName to be clicked.
    IEnumerator WaitForClick(string objectName)
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
}
