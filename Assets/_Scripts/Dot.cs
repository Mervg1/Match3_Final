using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    public int column;
    public int row;
    public int previousColumn;
    public int previousRow;
    public int targetX;
    public int targetY;
    public bool isMatched = false;
    private Board board;
    private GameObject otherDot;
    private Vector2 firstTouch;
    private Vector2 finalTouch;
    private Vector2 tempPosition;
    public float swipeAngle = 0;
    public float swipeResist = 1f;
    private bool isSelected;
    public SpriteRenderer render;
    private static Dot previousSelected;
    public string type;

    public bool canShoot = false;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        //targetX = (int)transform.position.x;
        //targetY = (int)transform.position.y;
        //row = targetY;
        //column = targetX;
        //previousColumn = column;
        //previousRow = row;
    }

    // Update is called once per frame
    void Update()
    {
        FindMatches();
        if (isMatched)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(0f, 0f, 0f, 1f);

        }
        targetX = column;
        targetY = row;
        if(Mathf.Abs(targetX - transform.position.x) > .1)
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if(board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
        }
        else
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
            //board.allDots[column, row] = this.gameObject;
        }
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
        }
        else
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
            //board.allDots[column, row] = this.gameObject;
        }
    }

    void Awake()
    {
        render = GetComponent<SpriteRenderer>();
    }

    private void Select()
    {
        isSelected = true;
        render.color = new Color(.5f, .5f, .5f, 1.0f);
        previousSelected = gameObject.GetComponent<Dot>();
    }

    private void Deselect()
    {
        isSelected = false;
        render.color = Color.white;
        previousSelected = null;
    }

    public IEnumerator checkMoveCor()
    {
        yield return new WaitForSeconds(0.5f);
        if(otherDot != null)
        {
            if(!isMatched && !otherDot.GetComponent<Dot>().isMatched)
            {
                otherDot.GetComponent<Dot>().row = row;
                otherDot.GetComponent<Dot>().column = column;
                row = previousRow;
                column = previousColumn;
            }
            else
            {
                board.DestroyMatches();
            }
            otherDot = null;
        }

    }

    private void OnMouseDown()
    {
        Debug.Log(canShoot);
        if (!canShoot)
        {
            firstTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (isSelected == true)
            {
                Deselect();
            }
            else
            {
                if (previousSelected == null)
                {
                    Select();
                }
                else
                {
                    previousSelected.Deselect();
                    Select();
                }
            }
        }
        if (canShoot)
        {
            isMatched = true;
        }
    }

    private void OnMouseUp()
    {
        if (!canShoot)
        {
            finalTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }

    void CalculateAngle()
    {
        if(Mathf.Abs(finalTouch.y - firstTouch.y) > swipeResist || Mathf.Abs(finalTouch.x - firstTouch.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(finalTouch.y - firstTouch.y, finalTouch.x - firstTouch.x) * 180 / Mathf.PI;
            MovePieces();
        }
    }

    void MovePieces()
    {
        if (this.gameObject.tag != "Rock")
        {
            if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1)
            {
                otherDot = board.allDots[column + 1, row];
                if (otherDot.tag != "Rock")
                {
                    previousColumn = column;
                    previousRow = row;
                    otherDot.GetComponent<Dot>().column -= 1;
                    column += 1;
                }
            }
            else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.heigth - 1)
            {
                otherDot = board.allDots[column, row + 1];
                if (otherDot.tag != "Rock")
                {
                    previousColumn = column;
                    previousRow = row;
                    otherDot.GetComponent<Dot>().row -= 1;
                    row += 1;
                }
            }
            else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
            {
                otherDot = board.allDots[column - 1, row];
                if (otherDot.tag != "Rock")
                {
                    previousColumn = column;
                    previousRow = row;
                    otherDot.GetComponent<Dot>().column += 1;
                    column -= 1;
                }
            }
            else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
            {
                otherDot = board.allDots[column, row - 1];
                if (otherDot.tag != "Rock")
                {
                    previousColumn = column;
                    previousRow = row;
                    otherDot.GetComponent<Dot>().row += 1;
                    row -= 1;
                }
            }
            if (otherDot.tag != "Rock")
                StartCoroutine(checkMoveCor());
        }
    }

    void FindMatches()
    {
        if(column > 0 && column < board.width - 1)
        {
            GameObject leftDot = board.allDots[column - 1, row];
            GameObject rightDot = board.allDots[column + 1, row];
            if(leftDot != null && rightDot != null)
            {
                if (leftDot.tag == this.gameObject.tag && rightDot.tag == this.gameObject.tag)
                {
                    if(leftDot.GetComponent<Dot>().type == "Target")
                    {
                        Shoot();
                    }
                    if(rightDot.GetComponent<Dot>().type == "Target")
                    {
                        Shoot();
                    }
                    if(type == "Target")
                    {
                        Shoot();
                    }

                    if (leftDot.GetComponent<Dot>().type == "Mush")
                    {
                        board.Clear();
                        board.setUp();
                    }
                    if (rightDot.GetComponent<Dot>().type == "Mush")
                    {
                        board.Clear();
                        board.setUp();
                    }
                    if (type == "Mush")
                    {
                        board.Clear();
                        board.setUp();
                    }

                    if (leftDot.GetComponent<Dot>().type == "Bomb")
                    {
                        BombExplosion();
                    }
                    if (rightDot.GetComponent<Dot>().type == "Bomb")
                    {
                        BombExplosion();
                    }
                    if (type == "Bomb")
                    {
                        BombExplosion();
                    }

                    leftDot.GetComponent<Dot>().isMatched = true;
                    rightDot.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
        }
        if (row > 0 && row < board.heigth - 1)
        {
            GameObject upDot = board.allDots[column, row + 1];
            GameObject downDot = board.allDots[column, row - 1];
            if (upDot != null && downDot != null)
            {
                if (upDot.tag == this.gameObject.tag && downDot.tag == this.gameObject.tag)
                {
                    if (upDot.GetComponent<Dot>().type == "Target")
                    {
                        Shoot();
                    }
                    if (downDot.GetComponent<Dot>().type == "Target")
                    {
                        Shoot();
                    }
                    if (type == "Target")
                    {
                        Shoot();
                    }

                    if (upDot.GetComponent<Dot>().type == "Mush")
                    {
                        board.Clear();
                        board.setUp();
                    }
                    if (downDot.GetComponent<Dot>().type == "Mush")
                    {
                        board.Clear();
                        board.setUp();
                    }
                    if (type == "Mush")
                    {
                        board.Clear();
                        board.setUp();
                    }

                    if (upDot.GetComponent<Dot>().type == "Bomb")
                    {
                        BombExplosion();
                    }
                    if (downDot.GetComponent<Dot>().type == "Bomb")
                    {
                        BombExplosion();
                    }
                    if (type == "Bomb")
                    {
                        BombExplosion();
                    }

                    upDot.GetComponent<Dot>().isMatched = true;
                    downDot.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
        }
    }

    void Shoot()
    {
        board.SetTrueShoot();
    }

    void BombExplosion()
    {
        if (column > 0 && column < board.width - 2)
        {
            GameObject leftDot = board.allDots[column - 1, row];
            GameObject leftLeftDot = board.allDots[column - 2, row];
            GameObject rightDot = board.allDots[column + 1, row];
            GameObject rightrightDot = board.allDots[column + 2, row];
            if (leftDot != null && leftLeftDot != null && leftDot != null && leftLeftDot != null)
            {
                leftDot.GetComponent<Dot>().isMatched = true;
                leftLeftDot.GetComponent<Dot>().isMatched = true;
                rightDot.GetComponent<Dot>().isMatched = true;
                rightrightDot.GetComponent<Dot>().isMatched = true;
                isMatched = true;
            }
        }
        if (row > 0 && row < board.heigth - 2)
        {
            GameObject upDot = board.allDots[column, row + 1];
            GameObject upUpDot = board.allDots[column, row + 2];
            GameObject downDot = board.allDots[column, row - 1];
            GameObject downDownDot = board.allDots[column, row - 2];
            if (upDot != null && upUpDot != null && downDot != null && downDownDot != null)
            {
                upDot.GetComponent<Dot>().isMatched = true;
                upUpDot.GetComponent<Dot>().isMatched = true;
                downDot.GetComponent<Dot>().isMatched = true;
                downDownDot.GetComponent<Dot>().isMatched = true;
                isMatched = true;
            }
        }
    }
}
