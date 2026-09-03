namespace Stubs
{
    public class SaveLoadManager
{
    public static GlobalDataClass GlobalData = new GlobalDataClass();
    public static UnsavedDataClass UnsavedData = new UnsavedDataClass();
}

  public class UnsavedDataClass
    {
        public int KissCount;
        public int FellaSceneCount;
        public int Days;
        public GlobalFlagsClass GlobalFlags = new GlobalFlagsClass();   // ← 追加
        public bool SubEventShown;                                      // ← 追加
    }
 public class GlobalFlagsClass
    {
        public bool SkippedToDay6;
        public void SetFlag(FlagEnum flag, bool isOn) { }
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

