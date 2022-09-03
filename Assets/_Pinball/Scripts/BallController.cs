using UnityEngine;
using System.Collections;
using SgLib;

public class BallController : MonoBehaviour
{

    private GameManager gameManager;
    private SpriteRenderer spriteRenderer;
    private bool isChecked;
    // Use this for initialization
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        gameObject.SetActive(false);
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.position += (Random.value >= 0.5f) ? (new Vector3(0.2f, 0)) : (new Vector3(-0.2f, 0));
        gameObject.SetActive(true);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Dead") && !gameManager.gameOver)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.eploring);
            gameManager.CheckGameOver(gameObject);
            Exploring();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Gold") && !gameManager.gameOver)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.hitGold);
            ScoreManager.Instance.AddScore(1);
            gameManager.CheckAndUpdateValue();

            ParticleSystem particle = Instantiate(gameManager.hitGold, other.transform.position, Quaternion.identity) as ParticleSystem;
            var main = particle.main;
            main.startColor = other.gameObject.GetComponent<SpriteRenderer>().color;
            particle.Play();
            Destroy(particle.gameObject, 1f);
            Destroy(other.gameObject);
            gameManager.CreateTarget();
        }
    }

    /// <summary>
    /// Handle when player die
    /// </summary>
    public void Exploring()
    {
        ParticleSystem particle = Instantiate(gameManager.die, transform.position, Quaternion.identity) as ParticleSystem;
        var main = particle.main;
        main.startColor = spriteRenderer.color;
        particle.Play();
        Destroy(particle.gameObject, 1f);
        Destroy(gameObject);
    }

}
