using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Character : Entity
{
    [Header("Slots")]
    public InventoryItem[] equipments = new InventoryItem[5];
    public InventoryItem[] inventory = new InventoryItem[24];

    [Header("Equipment Sockets")]
    public Transform handR_Socket;
    [Header("Sounds")]
    public AudioClip stepSound;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < Camera.main.GetComponent<PlayerController>().gameData.items.Count; i++)
        {
            AddItemToInventory(new InventoryItem(i, 1));
        }

        equipments[2] = new InventoryItem(5, 1);

        RefreshView();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        //transform.Rotate(Vector3.up * direction * 4);
    }

    void Step(AnimationEvent e)
    {
        if (e.animatorClipInfo.weight > 0.5f)
        {
            GetComponent<AudioSource>().pitch = Random.Range(1f, 1.2f);
            GetComponent<AudioSource>().PlayOneShot(stepSound);
        }
    }

    void Hit()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + transform.up, transform.TransformDirection(Vector3.forward), out hit, 2))
        {
            Entity entity;
            if (hit.collider.gameObject.TryGetComponent<Entity>(out entity))
            {
                //hit.collider.gameObject.GetComponent<Entity>().ApplyDamage(equipment?.damage ?? 10);
                Camera.main.GetComponent<UConnect>().CallEvent("send", "World", "ApplyDamage", name, entity.name, (10).ToString("n2"));
            }
        }
        
        Debug.DrawRay(transform.position + transform.up, transform.TransformDirection(Vector3.forward) * 2, Color.red, 2);
    }

    public int AddItemToInventory(InventoryItem item)
    {
        int quantity = item.quantity;
        var gameData = Camera.main.GetComponent<PlayerController>().gameData;

        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i].index == item.index)
            {
                int count = inventory[i].quantity + quantity;
                if (count > gameData.items[item.index].stackSize)
                {
                    quantity = (inventory[i].quantity + quantity) - gameData.items[item.index].stackSize;
                    inventory[i].quantity = gameData.items[item.index].stackSize;
                }
                else
                {
                    inventory[i].quantity = count;
                    quantity = 0;
                    break;
                }
            }
            else if (inventory[i].quantity == 0)
            {
                inventory[i].index = item.index;

                if (quantity > gameData.items[item.index].stackSize)
                {
                    quantity = quantity - gameData.items[item.index].stackSize;
                    inventory[i].quantity = gameData.items[item.index].stackSize;
                }
                else
                {
                    inventory[i].quantity = quantity;
                    quantity = 0;
                    break;
                }
            }
        }

        return quantity;
    }

    public void RefreshView()
    {
        if (handR_Socket.childCount > 0)
            Destroy(handR_Socket.GetChild(0).gameObject);

        if (equipments[2].quantity > 0)
        {
            Instantiate(Camera.main.GetComponent<PlayerController>().gameData.items[equipments[2].index].equipmentPrefab, handR_Socket);
        }
    }
}
