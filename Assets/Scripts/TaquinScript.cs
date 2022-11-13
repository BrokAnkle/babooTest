using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaquinScript : MonoBehaviour
{
    GameObject grid;

    void Start()
    {
        int tileToHide = Random.Range(0, transform.childCount); //select randomly a tile that will be taken out
        transform.GetChild(tileToHide).gameObject.SetActive(false);

        GameManager.instance.hiddenTile = transform.GetChild(tileToHide).GetComponent<TileScript>();    //give reference of the deactivated tile to the GameManager
        PlaceTiles();   //random shuffle of the tile to the slots
    }

    void PlaceTiles()
    {
        grid = GameObject.Find("Grid");

        List<SlotScript> slots = new List<SlotScript>();
        //iterate through every slots to add them to the list above
        for (int i = 0; i < grid.transform.childCount; i++)
            slots.Add(grid.transform.GetChild(i).GetComponent<SlotScript>());

        //iterate through every tile to put each into a random slot
        for (int i = 0; i < transform.childCount; i++)
        {
            TileScript tile = transform.GetChild(i).GetComponent<TileScript>();
            if (!tile.gameObject.activeSelf) continue;  //ignore the hidden tile
            //Debug.Log("Slots remaining " + slots.Count + "current : " + index, this);

            int index = Random.Range(0, slots.Count);   //random slot selection
            tile.transform.position = slots[index].transform.position;  //the current tile goes to the random slot position
            slots[index].holdingTile = tile.winPosition;    //tell the slot that its holding the current tile
            slots.Remove(slots[index]); //remove the slot from the list since its filled
        }
    }
}
