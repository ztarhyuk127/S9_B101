using ItemName;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDropItem : MonoBehaviour
{
    Enemy enemy;
    GameObject targetItem;

    [System.Serializable]
    public class Items
    {
        public GameObject item;
        public int weight;
    }
    public List<Items> ItemList = new List<Items>();

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    protected GameObject pickItem()
    {
        int sum = 0;
        foreach (Items item in ItemList)
        {
            sum += item.weight;
        }
        int random = Random.Range(0, sum);
        for (int i = 0; i < ItemList.Count; i++)
        {
            Items targetItem = ItemList[i];
            if (targetItem.weight > random) return ItemList[i].item;
            else random -= targetItem.weight;
        }
        return null;
    }

    public void ItemCheck(Vector3 pos)
    {
        targetItem = pickItem();
        if (targetItem == null) return;
        Debug.Log("Items/First_aid_kit/prefabs/"+ targetItem.name);
        PhotonNetwork.Instantiate("Items/First_aid_kit/prefabs/" + targetItem.name, pos, Quaternion.identity);
    }
}
    



    
