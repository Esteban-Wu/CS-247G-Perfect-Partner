using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public InventoryObject inventory;
    bool inRange = false;
    private Collider collision;
    
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
        var item = collision.GetComponent<Item>();
        if (inRange && item && Input.GetKeyDown(KeyCode.E))
        {
            inventory.AddItem(item.item, 1);
            Destroy(collision.gameObject);
            collision = null;
            inRange = false;
        }
    }


}
