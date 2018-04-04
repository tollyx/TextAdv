using Microsoft.VisualStudio.TestTools.UnitTesting;
using HelloWorld;

namespace HelloWorld.Tests
{
    [TestClass]
    public class NodeTests
    {
        [TestMethod]
        public void NeigbourBothWaysTest()
        {
            MapNode node = new MapNode("A", "A");
            node.SetNeighbour(Direction.Up, new MapNode("B", "B"), true);
            Assert.AreEqual(node, node.GetNeighbour(Direction.Up).GetNeighbour(Direction.Down));
        }
    }
}
