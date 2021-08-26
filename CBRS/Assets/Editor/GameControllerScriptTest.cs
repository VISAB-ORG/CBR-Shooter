using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class GameControllerScriptTest {

    public void Setup()
    {

    }

	[Test]
	public void GameControllerScriptTestSimplePasses() {
		// Use the Assert class to test conditions.
	}

    [Test]
    public void GameControllerScript_endRound()
    {
    }

	// A UnityTest behaves like a coroutine in PlayMode
	// and allows you to yield null to skip a frame in EditMode
	[UnityTest]
	public IEnumerator GameControllerScriptTestWithEnumeratorPasses() {
		// Use the Assert class to test conditions.
		// yield to skip a frame
		yield return null;
	}
}
