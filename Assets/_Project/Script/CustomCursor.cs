using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    public static CustomCursor Instance { get; private set; }

    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Vector2 hotspot = Vector2.zero;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SetDefaultCursor();
    }

    public void SetDefaultCursor()
    {
        Cursor.SetCursor(defaultCursor, hotspot, CursorMode.Auto);
    }

    public void SetCursor(Texture2D cursor)
    {
        Cursor.SetCursor(cursor, hotspot, CursorMode.Auto);
    }
}
