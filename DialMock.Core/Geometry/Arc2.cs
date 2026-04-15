namespace DialMock.Core.Geometry;

public readonly record struct Arc2(
    Point2 Center,
    double Radius,
    double StartAngleDeg,
    double SweepAngleDeg);