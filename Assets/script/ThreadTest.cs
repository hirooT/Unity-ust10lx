using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class ThreadTest : MonoBehaviour {

    private Thread thread;
    //bool isend = false;
    // Use this for initialization
    void Start () {
        thread = new Thread(Call);
        thread.IsBackground = true;
        thread.Start();

	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log("Update.");
        //Call();
	}

    private void Call()
    {
        int count = 0;
        while (count <= 5)
        {
            count++;
            Debug.Log(count);
            Thread.Sleep(100);
            if (count == 6) count = 1;
        }

    }
}
