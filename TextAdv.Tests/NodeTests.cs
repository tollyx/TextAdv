using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextAdv;

namespace TextAdv.Tests {
    [TestClass]
    public class NodeTests {
        [TestMethod]
        public void NeigbourBothWaysTest() {
            MapNode node = new MapNode("A", "A");
            node.SetNeighbour(Direction.Up, new MapNode("B", "B"), true);
            Assert.AreEqual(node, node.GetNeighbour(Direction.Up).GetNeighbour(Direction.Down));
        }

        [TestMethod]
        public void StringExtensionTest() {
            string str = "TestString";
            Assert.IsFalse(str.IsEmptyOrWhiteSpace());
            str = null;
            Assert.IsTrue(str.IsEmptyOrWhiteSpace());
            str = "       ";
            Assert.IsTrue(str.IsEmptyOrWhiteSpace());
        }
    }

    public static class Extensions {
        public static bool IsEmptyOrWhiteSpace(this string val) {
            return string.IsNullOrWhiteSpace(val);
        }
    }
}
