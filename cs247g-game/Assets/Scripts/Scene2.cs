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

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Code is trying to run Scene 2 (Day)");

        // Initialize private game objects
        sceneController = sceneControllerObject.GetComponent<SceneController>();
        
        // Initialize state of passed-in objects
        scriptedCamera.gameObject.SetActive(false);
        fpsController.gameObject.SetActive(true);
        canvas.gameObject.SetActive(true);

        // Start Scene 2
        StartCoroutine(RunScene());
    }

    IEnumerator RunScene() {
        // TODO: put the main code of scene 2 here
        

        // To wait for x seconds: sceneController.Wait(x);
        // To show bottom text: sceneController.ShowText(str, speed);
        // To move objects: 
        //     1) sceneController.MoveObjectToPosition(obj, pos)
        //     2) sceneController.MoveObjectOverTime(obj, start, end, duration);
        // Etc. Refer to SceneController.cs for more useful methods :D


        // Transition to scene 3 by calling: 
        //     1) Variables.currentLevel = 3;
        //     2) SceneManager.LoadScene("NightScene");
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
