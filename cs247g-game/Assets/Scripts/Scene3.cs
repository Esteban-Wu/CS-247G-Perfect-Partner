using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Scene3 : MonoBehaviour
{
    public Camera scriptedCamera;
    public FirstPersonController fpsController;
    public Canvas canvas;

    // Scene controller
    public GameObject sceneControllerObject;
    private SceneController sceneController;

    // Objects for Scene 3
    public GameObject rouen;
    public GameObject bedroomDoor;
    public GameObject flowers;
    public GameObject dinner;
    public GameObject cake;
    public GameObject phone;
    private GameObject cakePanel;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Code is trying to run Scene 3 (Night)");

        // Initialize private game objects
        sceneController = sceneControllerObject.GetComponent<SceneController>();
        cakePanel = canvas.transform.Find("Cake Panel").gameObject;

        // Initial state of passed-in objects
        scriptedCamera.gameObject.SetActive(false);
        fpsController.gameObject.SetActive(true);
        canvas.gameObject.SetActive(true);
        rouen.gameObject.SetActive(true);
        bedroomDoor.gameObject.SetActive(true);
        flowers.gameObject.SetActive(false);
        dinner.gameObject.SetActive(false);
        cake.gameObject.SetActive(false);
        phone.gameObject.SetActive(false);

        // Hide all panels in the canvas
        for (int j = 0; j < canvas.transform.childCount; j++) {
             canvas.transform.GetChild(j).gameObject.SetActive(false);
        }
        
        // Start Scene 3
        StartCoroutine(RunScene());
    }

    // Scene 3
    IEnumerator RunScene()
    {
        yield return TalkToRouen();
        yield return GoToLivingRoom();
        yield return ReceiveFlowers();
        yield return GoToDiningTable();
        yield return CakeActivity();
        yield return TalkAtDiningTable();

        // TODO: go to next scene
    }

    // Player's initial conversation with Rouen
    IEnumerator TalkToRouen() 
    {
        // Fade to transparent
        EnableFPSMovement(false);
        yield return sceneController.Fade(false, 0.2f);

        // Player walks towards Rouen
        EnableFPSMovement(true);
        yield return sceneController.WaitForPlayerInteract(rouen, 2f);
        EnableFPSMovement(false);

        // Talk
        yield return sceneController.ShowText("Rouen: \"Hello.\"", true, 1);
        yield return sceneController.ShowText("Who are you?", true, 0);
        yield return sceneController.ShowText("Rouen: \"Everything that you ever wanted. I'm your perfect partner.\"", true, 1);
        yield return sceneController.ShowText("You have the wrong person.", true, 0);
        yield return sceneController.ShowText("Rouen: \"So, your husband did not cheat on you and you did not order me.\"", true, 1);
        yield return sceneController.ShowText("......", true, 0);
    }

    // Player follows Rouen to living room
    IEnumerator GoToLivingRoom() 
    {
        // Rouen walks out door
        bedroomDoor.GetComponent<DoorScript>().OpenDoor();
        yield return sceneController.Wait(2f);
        Vector3 start = rouen.transform.position;
        Vector3 end = new Vector3(-0.723f, 2.87f, -0.713f);
        yield return sceneController.MoveObjectOverTime(rouen, start, end, 3);
        yield return sceneController.ShowText("Rouen: \"Follow me.\"", true, 1);

        // Player goes out door
        EnableFPSMovement(true);
        yield return sceneController.WaitForPlayerToGetNear(rouen, 2f);
        EnableFPSMovement(false);

        // Rouen walks downstairs
        start = end;
        end = new Vector3(-0.684f, 2.87f, 0.581f);
        yield return sceneController.MoveObjectOverTime(rouen, start, end, 1);
        EnableFPSMovement(true);
        start = end;
        end = new Vector3(-0.539f, 0.373f, 3.672f);
        yield return sceneController.MoveObjectOverTime(rouen, start, end, 3);
        Quaternion startAngle = rouen.transform.rotation;
        Quaternion endAngle = Quaternion.Euler(0, 180, 0);
        yield return sceneController.RotateObjectOverTime(rouen, startAngle, endAngle, 2);

        // Player goes downstairs
        yield return sceneController.WaitForPlayerToGetNear(rouen, 2f);

        // Rouen walks to living room
        start = end;
        end = new Vector3(3.659f, 0.213f, 2.994f);
        yield return sceneController.MoveObjectOverTime(rouen, start, end, 3);
        start = end;
        end = new Vector3(5.463f, 0.213f, 0.244f);
        yield return sceneController.MoveObjectOverTime(rouen, start, end, 2);

        // Player walks to living room
        yield return sceneController.WaitForPlayerToGetNear(rouen, 2f);
    }

    // Player receives flowers from Rouen
    IEnumerator ReceiveFlowers() 
    {
        // Rotate Rouen
        EnableFPSMovement(false);
        Quaternion startAngle = rouen.transform.rotation;
        Quaternion endAngle = Quaternion.Euler(0, 360, 0);
        yield return sceneController.RotateObjectOverTime(rouen, startAngle, endAngle, 2);

        // Talk
        yield return sceneController.ShowText("Rouen: \"This is worse than I thought.\"", true, 1);
        yield return sceneController.ShowText("Excuse me.", true, 0);
        yield return sceneController.ShowText("Rouen: \"Do not worry. Now that you have me. I will fix everything, I will give you the world.\"", true, 1);
        yield return sceneController.ShowText("Look, I appreciate you, but this was a mistake.", true, 0);
        yield return sceneController.ShowText("If you truly are the perfect partner, you would leave.", true, 0);
        yield return sceneController.ShowText("Rouen: \"One day.\"", true, 1);
        yield return sceneController.ShowText("What?", true, 0);

        // Flowers appear
        flowers.gameObject.SetActive(true);
        yield return sceneController.Wait(1f);

        // Talk
        yield return sceneController.ShowText("Rouen: \"Give me one day to prove that I am your perfect mate.\"", true, 1);
        yield return sceneController.ShowText("One dinner. That's it.", true, 0);
        yield return sceneController.ShowText("Rouen: \"One dinner is all I need.\"", true, 1);
        flowers.gameObject.SetActive(false);
    }

    // Player follows Rouen to the dining table
    IEnumerator GoToDiningTable()
    {
        // Rotate Rouen
        Quaternion startAngle = rouen.transform.rotation;
        Quaternion endAngle = Quaternion.Euler(0, 90, 0);
        yield return sceneController.RotateObjectOverTime(rouen, startAngle, endAngle, 1);

        // Rouen walks to dining table
        Vector3 start = rouen.transform.position;
        Vector3 end = new Vector3(4.725f, 0.213f, -1.199f);
        yield return sceneController.MoveObjectOverTime(rouen, start, end, 2);

        // Player follows Rouen to dining table
        EnableFPSMovement(true);
        yield return sceneController.WaitForPlayerToGetNear(rouen, 2f);
        EnableFPSMovement(false);

        // Dinner appears on the table
        yield return sceneController.ShowText("Rouen: \"Ready?\"", true, 1);
        dinner.gameObject.SetActive(true);

        // Talk
        yield return sceneController.ShowText("Rouen: \"Do you want to cook or just eat?\"", true, 1);
        yield return sceneController.ShowText("I kind of just want to decorate a cake, if that's okay. I always cook for my husband.", true, 0);
        yield return sceneController.ShowText("Rouen: \"Your wish is my desire.\"", true, 1);

        // Cake appears on the table
        cake.gameObject.SetActive(true);
        yield return sceneController.FlashObject(cake, 3, 2);
    }

    IEnumerator CakeActivity() 
    {
        // Player interacts with cake
        EnableFPSMovement(true);
        yield return sceneController.WaitForPlayerInteract(cake, 1.5f);
        EnableFPSMovement(false);

        // Open cake decoration panel
        yield return sceneController.ShowText("Rouen: \"I am excited to see what you will create.\"", true, 1);
        cakePanel.gameObject.SetActive(true);

        // Done button
        Button doneButton = cakePanel.transform.Find("DoneButton").GetComponent<Button>();
        bool cakeDone = false;

        // Wait until player finishes cake
        do {
            if(Input.GetMouseButtonDown(0)) {
                // Get the currently selected button
                GameObject curr = EventSystem.current.currentSelectedGameObject;

                // Done button
                if (curr != null && doneButton.name.Equals(curr.name)) {
                    cakeDone = true;
                }

                // TODO: handle drag decorations
            }
            yield return null;
        } while (!cakeDone);
        cakePanel.gameObject.SetActive(false);
        sceneController.DehighlightObject(cake);
    }

    IEnumerator TalkAtDiningTable()
    {
        // Move Rouen, phone, and scripted camera to table
        phone.gameObject.SetActive(true);
        sceneController.MoveObjectToPosition(rouen, new Vector3(5.299f, 0.033f, -1.688f));
        sceneController.MoveObjectToPosition(scriptedCamera.gameObject, new Vector3(6.705f, 1.325f, -1.738f));
        sceneController.RotateObjectToAngle(scriptedCamera.gameObject, Quaternion.Euler(15f, -90f, 0f));
        sceneController.MoveObjectToPosition(phone, new Vector3(6.232f, 0.981f, -1.896f));
        sceneController.EnableFPSController(false);

        // Talk
        yield return sceneController.ShowText("Rouen: \"The cake is almost as beautiful as you. Your husband is a fool.\"", true, 1);
        yield return sceneController.ShowText("You do not know me.", true, 0);
        yield return sceneController.ShowText("Rouen: \"I know that your smile could brighten up any room.\"", true, 1);
        yield return sceneController.ShowText("Is your game plan to spend this dinner simply complimenting me.", true, 0);
        yield return sceneController.ShowText("Rouen: \"No, it is to be the mate that you deserve.\"", true, 1);
        yield return sceneController.ShowText("Look, I am sorry. I see you are trying and this is amazing, but...", true, 0);
        yield return sceneController.ShowText("I think I just need an honest convo with my husband. Maybe it was a misunderstanding.", true, 0);
        yield return sceneController.ShowText("Rouen: \"Misunderstanding? There were texts.\"", true, 1);
        yield return sceneController.ShowText("I know... I just. It is so hard to process. I think I need more information.", true, 0);
        yield return sceneController.ShowText("Rouen: \"That is understandable. I am the same way, I need more information for it to be settled in.\"", true, 1);
        yield return sceneController.ShowText("So you think I should talk to him.", true, 0);
        yield return sceneController.ShowText("Rouen: \"No, I think that we need to do some digging.\"", true, 1);
        yield return sceneController.ShowText("We?", true, 0);
        yield return sceneController.ShowText("Rouen: \"Yes, we are a team. Now, do you have your husband's phone on you?\"", true, 1);
        yield return sceneController.ShowText("Yeah.", true, 0);
        
        // Wait for player to click phone
        sceneController.HighlightObject(phone, 2);
        yield return sceneController.WaitForPlayerInteract(phone, 50f);
        Debug.Log("phone interacted");
        yield return null;
    }

    // Enable/disable FPS controller movement
    void EnableFPSMovement(bool state)
    {
        fpsController.GetComponent<FirstPersonController>().enabled = state;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
