using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Space]
    [SerializeField] private Vector3 min = new Vector3(0, 0, 0);
    [SerializeField] private Vector3 max = new Vector3(0, 0, 0);
    [Space]
    [SerializeField] private RectTransform graphArea;
    [SerializeField] private Material mat;
    [SerializeField] private Sprite pointSprite;
    [SerializeField] private Color pointColor = Color.blue;
    [SerializeField] private Color lineColor = Color.red;
    [SerializeField] private float pointSize = 10f;

    [SerializeField] private Vector2 graphMin = new Vector2(-110, -110);
    [SerializeField] private Vector2 graphMax = new Vector2(110, 110);

    [Space]
    [SerializeField] private Button back;
    [SerializeField] private Button start;
    [SerializeField] private TextMeshProUGUI text2;
    [SerializeField] private TextMeshProUGUI text3;
    [SerializeField] private TextMeshProUGUI text4;
    [SerializeField] private TextMeshProUGUI textI;
    [SerializeField] private TextMeshProUGUI textII;
    [SerializeField] private TextMeshProUGUI textIII;
    [SerializeField] private TextMeshProUGUI textIV;
    [SerializeField] private TimerRomberg timer;


   public void DrawGraph()
    {
        //foreach (Transform child in graphArea)
        //    Destroy(child.gameObject);

        GameObject[] pointObjects = new GameObject[dots.Count];
        
        pointObjects[0] = DrawPoint(DataToCanvasPosition(dots[0]), pointColor);
        pointObjects[dots.Count-1] = DrawPoint(DataToCanvasPosition(dots[dots.Count -1 ]), pointColor);
        pointObjects[0].GetComponent<Image>().color = Color.green;
        pointObjects[0].GetComponent<Image>().enabled = true;
        pointObjects[dots.Count - 1].GetComponent<Image>().color = Color.black;
        pointObjects[dots.Count - 1].GetComponent<Image>().enabled = true;

        for (int i = 0; i < dots.Count - 1; i++)
        {
            Vector2 start = DataToCanvasPosition(dots[i]);
            Vector2 end = DataToCanvasPosition(dots[i+1]);
            DrawLine(start, end);
        }
    }

    Vector2 DataToCanvasPosition(Dot dataPoint)
    {
        float xNormalized = (dataPoint.X - graphMin.x) / (graphMax.x - graphMin.x);
        float yNormalized = (dataPoint.Y - graphMin.y) / (graphMax.y - graphMin.y);

        float canvasWidth = graphArea.rect.width;
        float canvasHeight = graphArea.rect.height;

        float xPos = -canvasWidth / 2 + xNormalized * canvasWidth;
        float yPos = -canvasHeight / 2 + yNormalized * canvasHeight;

        return new Vector2(xPos, yPos);
    }


    Vector2 GetAnchoredPosition(RectTransform rt)
    {
        return rt.anchoredPosition;
    }

    public GameObject DrawPoint(Vector2 position, Color color)
    {
        GameObject pointGO = new GameObject("Point");
        pointGO.transform.SetParent(graphArea, false);

        RectTransform rect = pointGO.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector3(pointSize, pointSize, -1);
        rect.anchoredPosition = position;

        Image image = pointGO.AddComponent<Image>();
        image.sprite = pointSprite;
        image.color = color;
        image.preserveAspect = true;

        image.enabled = false;

        return pointGO;
    }

    void DrawLine(Vector2 start, Vector2 end)
    {
        LineRenderer line = new GameObject("Line").AddComponent<LineRenderer>();
        line.transform.SetParent(graphArea, false);
        line.material = mat;
        line.startColor = lineColor;
        line.endColor = lineColor;
        line.startWidth = 1f;
        line.endWidth = 1f;
        line.positionCount = 2;
        line.useWorldSpace = false;
        line.SetPositions(new Vector3[] { 
            new Vector3(start.x, start.y, -1), 
            new Vector3(end.x, end.y, -1) });

        line.alignment = LineAlignment.TransformZ;
        line.transform.SetAsLastSibling(); 

    }

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

    public float time = 0f;

    private GameManager _manager;
    List<Dot> dots = new List<Dot> ();
    List<Dot> dotsBuffer = new List<Dot> ();
    int avgCount = 6;
    private bool testBool = true;

    void Start()
    {
        sens_val = 125f;

        if (_manager == null)
            _manager = GameManager.Instance;

        comPortName = StaticParams.COM_PORT_NAME;
        midValX = StaticParams.MID_VAL_X;
        midValY = StaticParams.MID_VAL_Y;
        sens = (int)StaticParams.SENSITIVE;
        currentValue = (int)StaticParams.SENSITIVE - 1;

        start.onClick.AddListener(StartReading);
        back.onClick.AddListener(BackToMenu);
    }

    private void BackToMenu()
    {
        keepReading = false;
        if (readThread != null && readThread.IsAlive)
        {
            readThread.Join();
        }

        SceneManager.LoadScene(0);
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
        timer.StartTimer();
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
                while (keepReading && serialPort.IsOpen)
                {
                    byte[] readByte = new byte[1];
                    int bytesRead = serialPort.Read(readByte, 0, 1);
                    
                    if (bytesRead > 0)
                    {
                        buffer.AddRange(readByte);

                        if (buffer.Count >= packetStart.Length
                            && buffer.Skip(buffer.Count - packetStart.Length).SequenceEqual(packetStart))
                        {
                            //Debug.Log(buffer.Count - packetStart.Length);
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


            if (time <= 10 && testBool)
            {
                relativeX = (x_stab + midValX) / (sens_val - (sens_step * 1));
                relativeY = (y_stab + midValY) / (sens_val - (sens_step * 1));


                var newPosX = Mathf.Clamp(relativeX, graphMin.x, graphMax.x);
                var newPosY = Mathf.Clamp(relativeY, graphMin.y, graphMax.y);

                dotsBuffer.Add(new Dot(newPosX, newPosY));

                if (dotsBuffer.Count == avgCount)
                {
                    dots.Add(avgDots(dotsBuffer));
                    dotsBuffer.Clear();
                }
            }        
            
        }
    }

    private Dot centerMass1;
    public Vector2 centerMass()
    {
        double x = 0, y = 0;

        foreach (Dot dot in dots)
        {
            x += dot.X;
            y += dot.Y;
        }

        //показать на графике как точку
        var res = new Dot((int)(x / dots.Count), (int)(y / dots.Count));
        centerMass1 = res;
        return new Vector2(res.X, res.Y);
    }


    public /*List<double>*/ void RMS()
    {
        double rmsX = 0;
        double rmsY = 0;

        double bufX = 0f;
        double bufY = 0f;
        for (int i = 0; i < dots.Count; i++)
        {
            bufX += Math.Pow(dots[i].X - centerMass1.X, 2);
            bufY += Math.Pow(dots[i].Y - centerMass1.Y, 2);
        }

        rmsX = Math.Sqrt(bufX / dots.Count);
        rmsY = Math.Sqrt(bufY / dots.Count);

        //вывести отдельно по X и Y 
        //Размах колебания по гориз и по вертик
        text2.text += rmsX.ToString();
        text3.text += rmsY.ToString();
        //return new List<double>() { rmsX, rmsY };
    }

    public void trajectoryLength()
    {
        double length = 0;

        for (int i = 0; i < dots.Count - 1; i++)
        {
            length += Math.Sqrt(Math.Pow((dots[i + 1].X - dots[i].X), 2) 
                              + Math.Pow((dots[i + 1].Y - dots[i].Y), 2));
        }

        text4.text = length.ToString();
        //return length;
    }

    public void countPointsByQuadrants()
    {
        int I = 0;
        int II = 0;
        int III = 0;
        int IV = 0;

        foreach (Dot dot in dots)
        {
            float x = dot.X;
            float y = dot.Y;


            if (x > 0 && y > 0)
            {
                I++;
            }
            else if (x < 0 && y > 0)
            {
                II++;
            }
            else if (x < 0 && y < 0)
            {
                III++;
            }
            else 
            {
                IV++;            
            }

        }
        //Количество точек в квадрантах
        //I = 1
        //II = 2
        //III = 2
        textI.text += (I / dots.Count * 100).ToString();
        textII.text += (II / dots.Count * 100).ToString();
        textIII.text += (III / dots.Count * 100).ToString();
        textIV.text += (IV / dots.Count * 100).ToString();

    } 

    private Dot avgDots(List<Dot> dotsBuf) { 
        
        double x = 0, y = 0;

        foreach (Dot dot in dotsBuf)
        {
            x += dot.X;
            y += dot.Y;
        }

        return new Dot((int)(x / avgCount), (int)(y / avgCount));

    }
    
/*    private void FixedUpdate()
    {
        if (time < 10)
        {
            time += Time.deltaTime;
        }
        else if (time >= 10 && testBool)
        {
            testBool = false;
            foreach (var dot in dots)
            {
                Debug.LogError($"{dot.X} : {dot.Y}");
            }
            
            DrawGraph();
        }
    }*/

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
    }
}
