using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MazeSpawner : BaseBoot
{
    [SerializeField] private GameObject _cellPrefab;
    [SerializeField] private GameObject _finisPrefab;
    [SerializeField] private GameObject _ball;
    [SerializeField] private Transform _zone;
    [SerializeField] private Camera _camera;
    [SerializeField] private InputVariant inputVariant;
    [SerializeField] private TextMeshProUGUI _textW;

   
    public Vector3 CellSize = new Vector3(1, 1, 0);
    public Vector3 BallPos = new Vector3(0, 0, 0);

    private Vector2 padding = new Vector2(2, 2);
    private bool isBallInstantiated = false;


    public async override UniTask Boot()
    {
        await base.Boot();
        StartSpawn();


    }

    public void StartSpawn()
    {
        MazeGeneratorNew generator = new MazeGeneratorNew();
        var maze = generator.GenerateMaze();
        int width = generator.Width;
        int height = generator.Height;
        _textW.text = $"{width} x {height}";

        var finishCell = generator.PlaceMazeExit(maze);

        float halfWidth = (width - 1f) / 2f;
        float halfHeight = (height - 1f) / 2f;

        // ✅ ГАРАНТИРОВАННО 3-5 ЛОВУШЕК!
        List<Cell> trapCandidates = new List<Cell>();
        Vector2Int startPos = new Vector2Int(0, 0);
        Vector2Int finishPos = new Vector2Int(finishCell.X, finishCell.Y);

        // Спавн клеток + сбор кандидатов
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float posX = (x - halfWidth) * CellSize.x;
                float posZ = (y - halfHeight) * CellSize.z;
                Vector3 pos = new Vector3(posX, 0f, posZ);

                Cell cell = Instantiate(_cellPrefab, pos, Quaternion.identity, _zone).GetComponent<Cell>();
                cell.SetMazePosition(x, y);

                cell.Floor.SetActive(maze[x, y].Floor);
                cell.WallLeft.SetActive(maze[x, y].WallLeft);
                cell.WallRight.SetActive(maze[x, y].WallRight);
                cell.WallFront.SetActive(maze[x, y].WallFront);
                cell.WallBottom.SetActive(maze[x, y].WallBottom);

                // ✅ ЛОВУШКИ ТОЛЬКО В "ХОРОШИХ" КЛЕТКАХ:
                Vector2Int cellPos = new Vector2Int(x, y);

                // Исключаем: start, finish, их соседей + клетки с Distance < 3 (ранние)
                if (cellPos != startPos && cellPos != finishPos &&
                    !IsNeighbor(cellPos, startPos, 1) &&
                    !IsNeighbor(cellPos, finishPos, 1) &&
                    maze[x, y].Distance >= 3) // ✅ ДАЛЬНИЕ клетки!
                {
                    trapCandidates.Add(cell);
                }
            }
        }

        _zone.transform.localPosition = Vector3.zero;

        // Шар в [0,0]
        if (!isBallInstantiated)
        {
            float startX = (0 - halfWidth) * CellSize.x;
            float startZ = (0 - halfHeight) * CellSize.z;
            Vector3 ballPos = new Vector3(startX, 0.5f, startZ);
            var tt = Instantiate(_ball, ballPos, Quaternion.identity);
            tt.GetComponent<BallController>().pos = ballPos;
            Debug.LogError($"{ballPos.x} {ballPos.y} {ballPos.z}");
            BallPos = new Vector3(ballPos.x, ballPos.y, ballPos.z);
            isBallInstantiated = true;
        }

        // Финиш
        float finishX = (finishCell.X - halfWidth) * CellSize.x;
        float finishZ = (finishCell.Y - halfHeight) * CellSize.z;
        var finishObj = Instantiate(_finisPrefab, new Vector3(finishX, 0, finishZ), Quaternion.identity, _zone);


        // Камера
        _camera.orthographicSize = Mathf.Max(width, height) * CellSize.x * 0.6f;
        _camera.transform.position = new Vector3(0, _camera.transform.position.y, 0);

        // ✅ ГАРАНТИРОВАННО 3-5 ЛОВУШЕК!
        PlaceGuaranteedTraps(trapCandidates, maze);
    }

    private void PlaceGuaranteedTraps(List<Cell> candidates, MazeGeneratorCell[,] maze)
    {
        Debug.Log($"🔥 Кандидатов для ловушек: {candidates.Count}");

        // ✅ ФИКСИРОВАННО 3-5 ЛОВУШЕК (НЕ %!)
        int numTraps = UnityEngine.Random.Range(3, 6); // 3,4,5
        int trapsToPlace = Mathf.Min(numTraps, candidates.Count);

        if (trapsToPlace == 0)
        {
            Debug.LogWarning("❌ Мало кандидатов! Увеличиваем размер maze или убираем фильтры.");
            return;
        }

        Debug.Log($"🎯 Ставим {trapsToPlace} ловушек из {numTraps} запланированных");

        // Перемешиваем для рандома
        ShuffleList(candidates);

        // Ставим ровно trapsToPlace ловушек
        for (int i = 0; i < trapsToPlace; i++)
        {
            Cell trapCell = candidates[i];

            // ✅ Активация
            trapCell.TrapTrigger?.SetActive(true);
            trapCell.TrapVisual?.SetActive(true);

            Debug.Log($"✅ Ловушка #{i + 1}/ {trapsToPlace} в [{trapCell.mazeX}, {trapCell.mazeY}] (Distance: {maze[trapCell.mazeX, trapCell.mazeY].Distance})");
        }
    }

    // ✅ ШЕЙКЕР для рандома (Fisher-Yates)
    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    // ✅ Расширенный IsNeighbor (расстояние)
    private bool IsNeighbor(Vector2Int a, Vector2Int b, int maxDist = 1)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        return (dx <= maxDist && dy <= maxDist && (dx + dy > 0));
    }


    private Vector2Int GetCellPosition(Cell cell, MazeGeneratorCell[,] maze)
    {
        // Предполагаем, что в MazeGeneratorCell есть X,Y, но Cell - GO. Нужно добавить в Cell public int X,Y; и сетать при спавне.
        // В спавне: cell.X = x; cell.Y = y;
        // Тогда return new Vector2Int(cell.X, cell.Y);
        // Пока заглушка - реализуй!
        return Vector2Int.zero; // FIX ME
    }
}
