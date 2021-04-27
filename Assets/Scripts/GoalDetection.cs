using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalDetection : MonoBehaviour
{
    [Header("Goals")]
    [SerializeField] GameObject blueGoal = null;
    [SerializeField] GameObject orangeGoal = null;

    [Header("Prefabs")]
    [SerializeField] GameObject ball = null;
    [SerializeField] GameObject goalExplosionParticles = null;
    [SerializeField] GameObject bluePlayer = null;
    [SerializeField] GameObject orangePlayer = null;

    [Header("Spawn Points")]
    [SerializeField] Transform ballRespawn = null;
    [SerializeField] Transform orangeRespawn = null;
    [SerializeField] Transform blueRespawn = null;

    [Header("Misc")]
    [SerializeField] float resetWaitTime = 3f;
    [SerializeField] float explosionRadius = 6f;
    [SerializeField] float explosionForce = 200f;
    [SerializeField] float explosionOffset = 3f;

    GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
            GoalScored();
            Debug.Log("goal scored");
        }
    }

    private void GoalScored()
    {
        GameObject goalScoredOn = this.gameObject;
        if (goalScoredOn == blueGoal)
        {
            gameManager.OrangeScored();
        }
        if (goalScoredOn == orangeGoal)
        {
            gameManager.BlueScored();
        }
        GoalExplosion();
        StartCoroutine(Reset());
    }

    private void GoalExplosion()
    {
        GameObject explosionParticles = Instantiate(goalExplosionParticles, ball.transform.position, Quaternion.identity);
        Destroy(explosionParticles, 2f);
        ball.SetActive(false);
        GoalKnockback(explosionParticles.transform);
    }

    private void GoalKnockback(Transform goalPosition)
    {
        GameObject bluePlayer = GameObject.FindGameObjectWithTag("Blue Player");
        GameObject orangePlayer = GameObject.FindGameObjectWithTag("Orange Player");
        ApplyKnockback(bluePlayer, goalPosition);
        ApplyKnockback(orangePlayer, goalPosition);
    }

    private void ApplyKnockback(GameObject player, Transform goalPosition)
    {
        if (Vector2.Distance(player.transform.position, goalPosition.position) < explosionRadius)
        {
            float px = player.transform.position.x - goalPosition.position.x;
            float py = player.transform.position.y - goalPosition.position.y + explosionOffset;

            player.GetComponent<Rigidbody2D>().AddForce(new Vector2(px, py).normalized * explosionForce / Vector2.Distance(player.transform.position, goalPosition.position), ForceMode2D.Impulse);
        }
    }

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(resetWaitTime);
        ResetBall();
        ResetBluePlayer();
        ResetOrangePlayer();
        gameManager.StartCountdown();
    }

    private void ResetBall()
    {
        ball.SetActive(true);
        ball.transform.position = ballRespawn.position;
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        ball.transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    private void ResetBluePlayer()
    {
        bluePlayer.transform.position = blueRespawn.position;
        Rigidbody2D rb = bluePlayer.GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        bluePlayer.transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    private void ResetOrangePlayer()
    {
        orangePlayer.transform.position = orangeRespawn.position;
        Rigidbody2D rb = orangePlayer.GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        orangePlayer.transform.rotation = Quaternion.Euler(Vector3.zero);
    }
}
