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

public class getdis_ethernet : MonoBehaviour {

    private int GET_NUM = 1;
    public int start_step = 540; //面對sensor左-右 >> 0-1080step
    [Tooltip("can't smaller than startstep.")]
    public int end_step = 541;
    [Tooltip("default:192.168.0.10")]
    public string ip_address;
    [Tooltip("default:10940")]
    public int port_number;

    public int range = 500; //單位:mm
    public bool showDebugLog = false;

    private Thread setuptcp;
    private Thread receivedata;
    private bool ipconfig = false;
    private bool tcpconnect = false;
    TcpClient urg;
    NetworkStream stream;

    // Use this for initialization
    void Start() {
        Get_connect_information(ip_address, port_number);
        receivedata = new Thread(Receive_data);
        receivedata.IsBackground = true;
        receivedata.Start();
        
    }
    // Update is called once per frame
    void Update() {
        if(Input.GetKeyDown(KeyCode.D) && Input.GetKey(KeyCode.LeftControl))
        {
            if (showDebugLog) showDebugLog = false;
            else showDebugLog = true;
        }
    }
    private void Get_connect_information(string ip, int port)
    {
        Debug.Log("<color=green>Connect setting = IP Address: </color>" + ip_address + "<color=green> Port number: </color>" + port_number.ToString());
        ipconfig = true;
        StartCoroutine(Setup_tcp());
        //Setup_tcp();
    }
    IEnumerator Setup_tcp() {
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
        catch (System.Exception e){
            Debug.Log("<color=red>Error! </color>" + e);
        }
        yield return null;
    }
    private void Receive_data() {
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
                    for (int k = 0; k < distances.Count; k++)
                    {
                        if(showDebugLog) Debug.Log("k: " + k +"  distance: " + distances[k] / 10 + "cm");

                        if ((int)distances[k] < range)
                        {
                            Debug.Log("<color=teal>Get distance: </color>" + distances[k] / 10 + "cm");
                            tcpconnect = false;
                            write(stream, SCIP_Writer.QT());    // stop measurement mode
                            read_line(stream); // ignore echo back
                            stream.Close();
                            urg.Close();
                            Debug.Log("<color=green>Sensor close.</color>");
                            break;
                        }

                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.Log("<color=red>Error!</color>" + e);
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