using UnityEngine;

public class MovingRootBehaviour : MonoBehaviour
{
    [SerializeField] public bool isInitialazed;
    [SerializeField] public Vector2 moveVector;
    [SerializeField] public float speed;
    private Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        InitializeAttack(Vector2.right);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInitialazed)
            return;

    }



    public void InitializeAttack(Vector2 vector)
    {
        if(rb != null)
        {
            rb.linearVelocity = vector.normalized * speed;
        }
    }
}
