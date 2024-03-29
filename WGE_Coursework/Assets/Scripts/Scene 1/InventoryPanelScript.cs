﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class InventoryPanelScript : MonoBehaviour {

    // Variables 
    Image itemSprite;
    Text itemNameText;
    Text itemCountText;

    public GameObject inventoryPanel;
    public VoxelChunk voxChunk;
    public FirstPersonController fpscript;
    public PlayerScript playerScript;
    public InputField searchBar;
    public string search;

    public bool invPanelOpen = false;
    public bool isSortedHighToLow = false;
    public bool isSortedAtoZ = false;

    public Sprite[] blockImage;
    public GameObject[] panel;
    InventoryItemScript[] inventoryItems;
    public string[] blockName;

    delegate int SortType(InventoryItemScript a, InventoryItemScript b);




    // Use this for initialization
    void Start()
    {
        inventoryPanel.SetActive(false);
        invPanelOpen = false;
        inventoryItems = new InventoryItemScript[4];
    }




    // Update is called once per frame
    void Update()
    {
        // update blockAmounts array
        for (int i = 0; i < 4; i++)
        {
            inventoryItems[i] = new InventoryItemScript();
            inventoryItems[i].itemCount = playerScript.blockCounts[i];
            inventoryItems[i].itemName = blockName[i];
            inventoryItems[i].itemImage = blockImage[i];
        }

        // if the 0 key is pressed
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            if (invPanelOpen)
            {
                invPanelOpen = false;
                // if the inventory panel is open, close it
                inventoryPanel.SetActive(false);
                // freeze the cursor
                voxChunk.LockCursor();
            }
            else
            {
                invPanelOpen = true;
                // if the inventory panel is closed, open it
                inventoryPanel.SetActive(true);
                SortByAmount();
                searchBar.text = "";
                // unfreeze the cursor
                voxChunk.UnlockCursor();
            }
        }

        // if there are no panels open
        if (!invPanelOpen && !voxChunk.panelOpen)
        {
            // allow the player to move
            fpscript.m_RunSpeed = 10;
            fpscript.m_WalkSpeed = 5;
        }
        // if either panel is open
        else if (invPanelOpen || voxChunk.panelOpen)
        {
            // freeze the player
            fpscript.m_RunSpeed = 0;
            fpscript.m_WalkSpeed = 0;
        }

        if (invPanelOpen)
        {
            // search input
            search = searchBar.text;
            SearchByName(search.ToLower());
        }
        
    }





    ///////////////////////////////////////////////////////////////////////////// Start of Sorting By Name ///////////////////////////////////////////////////////////////////////////////////////
    



    public void SortAlphabetically()
    {
        // Empty the sortBy variable
        SortType sortBy = null;

        // if the inventory is already sorted A to Z
        if (isSortedAtoZ)
        {
            // sort it by Z to A
            isSortedAtoZ = false;
            sortBy = new SortType(SortZtoA);
        }
        else // if the inventory is sorted Z to A
        {
            // sort it by A to Z
            isSortedAtoZ = true;
            sortBy = new SortType(SortAtoZ);
        }

        Sort(inventoryItems, sortBy);
    }




    int SortAtoZ(InventoryItemScript a, InventoryItemScript b)
    {
        // compare a to b
        return string.Compare(a.itemName, b.itemName);
    }




    int SortZtoA(InventoryItemScript a, InventoryItemScript b)
    {
        // compare b to a
        return string.Compare(b.itemName, a.itemName);
    }




    ///////////////////////////////////////////////////////////////////////////// End of Sorting By Name ///////////////////////////////////////////////////////////////////////////////////////




    ///////////////////////////////////////////////////////////////////////////// Start of Sorting By Amount ///////////////////////////////////////////////////////////////////////////////////////




    public void SortByAmount()
    {
        SortType sortBy = null;
        // if sorted high to low
        if (isSortedHighToLow)
        {
            // sort low to high
            isSortedHighToLow = false;
            sortBy = new SortType(SortLowToHigh);
        }
        else
        {
            // sort high to low
            isSortedHighToLow = true;
            sortBy = new SortType(SortHighToLow);
        }

        Sort(inventoryItems, sortBy);
    }




    int SortHighToLow(InventoryItemScript a, InventoryItemScript b)
    {
        if (a.itemCount > b.itemCount)
        {
            return -1;
        }
        else
        {
            return 1;
        }
    }




    int SortLowToHigh(InventoryItemScript a, InventoryItemScript b)
    {
        if (a.itemCount < b.itemCount)
        {
            return -1;
        }
        else
        {
            return 1;
        }
    }




    InventoryItemScript[] Sort(InventoryItemScript[] amounts, SortType sortType)
    {
        // if there is only one number, don't try to split into two arrays
        if (amounts.Length <= 1) return amounts;

        int arrayLength = amounts.Length / 2;

        // split into two arrays
        InventoryItemScript[] firstHalf = new InventoryItemScript[arrayLength];
        InventoryItemScript[] secondHalf = new InventoryItemScript[arrayLength];

        for (int i = 0; i < amounts.Length; i++)
        {
            if (i <= arrayLength - 1)
            {
                firstHalf[i] = amounts[i];
            }
            else if (i > arrayLength - 1)
            {
                secondHalf[i - arrayLength] = amounts[i];
            }
        }

        // call recursively
        firstHalf = Sort(firstHalf, sortType);
        secondHalf = Sort(secondHalf, sortType);

        if (!isSortedHighToLow)
        {
            // merge the two arrays, according to the sort type
            amounts = Merge(firstHalf, secondHalf, sortType);
        }
        else
        {
            // merge the two arrays, according to the sort type
            amounts = Merge(firstHalf, secondHalf, sortType);
        }

        // once the sorting is finished
        if (amounts.Length == 4)
        {
            for (int i = 0; i < amounts.Length; i++)
            {
                // reassign text and 
                panel[i].GetComponent<Image>().sprite = amounts[i].itemImage;
                panel[i].GetComponentInChildren<Text>().text = amounts[i].itemName + ": " + amounts[i].itemCount;
            }
            
        }
        
        return amounts;
    }




    InventoryItemScript[] Merge(InventoryItemScript[] left, InventoryItemScript[] right, SortType sortType)
    {
        // create a new array for the merged items
        InventoryItemScript[] merged = new InventoryItemScript[left.Length + right.Length];
        int i, j, m;
        i = j = m = 0;

        // while both i and j are less than the left and right array lengths
        while (i < left.Length && j < right.Length)
        {
            int result = sortType(left[i], right[j]);

            if (result == -1)
            {
                merged[m] = left[i];
                i++;
                m++;
            }
            else
            {
                merged[m] = right[j];
                j++;
                m++;
            }
        }

        if (i < left.Length)
        {
            // add the rest of the elements in a to the end of merged
            for (int k = i; k < left.Length; k++)
            {
                merged[m] = left[k];
                m++;
            }
        }
        else
        {
            // add the rest of the elements in b to the end of merged
            for (int k = j; k < right.Length; k++)
            {
                merged[m] = right[k];
                m++;
            }
        }

        return merged;
    }




    InventoryItemScript[] MergeHighToLow(InventoryItemScript[] a, InventoryItemScript[] b)
    {
        InventoryItemScript[] merged = new InventoryItemScript[a.Length + b.Length];
        int i, j, m;
        i = j = m = 0;

        while (i < a.Length && j < b.Length)
        {
            if (a[i].itemCount >= b[j].itemCount)
            {
                merged[m] = a[i];
                i++;
                m++;
            }
            else
            {
                merged[m] = b[j];
                j++;
                m++;
            }
        }

        if (i < a.Length)
        {
            // add the rest of the elements in a to the end of merged
            for (int k = i; k < a.Length; k++)
            {
                merged[m] = a[k];
                m++;
            }
        }
        else
        {
            // add the rest of the elements in b to the end of merged
            for (int k = j; k < b.Length; k++)
            {
                merged[m] = b[k];
                m++;
            }
        }

        return merged;
    }




    ///////////////////////////////////////////////////////////////////////////// End of Sorting By Amount ///////////////////////////////////////////////////////////////////////////////////////




    public void SearchByName(string playerInput)
    { 
        for (int i = 0; i < blockName.Length; i++)
        {
            // if player input is not null
            if (playerInput != "")
            {
                if (!panel[i].GetComponentInChildren<Text>().text.ToLower().Contains(playerInput))
                {
                    // Reduce opacity of all panels that do not match the player input
                    Color color = panel[i].GetComponent<Image>().color;
                    color.a = 0.1f;
                    panel[i].GetComponent<Image>().color = color;
                }
                else
                {
                    // if the item matches the player input, make opacity 1
                    Color color = panel[i].GetComponent<Image>().color;
                    color.a = 1;
                    panel[i].GetComponent<Image>().color = color;
                }
            }
            else
            {
                // if there is no input, make all the panels' opacity 1 (i.e. 100% opaque)
                Color color = panel[i].GetComponent<Image>().color;
                color.a = 1;
                panel[i].GetComponent<Image>().color = color;
            }
            
        }
    }
}
