using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class comm2 : MonoBehaviour
{
    public Text inputIP2;
    public Text inputPort2;

    public Text timeText;
    private static comm2 instance;
    private Thread receiveThread;
    private UdpClient receiveClient;
    private IPEndPoint receiveEndPoint;
    string ip = "";
    int receivePort =0; //= 50002;
    private bool isInitialized;
    private Queue receiveQueue;
    private Material m_Material;
    int seg = 0;

    string[] wday = new string[7] { " Lunes", " Martes", " Miercoles", " Jueves", " Viernes", " Sabado", " Domingo" };
    string[] Months = new string[12] { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };

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
                //string text = Encoding.UTF8.GetString(data);
                //temperature = System.BitConverter.ToSingle(data, 0);
                //Debug.Log(data[0].ToString() + " seg");
                //Debug.Log(data[1].ToString() + " min");
                //Debug.Log(data[2].ToString() + " hora");

                if (data.Length >= 7)
                {
                    string time = data[0] + ":" + data[1] + ":" + data[2] + wday[data[3]] + ", " + data[4] + " de " + Months[data[5]] + " de " + "20"+ data[6];
                    receiveQueue.Enqueue(time);
                }
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
                string dequeued = receiveQueue.Dequeue().ToString();
                Debug.Log(dequeued);
                timeText.text = dequeued;
            }
        }
        
    }
    public void StoreInfoConnections()
    {
        ip = inputIP2.text;
        receivePort = int.Parse(inputPort2.text);
    }
}
