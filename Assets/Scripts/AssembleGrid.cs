using UnityEngine;


public class AssembleGrid : MonoBehaviour
{

    [Header("Size")]
    [SerializeField] private Vector2 cellSize = new Vector2(1f, 1.8f);
    [SerializeField] private int trackWidth;

    [Header("Sprites")]
    [SerializeField] private SpriteRenderer bodySprite;
    [SerializeField] private SpriteRenderer leftTrackSprite;
    [SerializeField] private SpriteRenderer rightTrackSprite;

    [Header("Collider")]
    [SerializeField] private BoxCollider2D bodyCollider;
    [SerializeField] private BoxCollider2D leftTrackCollider;
    [SerializeField] private BoxCollider2D rightTrackCollider;



    public Vector2 CellSize => cellSize;

    private void Start()
    {
        Debug.Log("AssembleGrid Start");
        
    }

    public void RefreshGrid(int maxX, int maxY)
    {
        Debug.Log("RefreshGrid Start");
        int bodyWidth = maxX * 2 + 1;
        int bodyHeight = maxY * 2 + 3;

        if (bodyWidth >= 10)
        {
            trackWidth = bodyWidth / 5;
        }
        else
            trackWidth = 1;


        Vector2 bodySize = new Vector2(bodyWidth * cellSize.x, bodyHeight * cellSize.y);
        bodySprite.size = bodySize;
        bodyCollider.size = bodySize;
        bodySprite.transform.localPosition = Vector3.zero;

        Vector2 trackSize = new Vector2(trackWidth * cellSize.x, bodyHeight * cellSize.y);
        leftTrackSprite.size = trackSize;
        rightTrackSprite.size = trackSize;

        leftTrackCollider.size = trackSize;
        rightTrackCollider.size = trackSize;

        // track 중심 위치
        float trackPositionX = (bodySize.x*0.5f)+(trackSize.x*0.5f);
        // 코어 기준
        leftTrackSprite.transform.localPosition = new Vector2(-trackPositionX, 0f);
        rightTrackSprite.transform.localPosition = new Vector2(trackPositionX, 0f);


    }

    // 이하는 셀 단위 grid
    //[SerializeField] SpriteRenderer sr;

    //public Vector2Int CellPosition { get; private set; }
    //public GridCellType CellType { get; private set; }

    //public void Initialize(Vector2Int cellPosition, Sprite sprite, GridCellType gridCellType)
    //{
    //    CellPosition = cellPosition;
    //    sr.sprite = sprite;
    //    CellType = gridCellType;

    //}



}
