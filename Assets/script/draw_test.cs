using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class draw_test : MonoBehaviour {

    //public GameObject line;
    public GameObject pos;
    float val;
    public int step = 0;
    int end_step = 0;
    Vector2 pos_540;
    // Use this for initialization
    void Start () {
        end_step = getdis_ethernet.end_step;
		for(int i = 0; i < end_step; i++)
        {
            Instantiate(pos, pos.transform);
        }
	}
	
	// Update is called once per frame
	void Update () {
        //line.transform.Rotate(0, 0, getdis_ethernet.start_step / 1080, Space.Self);
        /*
        val = getdis_ethernet.value * 5;
        pos_540.y = -val * Mathf.Cos(135) - 540;
        pos_540.x = -val * Mathf.Sin(135);
        pos.transform.localPosition = pos_540;
        */
        //Debug.Log(pos_540);
        /*
        if(val < 700)
        {
            float yscale = val / 200;
            Debug.Log("real distance: " + val + "  scale: " + yscale);
            line.transform.localScale = new Vector2(0.001f, yscale);
        }
        */
        val = getdis_ethernet.value;
        for(int i = 0; i < end_step; i++)
        {
            pos.transform.GetChild(i).transform.position = DotPos(val, StepToDegree(i));
        }
    }

    float StepToDegree(int step)
    {
        float degree = 0;
        degree = step / 4 - 45;
        Debug.Log(degree);
        return degree;
    }
    Vector2 DotPos(float dis, float degree) {
        Vector2 pos;
        dis *= 5;
        pos.x = -dis * Mathf.Sin(degree);
        pos.y = -dis * Mathf.Cos(degree) - 540;
        return pos;
    }
}
