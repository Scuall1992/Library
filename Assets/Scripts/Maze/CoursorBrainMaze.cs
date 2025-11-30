using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoursorBrainMaze : MonoBehaviour
{
    [SerializeField] private Transform _pointer;
    [SerializeField] private float _speed = 10;
    [Space] [SerializeField] private Transform mintr;
    [SerializeField] private Transform maxtr;
    [SerializeField] private CanvasScaler scaler;
    [Space] [SerializeField] private Vector3 min = new Vector3(0, 0, 0);
    [SerializeField] private Vector3 max = new Vector3(0, 0, 0);
    [SerializeField] private float coeff;
    [SerializeField] private bool testBool;
    [SerializeField] private Button center;
    [Space] [SerializeField] private TMP_Dropdown _dropdown;
    [SerializeField] private TMP_InputField _inputSens;
    [SerializeField] private Button _inputOK;


    public bool isCalibrate = false;


    public int currentValue = 0;
    
    public string comPortName = "COM3";
    public int baudRate = 9600;
    public Vector3 additionalPose = new Vector3(0, 0, 0);

    public float sens = 4f;

    private Thread readThread;
    private volatile bool keepReading = false;

    public float midValX = 0f;
    public float midValY = 0f;


    public int x_stab = 0;
    public int y_stab = 0;
    public float sens_val = 140f;
    public float sens_step = 5f;
    public float senss = 5f;
    public float relativeX = 0;
    public float relativeY = 0;
    
    void Start()
    {

        sens_val = 140f;
       /* min.x = mintr.position.x * coeff;
        min.y = mintr.position.y * coeff;
        max.x = maxtr.position.x * coeff;
        max.y = maxtr.position.y * coeff;

        StartReading();*/
    }


    void StartReading()
    {
        readThread = new Thread(ReadAndProcessComPort);
        readThread.IsBackground = true;
        keepReading = true;
        readThread.Start();
    }

    void ReadAndProcessComPort()
    {
        using (var serialPort = new SerialPort(comPortName, baudRate))
        {
            List<byte> buffer = new List<byte>();
            byte[] packetStart = new byte[] { 0x80, 0x80 };

            try
            {
                serialPort.Open();
                while (keepReading)
                {
                    byte[] readByte = new byte[1];
                    int bytesRead = serialPort.Read(readByte, 0, 1);
                    if (bytesRead > 0)
                    {
                        buffer.AddRange(readByte);

                        if (buffer.Count >= packetStart.Length
                            && buffer.Skip(buffer.Count - packetStart.Length).SequenceEqual(packetStart))
                        {
                            ProcessData(buffer.Take(buffer.Count - packetStart.Length).ToArray());
                            buffer.Clear();
                            buffer.AddRange(packetStart);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Ошибка: {ex.Message}");

            }

            serialPort.Close();
        }
    }

    void ProcessData(byte[] data)
    {
        if (data.Any())
        {
            x_stab = BitConverter.ToInt16(data, 2);
            y_stab = BitConverter.ToInt16(data, 4);

            if (x_stab > 32767)
            {
                x_stab -= 65536;
            }

            if (y_stab > 32767)
            {
                y_stab -= 65536;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isCalibrate)
            return;

        relativeX = (x_stab + midValX) / (sens_val - (sens_step * senss));
        relativeY = (y_stab + midValY) / (sens_val - (sens_step * senss));

        additionalPose = new Vector3(relativeX, relativeY, 0);

        

        _pointer.localPosition = new Vector3(additionalPose.x, additionalPose.y, 1f);
    }

    private void OnDestroy()
    {
        keepReading = false;
        if (readThread != null && readThread.IsAlive)
        {
            readThread.Join();
        }
    }

    private void OnApplicationQuit()
    {
        keepReading = false;
        if (readThread != null && readThread.IsAlive)
        {
            readThread.Join();
        }
        else
        {
            readThread.Abort();
        }
    }
}
