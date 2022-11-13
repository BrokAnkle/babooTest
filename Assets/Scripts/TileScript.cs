using UnityEngine;

public enum TILE_POSITION
{
    NONE,
    TOPLEFT,
    TOPCENTER,
    TOPRIGHT,
    MIDLEFT,
    MIDCENTER,
    MIDRIGHT,
    BOTLEFT,
    BOTCENTER,
    BOTRIGHT
}

public class TileScript : MonoBehaviour
{
    [SerializeField] public TILE_POSITION winPosition;  //the position this tile should be in order to win
    Vector2 beginDragPosition;  //cache of this tile position at the moment the player start to drag it
    bool isDragging = false;

    void Update()
    {
        #region Touch
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);    //only get the first touch
            if (touch.phase == TouchPhase.Began)
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touch.position), Camera.main.transform.forward);
                if (hit.transform != null)
                    if (hit.transform == transform)
                        BeginDrag(touch.position);
            }

            if (isDragging)
            {
                //Debug.Log("dragging");
                Drag(Camera.main.ScreenToWorldPoint(touch.position));
                if (touch.phase == TouchPhase.Ended)
                    EndDrag(touch.position);
            }
        }
        #endregion

        #region Mouse
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Camera.main.transform.forward);
                if (hit.transform != null)
                {
                    //Debug.Log(hit.transform.tag, hit.transform);
                    if (hit.transform == transform)
                        BeginDrag(Input.mousePosition);
                }
            }
            if (isDragging)
            {
                Drag((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition));
                if (Input.GetMouseButtonUp(0))
                    EndDrag(Input.mousePosition);
            }
        }
        #endregion
    }

    void BeginDrag(Vector2 pos)
    {
        //Debug.Log("begin drag", this);
        GetComponent<Collider2D>().enabled = false; //disable collider to get the slot's one when drop the tile
        isDragging = true;
        beginDragPosition = transform.position; //store the position at the start of the drag in case of a wrong drop

        //get the slot under the tile to tell it it hold nothing now
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(pos), Camera.main.transform.forward);
        if (hit.transform != null)
        {
            SlotScript slot = hit.transform.GetComponent<SlotScript>();
            if (slot != null)
                slot.holdingTile = TILE_POSITION.NONE;
        }
    }

    void Drag(Vector2 dest)
    {
        transform.position = dest;
    }

    void EndDrag(Vector2 pos)
    {
        transform.position = beginDragPosition; //remove it to the position at the beggining of the drag

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(pos), Camera.main.transform.forward);
        if (hit.transform != null)
            if (hit.transform.CompareTag("Slot"))   //if the player droped the tile on a slot
            {
                if ((GameManager.instance.tileConstraintToTheNextSlot && Vector2.Distance(beginDragPosition, hit.transform.position) == GetComponent<SpriteRenderer>().size.x)
                    || !GameManager.instance.tileConstraintToTheNextSlot)   //check if the game only allow movement to a side slot
                {
                    hit.transform.GetComponent<SlotScript>().holdingTile = winPosition; //tell the slot that it is holding this tile
                    transform.position = (Vector2)hit.transform.position;   //move the tile to the slot position

                    GameManager.instance.nbMoves++;
                    GameManager.instance.CheckWin();
                }
            }

        isDragging = false; //this tile is no longer being dragged
        GetComponent<Collider2D>().enabled = true;  //re enable this tile's collider
        //Debug.Log("end drag", this);
    }
}
