using System;

using Backend;
using NUnit.Framework;
using Gdk;

namespace PrototypeTests
{
    [TestFixture()]
    public class APinTests
    {
        [Test()]
        public void APinConstructorTests()
        {
            var pin = new APin();

            Assert.AreEqual(Backend.PinType.ANALOG, pin.Type);
            Assert.AreEqual(Backend.PinMode.INPUT, pin.Mode);

            pin.PlotColor = new Color(255, 0, 0);
            Assert.AreEqual(new Color(255, 0, 0), pin.PlotColor);
        }

        [Test()]
        public void APinCopyConstructorTest()
        {
            var pin = new APin();
            pin.Name = "PinPinsen";
            pin.Interval = 3000;

            double value = 42;
            pin.Value = new DateTimeValue(value, DateTime.Now);

            var copypin = new APin(pin);
            Assert.AreEqual("PinPinsen", copypin.Name);
            Assert.AreEqual(3000, copypin.Interval);
            Assert.AreEqual(value, copypin.Value.Value);
            Assert.AreEqual(1, copypin.Values.Count);
            Assert.AreEqual(pin.RAWValues, copypin.RAWValues);
        }

        [Test()]
        public void APinNewValueEventTest()
        {
            double val = -1;
            var pin = new APin();
            pin.OnNewValue += (o, args) => val = args.Value;

            pin.Value = new DateTimeValue(42.0, DateTime.Now);
            Assert.AreEqual(42, val);

            pin.Value = new DateTimeValue(43, DateTime.Now);
            Assert.AreEqual(43, val);
        }

        [Test()]
        public void APinNewValueEventTestAfterEdit()
        {
            double val = -1;
            var pin = new APin();
            pin.OnNewValue += (o, args) => val = args.Value;

            pin.Value = new DateTimeValue(42, DateTime.Now);

            Assert.AreEqual(42, val);

            pin.Interval = 3000;
            pin.OnNewValue = null;
            pin.OnNewValue += (o, args) => val = args.Value * 2;
            pin.Value = new DateTimeValue(42, DateTime.Now);

            Assert.AreEqual(84, val);
        }

        [Test()]
        public void APinEqualsTest()
        {
            var pin1 = new APin();
            var pin2 = new APin();

            Assert.AreEqual(true, pin1.Equals(pin2));

            pin1.PlotColor = new Color(0, 255, 0);
            pin2.PlotColor = new Color(0, 255, 255);

            Assert.AreEqual(false, pin1.Equals(pin2));
        }

        [Test()]
        public void APinValuesTest()
        {
            var pin1 = new APin();
            pin1.Value = new DateTimeValue(5, DateTime.Now);

            Assert.AreEqual(5, pin1.Value.Value);

            pin1.Values.Clear();
            Assert.AreEqual(double.NaN, pin1.Value.Value);
        }

        [Test]
        public void APinvaluesTest2()
        {
            var pin1 = new APin();
            pin1.Offset = 2;
            pin1.Value = new DateTimeValue(5, DateTime.Now);

            Assert.AreEqual(7, pin1.Value.Value);

            pin1.Offset = 0;
            pin1.Slope = .5;
            pin1.Value = new DateTimeValue(5, DateTime.Now);
            Assert.AreEqual(2.5, pin1.Value.Value);
        }

        [Test]
        public void APinvaluesTest3()
        {
            var pin1 = new APin();
            pin1.MeanValuesCount = 2;
            pin1.Value = new DateTimeValue(4, DateTime.Now);
            pin1.Value = new DateTimeValue(2, DateTime.Now);
            Assert.AreEqual(3, pin1.Value.Value);
        }

        [Test]
        public void APinvaluesTest4()
        {
            var pin1 = new APin();
            pin1.MeanValuesCount = 2;
            pin1.Slope = .5;
            pin1.Value = new DateTimeValue(4, DateTime.Now);
            pin1.Value = new DateTimeValue(2, DateTime.Now);
            Assert.AreEqual(1.5, pin1.Value.Value);
        }

        [Test]
        public void APinvaluesTest5()
        {
            var pin1 = new APin();
            pin1.MeanValuesCount = 3;
            pin1.Slope = .5;
            pin1.Value = new DateTimeValue(4, DateTime.Now);
            pin1.Value = new DateTimeValue(2, DateTime.Now);
            Assert.AreEqual(double.NaN, pin1.Value.Value);
            pin1.Value = new DateTimeValue(2, DateTime.Now);
            Assert.AreEqual(4.0 / 3.0, pin1.Value.Value);

        }

        [Test]
        public void RAWToVoltTest()
        {
            var b = new Board();

            Assert.AreEqual(5, b.AnalogReferenceVoltage);
            Assert.AreEqual("DEFAULT", b.AnalogReferenceVoltageType);

            Assert.AreEqual(5, b.RAWToVolt(1023));
            Assert.AreEqual(2.5, b.RAWToVolt(511.5));

            b.AnalogReferenceVoltage = 3;
            Assert.AreEqual(1.5, b.RAWToVolt(511.5));
        }
    }
}

