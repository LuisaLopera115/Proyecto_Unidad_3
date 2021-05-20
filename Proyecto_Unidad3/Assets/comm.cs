using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class comm : MonoBehaviour
{
    public Text inputIP1;
    public Text inputPort1;
    public Text temp;

    private static comm instance;
    private Thread receiveThread;
    private UdpClient receiveClient;
    private IPEndPoint receiveEndPoint;
    string ip = ""; // = "192.168.1.8";
    int receivePort=0; // = 50001;
    private bool isInitialized;
    private Queue receiveQueue;
    public GameObject  sun, sunCloud, cloud, moon, moonRain;
    private Material m_Material;
    float temperature = 0;

    private void Awake()
    {
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        instance = this;
        receiveEndPoint = new IPEndPoint(IPAddress.Parse(ip), receivePort);
        receiveClient = new UdpClient(receivePort);
        receiveQueue = Queue.Synchronized(new Queue());
        receiveThread = new Thread(new ThreadStart(ReceiveDataListener));
        receiveThread.IsBackground = true;
        receiveThread.Start();
        isInitialized = true;
    }

    private void ReceiveDataListener()
    {
        while (true)
        {
            try
            {
                byte[] data = receiveClient.Receive(ref receiveEndPoint);
                string text = Encoding.UTF8.GetString(data);
                temperature = System.BitConverter.ToSingle(data, 0);
                Debug.Log(temperature);
                receiveQueue.Enqueue(temperature);
                //SerializeMessage(text);
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.ToString());
            }
        }
    }

    private void SerializeMessage(string message)
    {
        try
        {
            string[] chain = message.Split(' ');
            string key = chain[0];
            float value = 0;
            if (float.TryParse(chain[1], out value))
            {
                receiveQueue.Enqueue(value);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private void OnDestroy()
    {
        TryKillThread();
    }

    private void OnApplicationQuit()
    {
        TryKillThread();
    }

    private void TryKillThread()
    {
        if (isInitialized)
        {
            receiveThread.Abort();
            receiveThread = null;
            receiveClient.Close();
            receiveClient = null;
            Debug.Log("Thread killed");
            isInitialized = false;
        }
    }

    void Update()
    {
        if (isInitialized)
        {
            if (receiveQueue.Count != 0)
            {
                float counter = (float)receiveQueue.Dequeue();
                temp.text = counter.ToString() + "°";

                if (counter > 15 && counter <= 18)
                {
                    moonRain.SetActive(true);
                    moon.SetActive(false);
                    cloud.SetActive(false);
                    sunCloud.SetActive(false);
                    sun.SetActive(false);
                }
                if (counter > 18 && counter <= 21)
                {
                    moonRain.SetActive(false);
                    moon.SetActive(true);
                    cloud.SetActive(false);
                    sunCloud.SetActive(false);
                    sun.SetActive(false);
                }

                if (counter > 21 && counter <= 23)
                {
                    moonRain.SetActive(false);
                    moon.SetActive(false);
                    cloud.SetActive(true);
                    sunCloud.SetActive(false);
                    sun.SetActive(false);
                }

                if (counter > 23 && counter <= 25)
                {
                    moonRain.SetActive(false);
                    moon.SetActive(false);
                    cloud.SetActive(false);
                    sunCloud.SetActive(true);
                    sun.SetActive(false);
                }

                if (counter > 25 && counter <= 28)
                {
                    moonRain.SetActive(false);
                    moon.SetActive(false);
                    cloud.SetActive(false);
                    sunCloud.SetActive(true);
                    sun.SetActive(true);
                }
            }
        }
        
    }

    public void StoreInfoConnections()
    {
        Debug.Log(ip);
        ip = inputIP1.text;
        receivePort = int.Parse(inputPort1.text);
    }
}
