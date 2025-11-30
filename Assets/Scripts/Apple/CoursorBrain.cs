using System;
using System.IO.Ports;
using System.Threading;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

public class CoursorBrain : BaseBoot
{
    [SerializeField] private Transform _pointer;
    [SerializeField] private float _speed = 10;
    [Space]
    [SerializeField] private Transform mintr;
    [SerializeField] private Transform maxtr;
    [SerializeField] private CanvasScaler scaler;
    [Space]
    [SerializeField] private Vector3 min = new Vector3(0, 0, 0);
    [SerializeField] private Vector3 max = new Vector3(0, 0, 0);
    [SerializeField] private float coeff;
    [SerializeField] private bool testBool;
    [SerializeField] private Button center;
    [Space]
    [SerializeField] private TMP_Dropdown _dropdown;
    [SerializeField] private TMP_InputField _inputSens;
    [SerializeField] private Button _inputOK;

    [Space]
    [SerializeField] private Button _sensUp;
    [SerializeField] private Button _sensDown;
    [SerializeField] private TMP_InputField _sensInput;

    [Space]
    [SerializeField] private Timer _timer;

    public bool isCalibrate = false;


    public int currentValue = 0;
    
    private List<string> sensData = Enumerable.Range(1, 99)
        .Select(n => n.ToString())
        .ToList();


    public string comPortName;
    public int baudRate = 9600;
    public Vector3 additionalPose = new Vector3(0, 0, 0);

    public int sens = 1;

    private Thread readThread;
    private volatile bool keepReading = false;

    public float midValX = 0f;
    public float midValY = 0f;


    public int x_stab = 0;
    public int y_stab = 0;
    public float sens_val = 0f;
    public float sens_step = 5f;
    public float relativeX = 0;
    public float relativeY = 0;

    private GameManager _manager;

    public async override UniTask Boot()
    {
        await base.Boot();
        StartBrain();
    }

    void StartBrain()
    {
        sens_val = 125f;

        if (_manager == null)
            _manager = GameManager.Instance;
        
        comPortName = StaticParams.COM_PORT_NAME;
        midValX = StaticParams.MID_VAL_X;
        midValY = StaticParams.MID_VAL_Y;
        sens = (int)StaticParams.SENSITIVE;
        currentValue = (int)StaticParams.SENSITIVE - 1;

        min.x = mintr.position.x * coeff ;
        min.y = mintr.position.y * coeff ;
        max.x = maxtr.position.x * coeff ;
        max.y = maxtr.position.y * coeff ;

        center.onClick.AddListener(CenterCoursor);

        _sensInput.text = _manager.Data.sensitive.ToString();
        _sensUp.onClick.AddListener(() => SetSensitiveButton(0));
        _sensDown.onClick.AddListener(() => SetSensitiveButton(1));
        _sensInput.onEndEdit.AddListener(delegate
        {
            SetSensitiveButton(2);
        });
        
        StartReading();
    }

    private void SetSensitiveButton(int operation)
    {
        if (operation == 0)
        {
            if(currentValue < sensData.Count - 1)
            {
                currentValue++;
            }
        }
        else if (operation == 1)
        {
            if (currentValue > 0)
            {
                currentValue--;
            }
        }
        else if (operation == 2)
        {
            var sensValue = 0;
            Int32.TryParse(_sensInput.text, out sensValue);
            
            if (sensValue >= 1 && sensValue <= 99)
            {
                currentValue = sensValue-1;
            }
            else
            {
                _sensInput.text = $"{currentValue + 1}";

                Debug.LogError($"New Sens: {sensData[currentValue]}");
                return;
            }
        }

        Debug.LogError($"New Sens: {sensData[currentValue]}");
        StaticParams.SENSITIVE = Convert.ToInt32(sensData[currentValue]);
        _manager.Data.sensitive = Convert.ToInt32(sensData[currentValue]) /** (float)Math.Pow(2, GameManager.Instance.Data.offset)*/;
      
        _sensInput.text = sensData[currentValue];
    }
    
    private void CenterCoursor()
    {
        isCalibrate = true;
        _timer.CenterTimer();
    }

    public void FinalCenter()
    {
        midValX = -x_stab;
        midValY = -y_stab;
        StaticParams.MID_VAL_X = midValX;
        StaticParams.MID_VAL_Y = midValY;
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
        using (SerialPort serialPort = new SerialPort(comPortName, baudRate))
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

            //x_stab *= (int)StaticParams.SENSITIVE;
            //y_stab *= (int)StaticParams.SENSITIVE;
        }
    }

    private void FixedUpdate()
    {
        if (isCalibrate)
            return; 

        relativeX = (x_stab + midValX) / (sens_val - (sens_step * (int)StaticParams.SENSITIVE));
        relativeY = (y_stab + midValY) / (sens_val - (sens_step * (int)StaticParams.SENSITIVE));

        additionalPose = new Vector3(relativeX * max.x / 100, relativeY * max.y / 100, 0);

        var newPosX = Mathf.Clamp(additionalPose.x, min.x, max.x);
        var newPosY = Mathf.Clamp(additionalPose.y, min.y, max.y);
        
        _pointer.localPosition = new Vector3(newPosX, newPosY, 1f);
    }

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
