using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using UnityEngine;

public class StabService
{
    private Thread readThread;
    private volatile bool keepReading = false;

    public float zoneBorder = 50;

    public float midValX = 0f;
    public float midValY = 0f;

    public int x_stab = 0;
    public int y_stab = 0;

    public float sens_val;
    public float sens_step = 5f;

    public float relativeX = 0f;
    public float relativeY = 0f;


    void Initialize()
    {
        
        //transform.position = _positionList.RightUp.position;
        
    }

    public void StartReading()
    {
        sens_val = StaticParams.SENS_VAL;

        readThread = new Thread(ReadAndProcessComPort);
        readThread.IsBackground = true;
        keepReading = true;
        readThread.Start();
    }

    void ReadAndProcessComPort()
    {
        using (var serialPort = new SerialPort(StaticParams.COM_PORT_NAME, StaticParams.BAUND_RATE))
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
                Debug.LogError($"Îøèáêà: {ex.Message}");

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

    //private void FixedUpdate()
    //{
    //    relativeX = (x_stab + midValX) / (sens_val - (sens_step * 5f));
    //    relativeY = (y_stab + midValY) / (sens_val - (sens_step * 5f));

    //    if (!(Math.Abs(relativeX) > border && Math.Abs(relativeY) > border))
    //    {
    //        return;
    //    }

    //    if (relativeX > 0 && relativeY > 0)
    //    {
    //        _position = _positionList.RightUp.position;
    //    }

    //    else if (relativeX > 0 && relativeY < 0)
    //    {
    //        _position = _positionList.RightDown.position;
    //    }

    //    else if (relativeX < 0 && relativeY > 0)
    //    {
    //        _position = _positionList.LeftUp.position;
    //    }

    //    else
    //    {
    //        _position = _positionList.LeftDown.position;
    //    }

    //    transform.position = _position;
    //}

    private void OnDestroy()
    {
        StopReading();
    }

    private void OnApplicationQuit()
    {
        StopReading();

        Screen.fullScreenMode = FullScreenMode.Windowed;
        Screen.SetResolution(1024, 1024, false);
    }

    public void StopReading()
    {
        keepReading = false;
        readThread.Join();
    }

}
