public class ExperienceCalculator
{
    public int CalculateRequiredExp(int level)
    {
        if (level <= 0)
        {
            throw new System.ArgumentException("レベルは1以上である必要があります");
        }

        return level * level * 100;
    }
}