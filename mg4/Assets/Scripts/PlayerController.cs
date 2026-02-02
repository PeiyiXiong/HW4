using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float flapForce = 5f;
    private Rigidbody2D rb;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isDead) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(Vector2.up * flapForce, ForceMode2D.Impulse);
            GameEvents.RaisePlayerJumped(); 
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDead && collision.gameObject.CompareTag("Pipe"))
        {
            isDead = true;
            GameEvents.RaisePlayerDied(); 
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;
        if (other.CompareTag("ScoreZone"))
        {
            GameEvents.RaiseScoreIncreased(); 
            Destroy(other.gameObject); 
        }
    }
}