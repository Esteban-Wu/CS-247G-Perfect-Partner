using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public InventoryObject inventory;
    public SceneController sceneController;
    private bool inRange;
    private Collider collision;
    private Item item;
    
    void Start()
    {
        inRange = false;
        collision = null;
        item = null;
    }
    
    public void OnTriggerEnter(Collider other)
    {
        collision = other;
        inRange = true;

        // Highlight item
        item = other.GetComponent<Item>();
        if (item)
        {
            sceneController.HighlightObject(other.gameObject, 8);
        }

    }

    public void OnTriggerExit(Collider other)
    {
        // Dehighlight item
        item = other.GetComponent<Item>();
        if (item)
        {
            sceneController.DehighlightObject(other.gameObject);
        }

        collision = null;
        inRange = false;
    }

    private void OnApplicationQuit()
    {
        inventory.Container.Clear();
    }

    void Update()
    {
        if (collision != null) {
            item = collision.GetComponent<Item>();
        }

        // Collect item
        if (inRange && item && Input.GetKeyDown(KeyCode.E))
        {
            inventory.AddItem(item.item, 1);
            Destroy(collision.gameObject);
            collision = null;
            inRange = false;
        }
    }
}
