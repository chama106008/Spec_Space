using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.gameObject.tag == "Player")
        {   
            rb= collision.GetComponent<Rigidbody2D>();
            collision.transform.position = Vector3.zero;
            rb.linearVelocity = Vector3.zero;
        }
    }
}
