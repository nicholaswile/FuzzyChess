using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    [SerializeField] private List<CursorObject> cursorObjectList;
    [SerializeField] private Camera cam;

    private CursorObject cursorObject;

    public enum CursorType 
    {
        Default,
        Select,
        Place, 
        Grab,
        Unavailable
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetActiveCursorType(CursorType.Default);
    }

    private void Update()
    {
        Cursor.SetCursor(cursorObject.textureArray, new Vector2(.5f, .5f), CursorMode.Auto);

        if (Input.GetMouseButton(1) && cam.isActiveAndEnabled)
            CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Grab);

        else CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Default);

    }

    public void SetActiveCursorType(CursorType cursorType) 
    {
        SetActiveCursorObject(GetCursorObject(cursorType));
    }

    private CursorObject GetCursorObject(CursorType cursorType) 
    {
        foreach (CursorObject cursorObject in cursorObjectList) 
        {
            if (cursorObject.cursorType == cursorType) 
            {
                return cursorObject;
            }
        }
        return null;
    }

    private void SetActiveCursorObject(CursorObject cursorObject) 
    {
        this.cursorObject = cursorObject;
    }

    [System.Serializable]
    public class CursorObject {
        public CursorType cursorType;
        public Texture2D textureArray;
    }
}
