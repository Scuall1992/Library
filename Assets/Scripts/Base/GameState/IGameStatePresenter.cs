public interface IGameStatePresenter 
{
    void AddRightValue();    
    void AddMissValue();   
    int GetRightValue();
    int GetMissValue();
    string GetTimeValue();
}
