﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

    // Variables
    bool empty;
    public int blockNum;
    public int[] blockCounts;
    public delegate void EventSetBlock(Vector3 index, int blockType);
    public static event EventSetBlock OnEventSetBlock;




    // Update is called once per frame
    void Update ()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 v;
            if (PickBlock(out v, 4, false))
            {
                // set the block type to 0
                OnEventSetBlock(v, 0);
            }
        }
        else if (Input.GetButtonDown("Fire2"))
        {
            if (blockCounts[blockNum-1] > 0)
            {
                Vector3 v;
                if (PickBlock(out v, 4, true))
                {
                    // sets a block of type blockNum down
                    OnEventSetBlock(v, blockNum);
                    blockCounts[blockNum-1]--;
                }
            }
        }

        if (transform.position.y < -10)
        {
            transform.position = new Vector3(0, 5, 0);
        }
    }


    

    bool PickBlock(out Vector3 v, float dist, bool empty)
    {
        v = new Vector3();
        
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, dist))
        {
            if (empty == true)
            {
                v = hit.point + hit.normal / 2;
            }
            else
            {
                
                v = hit.point - hit.normal / 2;
            }
            // round down to get the index of the empty
            v.x = Mathf.Floor(v.x);
            v.y = Mathf.Floor(v.y);
            v.z = Mathf.Floor(v.z);
            return true;
        }
        return false;
    }

}
