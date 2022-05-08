using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    public Camera scriptedCamera;
    public FirstPersonController fpsController;
    public Canvas canvas;
    public GameObject upperEyelid;
    public GameObject lowerEyelid;

    private Image blackOverlay;
    private TextMeshProUGUI bottomText;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize private game objects
        blackOverlay = canvas.transform.Find("Black Overlay").GetComponent<Image>();
        bottomText = canvas.transform.Find("Bottom Text").GetComponent<TextMeshProUGUI>();

        // Start Scene 1
        StartCoroutine(Scene1());
    }

    // This is a coroutine for Scene 1.
    IEnumerator Scene1() 
    {
        yield return PanMemoryPhotos();
        yield return WakeUpToPhoneBuzz();

        // Debug.Log("Switch to first-person controller");
        scriptedCamera.gameObject.SetActive(false);
        fpsController.gameObject.SetActive(true);
        // yield return null;
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

        // TODO (Katherine): phone buzz, click phone, unlock phone, faint, end scene
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

        blackOverlay.gameObject.SetActive(false);
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
}
