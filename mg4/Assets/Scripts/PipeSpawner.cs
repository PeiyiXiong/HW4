using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    public GameObject pipePrefab; 
    public float spawnRate = 6f;  
    private float timer;
    private float spawnX;   
    private float leftBound; 
    [Header("Random amplitude of the Y-axis of the pipe")]
    public float yRandomRange = 1f; 
    [Header("Overall Y-axis offset of the pipe")]
    public float yOffset = 0.5f; 

    void Start()
    {
        Camera cam = Camera.main;
        float camWidth = cam.orthographicSize * cam.aspect;
        spawnX = cam.transform.position.x + camWidth;
        leftBound = cam.transform.position.x - camWidth - 1f;

        SpawnPipe();
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnRate)
        {
            SpawnPipe();
            timer = 0f;
        }
    }

    void SpawnPipe()
    {
        if (pipePrefab == null)
        {
            Debug.LogError("Pipe Prefab");
            return;
        }

        GameObject pipe = Instantiate(pipePrefab);
        PipeMovement pipeMovement = pipe.GetComponent<PipeMovement>();

        float centerY = Random.Range(-yRandomRange, yRandomRange) + yOffset;

        pipe.transform.position = new Vector3(spawnX, centerY, 0f);
        
    }
}