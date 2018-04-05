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
    }
}
