using UniRx;

public class BaseGameStateModel
{
    public readonly IntReactiveProperty RightCount = new(0);
    public readonly IntReactiveProperty MissCount = new(0);
    public readonly IntReactiveProperty Time = new(0);
    public readonly IntReactiveProperty Level = new(0);

    public void Reset()
    {
        RightCount.Value = 0;
        MissCount.Value = 0;
    }

    public void ResetTime()
    {
        Time.Value = 0;
    }

    public void ResetLevel()
    {
        Level.Value = 0;
    }
}
