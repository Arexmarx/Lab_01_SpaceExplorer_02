using UnityEngine;

public class PurpleBubbleMovement : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 2f;  

    void Update()
    {
        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);

        if (transform.position.y < Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y - 1f)
        {
            Destroy(gameObject);
        }
    }
}
