using UnityEngine;

public class MovingRootBehaviour : MonoBehaviour
{
    [SerializeField] public bool isInitialazed;
    [SerializeField] public Vector2 moveVector;
    [SerializeField] public float speed;
    [SerializeField] private Rigidbody2D rb;

    private void Awake()
    {
        if(rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInitialazed)
            return;

    }



    public void InitializeAttack(Vector2 vector)
    {
        Debug.Log("RootAttack1: "+ (rb != null));

        if (rb != null)
        {
            Debug.Log("RootAttack2");

            if (vector.x < 0)
                GetComponent<SpriteRenderer>().flipX = true;
            rb.linearVelocity = vector.normalized * speed;
        }
    }

    
}
