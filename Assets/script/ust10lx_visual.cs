using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System;
using SCIP_library;
using UnityEngine.UI;

public class ust10lx_visual : MonoBehaviour {

    private int GET_NUM = 1;
    public int start_step = 0;
    public int end_step = 1080;
    public string ip_address = "192.168.0.10";
    public int port_number = 10940;

    public Transform seneor;
    public GameObject prefab;
    private int total_step = 1080;
    private Thread setuptcp;
    private Thread receivedata;
    private bool ipconfig = false;
    private bool tcpconnect = false;
    TcpClient urg;
    NetworkStream stream;

    private int distance_pool;
    public GameObject[] pos;
    Vector2 point = new Vector2();

    void Start () {
        Get_connect_information(ip_address, port_number);
        receivedata = new Thread(Receive_data);
        receivedata.IsBackground = true;
        receivedata.Start();
        
    }
	
	void Update () {
        prefab.transform.position = point;
    }

    public Vector2 dis_to_pos(int distance)
    {
        Vector2 center = new Vector2(540, 0);
        int dis = distance;
        Vector2 position = new Vector2();
        position.x = center.x + dis * Mathf.Cos(-45 + 0.25f) / -180 * Mathf.PI;
        position.y = center.y + dis * Mathf.Sin(-45 + 0.25f) / -180 * Mathf.PI;

        return position;
    }
    private void Get_connect_information(string ip, int port)
    {
        Debug.Log("<color=green>Connect setting = IP Address: </color>" + ip_address + "<color=green> Port number: </color>" + port_number.ToString());
        ipconfig = true;
        StartCoroutine(Setup_tcp());
        //Setup_tcp();
    }

    IEnumerator Setup_tcp()
    {
        try
        {
            if (ipconfig)
            {
                urg = new TcpClient(ip_address, port_number);
                stream = urg.GetStream();
                Debug.Log("<color=green>Success!</color>");
                tcpconnect = true;
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("<color=red>Error! </color>" + e);
        }
        yield return null;
    }

    private void Receive_data()
    {
        while (tcpconnect)
        {
            try
            {
                write(stream, SCIP_Writer.SCIP2());
                read_line(stream); // ignore echo back
                write(stream, SCIP_Writer.MD(start_step, end_step, 1, 0, 0));
                read_line(stream);  // ignore echo back
                List<long> distances = new List<long>();
                long time_stamp = 0;
                for (int i = 0; i < GET_NUM; ++i)
                {
                    string receive_data = read_line(stream);

                    if (!SCIP_Reader.MD(receive_data, ref time_stamp, ref distances))
                    {
                        //Debug.Log("<color=blue>Receive Data: </color>" + receive_data);
                        break;
                    }

                    if (distances.Count == 0)
                    {
                        //Debug.Log("<color=red>Receive Data: </color>" + receive_data);
                        continue;
                    }
                    // show distance data
                    for (int k = 0; k < 1080; k++)
                    {
                        distance_pool = (int)distances[k] / 10;
                        if(k == 540)
                        {
                            
                            point = dis_to_pos(distance_pool);
                            
                            //Debug.Log(point);

                        }
                        
                        //Debug.Log("<color=yellow>Step: </color>" + k + "<color=teal>Get distance: </color>" + distances[k] / 10 + "cm");
                        //tcpconnect = false;
                        //write(stream, SCIP_Writer.QT());    // stop measurement mode
                        //read_line(stream); // ignore echo back
                        //stream.Close();
                        //urg.Close();
                        //Debug.Log("<color=green>Sensor close.</color>");
                        //break;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
            }
            //Debug.Log("<color=yellow>sleep</color>");
            Thread.Sleep(10);
        }

    }

    static bool write(NetworkStream stream, string data)
    {
        if (stream.CanWrite)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            stream.Write(buffer, 0, buffer.Length);
            //Debug.Log(data);
            return true;
        }
        else
        {
            return false;
        }
    }

    static string read_line(NetworkStream stream)
    {
        if (stream.CanRead)
        {
            StringBuilder sb = new StringBuilder();
            bool is_NL2 = false;
            bool is_NL = false;
            do
            {
                char buf = (char)stream.ReadByte();
                if (buf == '\n')
                {
                    if (is_NL)
                    {
                        is_NL2 = true;
                    }
                    else
                    {
                        is_NL = true;
                    }
                }
                else
                {
                    is_NL = false;
                }
                sb.Append(buf);
            } while (!is_NL2);

            return sb.ToString();
        }
        else
        {
            return null;
        }
    }
}
