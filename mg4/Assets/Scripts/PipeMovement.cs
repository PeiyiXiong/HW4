using UnityEngine;

public class PipeMovement : MonoBehaviour
{
    public float speed = 2f;
    public float gapHeight = 6f;
    public Transform topPipe;
    public Transform bottomPipe;
    public Transform scoreZone; 

    void Start()
    {
        if (topPipe == null || bottomPipe == null || scoreZone == null)
        {
            Debug.LogError("Pipe references not assigned!");
            return;
        }

        bottomPipe.localPosition = new Vector3(0, -gapHeight / 2f, 0);
        topPipe.localPosition = new Vector3(0, gapHeight / 2f, 0);
        scoreZone.localPosition = new Vector3(0.5f, 0, 0); 

    }

    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        float camWidth = Camera.main.orthographicSize * Camera.main.aspect;
        float leftBound = Camera.main.transform.position.x - camWidth - 1f;

        if (transform.position.x < leftBound)
            Destroy(gameObject);
    }
}