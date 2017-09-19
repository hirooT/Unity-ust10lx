﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * When creating your message listeners you need to implement these two methods:
 *  - OnMessageArrived
 *  - OnConnectionEvent
 */

public class MessageListenerF : MonoBehaviour {

    public bool showDebugLog = true;
    private bool showOnce = false;

	void Update () {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F))
        {
            if (showDebugLog) showDebugLog = false;
            else showDebugLog = true;
        }
    }
    void OnMessageArrived(string msg)
    {
        if (!showOnce)
        {
            Debug.Log("Message arrived: " + msg);
            showOnce = true;
        }
        if (showDebugLog)
        {
            Debug.Log(" >>" + msg);
        }
    }
    void OnConnectionEvent(bool success)
    {
        if (showDebugLog)
        {
        if(success)
            Debug.Log("<color=green>Connection established.</color>");
        else
            Debug.Log("<color=maroon>Connection attempt failed or disconnection detected</color>");
        }
            
    }
}