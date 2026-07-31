using UnityEngine;
using UnityEngine.InputSystem;

public class GameCursor : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject cursorObject;

    Vector3 mousePosition;
    Vector3 worldPosition;
    bool isPlaying;

    private void Update()
    {
        if (GameManager.instance == null)
        {
            return;
        }

        isPlaying = GameManager.instance.State == GameState.Playing;

        Cursor.visible = !isPlaying;
        cursorObject.SetActive(isPlaying);

        if ((isPlaying == false)|| (Mouse.current == null))
        {
            return;
        }

        mousePosition = Mouse.current.position.ReadValue();
        worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

        worldPosition.z = 0f;
        cursorObject.transform.position = worldPosition;
    }

    private void OnDisable()
    {
        Cursor.visible = true;

    }
}