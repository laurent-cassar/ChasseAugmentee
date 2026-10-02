using UnityEngine;

public class MazeExit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerCursor"))
        {
            ScaryMazeController controller = Object.FindFirstObjectByType<ScaryMazeController>();
            if (controller != null)
            {
                controller.OnReachExit();
            }
        }
    }
}