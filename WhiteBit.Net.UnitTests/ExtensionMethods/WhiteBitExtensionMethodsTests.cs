using CryptoExchange.Net.SharedApis;
using NUnit.Framework;
using WhiteBit.Net.Enums;
using WhiteBit.Net.ExtensionMethods;

namespace WhiteBit.Net.UnitTests.ExtensionMethods;

[TestFixture()]
public class WhiteBitExtensionMethodsTests
{
    [Test]
    public void ToPositionSide()
    {
        var sharedPositionSide = SharedPositionSide.Long;
        var actualPositionSide = sharedPositionSide.ToPositionSide();
        Assert.That(actualPositionSide, Is.EqualTo(PositionSide.Long));

        sharedPositionSide = SharedPositionSide.Short;
        actualPositionSide = sharedPositionSide.ToPositionSide();
        Assert.That(actualPositionSide, Is.EqualTo(PositionSide.Short));
    }
}
