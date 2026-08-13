namespace Stubs
{
    public class SaveLoadManager
{
    public static GlobalDataClass GlobalData = new GlobalDataClass();
}

public class GlobalDataClass
{
    public GameOptionClass GameOption = new GameOptionClass();

    public float GetMouseSensitivityFactor()
    {
        return 1f;
    }
}

public class GameOptionClass
{
    public int MouseButtonDecision = 0;
    public int MouseButtonAuto = 1;
    public int MouseButtonSpecial = 2;
}
}

