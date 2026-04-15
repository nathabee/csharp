namespace DialMock.Core.Geometry;

public readonly record struct Text2(
    Point2 Position,
    string Content,
    double RotationDeg = 0);