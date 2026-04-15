namespace DialMock.Core.Models;

public class DialLayoutData
{
    public double NeedleAngleDeg { get; set; }

    public List<TickMark> MajorTicks { get; set; } = new();

    public class TickMark
    {
        public double Value { get; set; }
        public double AngleDeg { get; set; }
    }
}