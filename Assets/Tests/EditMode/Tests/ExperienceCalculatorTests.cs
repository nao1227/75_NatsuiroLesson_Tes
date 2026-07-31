using NUnit.Framework;

public class ExperienceCalculatorTests
{
    [Test]
    public void CalculateRequiredExp_ReturnsCorrectValue_WhenLevelIsOne()
    {
        var calculator = new ExperienceCalculator();
        int result = calculator.CalculateRequiredExp(1);
        Assert.AreEqual(100, result);
    }

    [Test]
    public void CalculateRequiredExp_ReturnsCorrectValue_WhenLevelIsTen()
    {
        var calculator = new ExperienceCalculator();
        int result = calculator.CalculateRequiredExp(10);
        Assert.AreEqual(10000, result);
    }

    [TestCase(1, 100)]
    [TestCase(2, 400)]
    [TestCase(5, 2500)]
    [TestCase(10, 10000)]
    public void CalculateRequiredExp_ReturnsExpectedValue_ForVariousLevels(int level, int expected)
    {
        var calculator = new ExperienceCalculator();
        int result = calculator.CalculateRequiredExp(level);
        Assert.AreEqual(expected, result);
    }

    [Test]
    public void CalculateRequiredExp_ThrowsArgumentException_WhenLevelIsZero()
    {
        var calculator = new ExperienceCalculator();

        Assert.Throws<System.ArgumentException>(() =>
        {
            calculator.CalculateRequiredExp(0);
        });
    }

    [Test]
    public void CalculateRequiredExp_ThrowsArgumentException_WhenLevelIsNegative()
    {
        var calculator = new ExperienceCalculator();

        Assert.Throws<System.ArgumentException>(() =>
        {
            calculator.CalculateRequiredExp(-5);
        });
    }
}