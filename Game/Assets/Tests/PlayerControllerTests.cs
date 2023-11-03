using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerControllerTests
{
    private GameObject playerObject;
    private PlayerController playerController;
    private Rigidbody2D rb;
    private Animator playerAnim;

    [SetUp]
    public void SetUp()
    {
        playerObject = new GameObject();
        playerObject.AddComponent<PlayerController>();
        playerObject.AddComponent<Rigidbody2D>();
        playerObject.AddComponent<Animator>();
        playerController = playerObject.GetComponent<PlayerController>();
        rb = playerObject.GetComponent<Rigidbody2D>();
        playerAnim = playerObject.GetComponent<Animator>();
    }

    // A Test behaves as an ordinary method
    [Test]
    public void PlayerTestsSimplePasses()
    {


        // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator PlayerITimeTest()
    {
        for (int i = 0; i < 10; i++)
        {
            playerController.TakeDamage(10);
            yield return new WaitForSeconds(0.1f);
        }

        // Use the Assert class to test conditions.
        // Use yield to skip a frame.

        Assert.AreEqual(playerController.curHP, 80);
        yield return null;
    }

    [UnityTest]
    public IEnumerator PlayerDeathTest()
    {
        for (int i = 0; i < 5; i++)
        {
            playerController.TakeDamage(20);
            yield return new WaitForSeconds(0.6f);
        }

        Assert.AreEqual(true, playerController.isDead());
        yield return null;
    }

    [UnityTest]
    public IEnumerator Move_SetsScaleBasedOnMoveX()
    {
        // Arrange
        float moveX = 1;

        // Act
        playerController.moveX = moveX;
        playerController.move();

        // Assert
        int expectedScaleX = 1;
        Assert.AreEqual(expectedScaleX, (int)playerController.transform.localScale.x);
        yield return null;
    }
}