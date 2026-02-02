using UnityEngine;
using TMPro;  

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance; 

    public TMP_Text scoreText; 
    public GameObject gameOverText; 
    [Header("audio setting")]
    public AudioSource audioSource; 
    public AudioClip scoreClip; 
    public AudioClip deathClip; 
    public AudioClip jumpClip; 

    private int score = 0;
    private bool isGameOver = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        GameEvents.OnScoreIncreased += HandleScoreIncreased;
        GameEvents.OnPlayerDied += HandlePlayerDied;
        GameEvents.OnPlayerJumped += HandlePlayerJumped; 

        if (scoreText != null) scoreText.text = "Score: 0";
        if (gameOverText != null) gameOverText.SetActive(false);

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false; 
        }
    }

    private void HandleScoreIncreased(int addValue)
    {
        if (isGameOver || scoreText == null) return;
        
        score += addValue;
        scoreText.text = "Score: " + score;
        
        if (audioSource != null && scoreClip != null)
        {
            audioSource.PlayOneShot(scoreClip);
        }
    }
    private void HandlePlayerDied()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (audioSource != null && deathClip != null)
        {
            audioSource.PlayOneShot(deathClip);
        }

        if (gameOverText != null) gameOverText.SetActive(true);

        PipeMovement[] pipes = FindObjectsOfType<PipeMovement>();
        foreach (var pipe in pipes)
        {
            pipe.enabled = false;
            Rigidbody2D pipeRb = pipe.GetComponent<Rigidbody2D>();
            if (pipeRb != null)
            {
                pipeRb.velocity = Vector2.zero;
                pipeRb.isKinematic = true;
            }
        }

        PlayerController bird = FindObjectOfType<PlayerController>();
        if (bird != null) 
        {
            bird.enabled = false;
            Rigidbody2D birdRb = bird.GetComponent<Rigidbody2D>();
            if (birdRb != null)
            {
                birdRb.velocity = Vector2.zero;
                birdRb.angularVelocity = 0f;
                birdRb.isKinematic = true;
            }
        }

        Time.timeScale = 0f;
        Time.fixedDeltaTime = 0f;
    }

    private void HandlePlayerJumped()
    {
        if (isGameOver) return; 
        if (audioSource != null && jumpClip != null)
        {
            audioSource.PlayOneShot(jumpClip); 
        }
    }

    void OnDestroy()
    {
        GameEvents.OnScoreIncreased -= HandleScoreIncreased;
        GameEvents.OnPlayerDied -= HandlePlayerDied;
        GameEvents.OnPlayerJumped -= HandlePlayerJumped; 
    }
}