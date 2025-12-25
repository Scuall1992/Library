using Cysharp.Threading.Tasks;
using DG.Tweening;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.VisualScripting.Member;

public class BallController : BaseBoot
{
    [SerializeField] private float moveForce = 1.5f;
    [SerializeField] private float maxSpeed = 3f;

    [Space]
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

    private MazeSpawner ms;

    private Rigidbody rb;
    private Transform rbtr;

    public Vector3 pos = new Vector3();

    public float sens_step = 5f;
    public float senss = 5f;
    public float relativeX = 0;
    public float relativeY = 0;

    public bool isBall = true;

    public async override UniTask Boot()
    {
        await base.Boot();

        comPortName = StaticParams.COM_PORT_NAME;


            rb = GameObject.FindWithTag("ball").GetComponent<Rigidbody>();
            ms = GameObject.FindWithTag("spawner").GetComponent<MazeSpawner>();
            Debug.LogError(ms.name);
            if (rb != null)
            {
                // Do something with the playerObject
                Debug.Log("Found player: " + rb.name);
            }
            else
            {
                Debug.LogWarning("Player object with tag 'Player' not found.");
            }

        sens_val = 140f;
        readThread = new Thread(ReadAndProcessComPort);
        readThread.IsBackground = true;
        keepReading = true;
        readThread.Start();
    }

    private void OnDisable()
    {
        StopReading();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Trap"))
        {
            Debug.Log("123123");

            var rbtr1 = GameObject.FindWithTag("ball").GetComponent<Transform>();
            Debug.LogError($"{pos.x} {pos.y} {pos.z}");
            rbtr1.position = pos;
        }
    }

    private void Respawn()
    {
        // Телепорт на старт

        // Сброс физики
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;


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
       
            BallMovement();
      
    }


    private void BallMovement()
    {
        relativeX = (x_stab + midValX) / (sens_val - (sens_step * senss));
        relativeY = (y_stab + midValY) / (sens_val - (sens_step * senss));

        Vector3 movement = new Vector3(relativeX / 10, 0, relativeY / 10);

        if (rb != null)
        {
            if (rb.linearVelocity.magnitude < maxSpeed)
            {
                rb.AddForce(movement * moveForce);
            }
            else
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }
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
        else
        {
            readThread.Abort();
        }

        Debug.LogError("STOP READING");
    }

}


