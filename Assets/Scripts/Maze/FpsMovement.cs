using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Windows;

public class FpsMovement : MonoBehaviour
{
    [SerializeField] private float maxTiltAngle = 30f;
    [SerializeField] private float tiltSpeed = 5f;   

    private Quaternion targetRotation;
    float targetPitch;
    float targetRoll;

    public int currentValue = 0;

    public string comPortName;
    public int baudRate = 9600;
    public Vector3 additionalPose = new Vector3(0, 0, 0);

    public float sens = 4f;

    private Thread readThread;
    private volatile bool keepReading = false;

    public float midValX = 0f;
    public float midValY = 0f;


    public int x_stab = 0;
    public int y_stab = 0;
    public float sens_val = 327.67f;


    void Start()
    {
        comPortName = StaticParams.COM_PORT_NAME;
        targetRotation = transform.rotation;
        
    }

    private void OnDisable()
    {
        StopReading();
    }

    private void OnEnable()
    {
        if (keepReading || (readThread != null && readThread.IsAlive))
            return;

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
        var relativeX = (x_stab + midValX) / (sens_val / (float)Math.Pow(2, sens - 1)) / 100;
        var relativeY = (y_stab + midValY) / (sens_val / (float)Math.Pow(2, sens - 1)) / 100;

        targetPitch = Mathf.Clamp(relativeY * maxTiltAngle, -maxTiltAngle, maxTiltAngle);
        targetRoll = Mathf.Clamp(-relativeX * maxTiltAngle, -maxTiltAngle, maxTiltAngle);

        targetRotation = Quaternion.Euler(targetPitch, 0, targetRoll);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * tiltSpeed);
    }

    private void OnDestroy()
    {
        StopReading();
    }

    private void OnApplicationQuit()
    {
        StopReading();

    }

    public void StopReading()
    {
        keepReading = false;
        if (readThread != null && readThread.IsAlive)
        {
            readThread.Join();
        }

        Debug.LogError("STOP READING");
    }

}
