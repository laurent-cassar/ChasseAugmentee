using UnityEngine;
using UnityEngine.UI;

public class ScaryMazeController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private RectTransform playerCursor;

    [Header("Jumpscare")]
    [SerializeField] private GameObject jumpscareImage;   
    [SerializeField] private AudioSource jumpscareAudio;  

    [Header("Settings")]
    [SerializeField] private float followSpeed = 40f;

    private bool isPlaying = true;
    private RectTransform mazeArea;

    private void Start()
    {
        mazeArea = playerCursor.parent as RectTransform;

        // Cache le jumpscare au démarrage
        if (jumpscareImage != null)
            jumpscareImage.SetActive(false);

        playerCursor.gameObject.SetActive(true);
        isPlaying = true;
    }

    private void Update()
    {
        if (!isPlaying) return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            MoveCursor(touch.position);
        }
        else if (Input.GetMouseButton(0))
        {
            MoveCursor(Input.mousePosition);
        }
    }

    private void MoveCursor(Vector2 screenPosition)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            mazeArea,
            screenPosition,
            null,
            out localPoint))
        {
            playerCursor.anchoredPosition = Vector2.Lerp(
                playerCursor.anchoredPosition,
                localPoint,
                followSpeed * Time.deltaTime
            );
        }
    }

    public void OnHitWall()
    {
        if (!isPlaying) return;

        isPlaying = false;
        playerCursor.gameObject.SetActive(false);

        if (jumpscareImage != null)
            jumpscareImage.SetActive(true);

        if (jumpscareAudio != null)
            jumpscareAudio.Play();
    }

    public void OnReachExit()
    {
        if (!isPlaying) return;

        isPlaying = false;
        playerCursor.gameObject.SetActive(false);

        Debug.Log("SORTIE ATTEINTE → Indice débloqué");
    }

    public void Restart()
    {
        if (jumpscareImage != null)
            jumpscareImage.SetActive(false);

        playerCursor.anchoredPosition = Vector2.zero; 
        playerCursor.gameObject.SetActive(true);
        isPlaying = true;
    }
}