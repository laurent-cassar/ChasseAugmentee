using UnityEngine;

public class MazeWall : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerCursor"))
        {
            ScaryMazeController controller = FindObjectOfType<ScaryMazeController>();
            if (controller != null)
            {
                controller.OnHitWall();
            }
        }
    }
}