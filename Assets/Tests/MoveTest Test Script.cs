using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MoveTestTestScript
{
    // A Test behaves as an ordinary method
    [Test]
    public void MoveTestTestScriptSimplePasses()
    {
        // Use the Assert class to test conditions
        Assert.AreEqual(new Vector3(0, 0, 1), MoveTest.DirectionVector(1));
        Assert.AreEqual(new Vector3(1, 0, 1), MoveTest.DirectionVector(2));
        Assert.AreEqual(new Vector3(1, 0, 0), MoveTest.DirectionVector(3));
        Assert.AreEqual(new Vector3(1, 0, -1), MoveTest.DirectionVector(4));
        Assert.AreEqual(new Vector3(0, 0, -1), MoveTest.DirectionVector(5));
        Assert.AreEqual(new Vector3(-1, 0, -1), MoveTest.DirectionVector(6));
        Assert.AreEqual(new Vector3(-1, 0, 0), MoveTest.DirectionVector(7));
        Assert.AreEqual(new Vector3(-1, 0, 1), MoveTest.DirectionVector(8));
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator MoveTestTestScriptWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
