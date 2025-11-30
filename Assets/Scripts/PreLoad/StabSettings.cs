using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StabSettings : BaseBoot
{

    [Space]
    [SerializeField] private TextMeshProUGUI selectedComPort;
    [SerializeField] private Button findComPortButton;
    [SerializeField] private Button exit;


    [Space]
    [SerializeField] private Button calibrationButton;
    [SerializeField] private TextMeshProUGUI inputX;
    [SerializeField] private TextMeshProUGUI inputY;

    [Space]
    [SerializeField] private Button startApple;
    [SerializeField] private Button startMaze;
    [SerializeField] private Button startWolf;
    [SerializeField] private Button rombergTest;
    [SerializeField] private Button _openResultPanel;
    [SerializeField] private Button _backToMenuFromResult;
    [SerializeField] private TextMeshProUGUI errorString;
    [SerializeField] private TextMeshProUGUI errorString2;

    [Space]
    [SerializeField] private Button _sensUp;
    [SerializeField] private Button _sensDown;
    [SerializeField] private TMP_InputField _sensInput;
    
    [Space]
    [SerializeField] private GameObject _resultPanel;
    [SerializeField] private LoadRecordsData _loadRecordsData;
    [SerializeField] private GameObject _canvas;

    [Space]
    [SerializeField] private GameObject _currentPanel;
    [SerializeField] private GameObject _settingsPanel;
    [SerializeField] private Button _backToMain;
    [SerializeField] private Button _settings;

    public int defaultBaudRate = 9600;

    public float midValX = 0;
    public float midValY = 0;

    private int x_stab = 0;
    private int y_stab = 0;

    private Thread readThread;
    private volatile bool keepReading = false;

    public static float fixedTime = 1.0f;
    private float timeLeft = 0.0f;

    private GameManager _manager;
    
    public bool isComPortFind = false;

    public int currentValue = 0;
    private List<string> sensData = Enumerable.Range(1, 99)
        .Select(n => n.ToString())
        .ToList();

    public async override UniTask Boot()
    {
        await base.Boot();
        StartSettings();
    }

    void StartSettings()
    {
        if (_manager == null)
            _manager = GameManager.Instance;


       


        findComPortButton.onClick.AddListener(FindComPort);
        calibrationButton.onClick.AddListener(StabCalibration);
        startApple.onClick.AddListener(() => LoadGameScene(1));
        startWolf.onClick.AddListener(() => LoadGameScene(3));
        startMaze.onClick.AddListener(() => LoadGameScene(2));
        rombergTest.onClick.AddListener(() => LoadGameScene(4));
        exit.onClick.AddListener(() => OnApplicationQuit());

        _backToMenuFromResult.onClick.AddListener(BackToMenuUI);
        _openResultPanel.onClick.AddListener(OpenResult);
        _settings.onClick.AddListener(() =>
        {
            _settingsPanel.SetActive(true);
            _currentPanel.SetActive(false);
        });
        _backToMain.onClick.AddListener(()=>
        {
            _settingsPanel.SetActive(false);
            _currentPanel.SetActive(true);
        });
 

        var value = _manager.Data.sensitive;

        StaticParams.SENSITIVE = value;

        _sensInput.text = StaticParams.SENSITIVE.ToString();


        _sensUp.onClick.AddListener(() => SetSensitiveButton(0));
        _sensDown.onClick.AddListener(() => SetSensitiveButton(1));
        _sensInput.onEndEdit.AddListener(delegate 
        { 
            SetSensitiveButton(2);
        });


        readThread = new Thread(ReadAndProcessComPort);
        readThread.IsBackground = true;

        FindComPort();


        

       
    }

    private void OpenResult()
    {
        Debug.Log("123123");
        _canvas.SetActive(false);
        _loadRecordsData.UpdateList();
        _resultPanel.SetActive(true);
    }

    private void BackToMenuUI()
    {
        _resultPanel.SetActive(false);
        _canvas.SetActive(true);
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

        StaticParams.SENSITIVE = Convert.ToInt32(sensData[currentValue]);
        _manager.Data.sensitive = Convert.ToInt32(sensData[currentValue]);
   
        _sensInput.text = sensData[currentValue];

    }

    private async void LoadGameScene(int scene)
    {
        keepReading = false;
        isComPortFind = false;

        if (readThread != null && readThread.IsAlive)
        {
            readThread.Join();
        }


        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        Screen.SetResolution(1920, 1080, true);

        Debug.LogError($"Sens {StaticParams.SENSITIVE}");
        await GameManager.Instance.SaveGameDataToJson();
        SceneManager.LoadScene(scene);
    }
    
    private void FindComPort()
    {
        var port = ScanAndComparePorts();
        
        if (isComPortFind && port != null)
        {
            selectedComPort.text = StaticParams.COM_PORT_NAME;
            return;
        }

        if (port != null)
        {
            errorString.gameObject.SetActive(false);
            errorString2.gameObject.SetActive(false);
            StaticParams.COM_PORT_NAME = port;
            selectedComPort.text = port;

            isComPortFind = true;

            keepReading = true;
            readThread.Start();
        }
        else
        {
            selectedComPort.text = "";
            errorString.gameObject.SetActive(true);
            errorString2.gameObject.SetActive(true);
            StaticParams.COM_PORT_NAME = "";
        }
    }

    private void StabCalibration()
    {
        midValX = -x_stab;
        midValY = -y_stab;
        StaticParams.MID_VAL_X = midValX;
        StaticParams.MID_VAL_Y = midValY;
        Debug.LogError(-x_stab * StaticParams.SENSITIVE);
        Debug.LogError(-y_stab * StaticParams.SENSITIVE);
    }
    
    private void OnApplicationQuit()
    {
        keepReading = false;
        if (readThread != null && readThread.IsAlive)
        {
            readThread.Join();
        }

        Application.Quit();
    }

    private bool isOurComPort(string comPortName)
    {
        bool result = false;
        List<byte> buffer = new List<byte>();
        byte[] packetStart = new byte[] { 0x80, 0x80 };

        try
        {
            using (SerialPort serialPort = new SerialPort(comPortName, defaultBaudRate))
            {
                serialPort.Open();
                serialPort.ReadTimeout = 200;
                for (int i = 0; i < 3; i++)
                {
                    byte[] readByte = new byte[1];
                    int bytesRead = serialPort.Read(readByte, 0, 1);
                    if (bytesRead > 0)
                    {
                        buffer.AddRange(readByte);

                        if (buffer.Count >= packetStart.Length && 
                            buffer.Skip(buffer.Count - packetStart.Length).SequenceEqual(packetStart))
                        {
                            result = true;
                            buffer.Clear();
                            buffer.AddRange(packetStart);
                            break;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"error {comPortName}: {ex.Message}");

        }

        return result;
    }

    private string ScanAndComparePorts()
    {
        string[] ports = SerialPort.GetPortNames();

        foreach (string port in ports)
        {
            if (isOurComPort(port))
            {
                Debug.Log($"Our Port {port}");
                return port;
            }
        }

        Debug.Log($"Port not found");
        isComPortFind = false;
        return null;
    }


    void ReadAndProcessComPort()
    {
        using (SerialPort serialPort = new SerialPort(StaticParams.COM_PORT_NAME, defaultBaudRate))
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

                        if (buffer.Count >= packetStart.Length && buffer.Skip(buffer.Count - packetStart.Length).SequenceEqual(packetStart))
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
                Debug.LogError($"error: {ex.Message}");
            }
            finally
            {
                serialPort.Close();
            }
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
        timeLeft -= Time.fixedDeltaTime;

        if (timeLeft < 0)
        {
            //Debug.Log(StaticParams.SENSITIVE);
            inputX.text = $"X = {x_stab + midValX}";
            inputY.text = $"Y = {y_stab + midValY}";
            timeLeft = fixedTime;
        }
    }
}
