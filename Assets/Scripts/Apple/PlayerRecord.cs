using System.Drawing.Printing;

public class PlayerRecord
{
    public string _date;
    public string _timeValue;
    public int _scoreG;
    public int _scoreL;
    public bool _isTimeMode;
    public int _type;
    public string _mazeType;
    public string _fit;
    public string _level;

    public PlayerRecord() { }

    public PlayerRecord(string date, bool isTimeMode, int scoreG, int scoreL, string timeValue, string level)
    {
        _date = date;
        _isTimeMode = isTimeMode;
        _scoreG = scoreG;
        _scoreL = scoreL;
        _timeValue = timeValue;
        _type = 0;
        _level = level;
    }

    public PlayerRecord(string date, int scoreG, int scoreL, string timeValue)
    {
        _date = date;
        _scoreG = scoreG;
        _scoreL = scoreL;
        _timeValue = timeValue;
        _type = 2;
    }

    public PlayerRecord(string date, string timeValue, string type, string fit)
    {
        _date = date;
        _timeValue = timeValue;
        _type = 1;
        _mazeType = type;
        _fit = fit;
    }
}
