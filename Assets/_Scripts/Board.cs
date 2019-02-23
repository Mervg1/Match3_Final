using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int heigth;
    public int offSet;
    public GameObject tilePrefab;
    private BackgroundTile[,] allTiles;
    public GameObject[] dots;
    public GameObject[,] allDots;
    public int score;
    public Text scoreText;

    // Start is called before the first frame update
    void Start()
    {
        allTiles = new BackgroundTile[width, heigth];
        allDots = new GameObject[width, heigth];
        setUp();
    }

    // Update is called once per frame
    public void setUp()
    {
        for(int i=0; i<width; i++)
        {
            for(int j=0; j<heigth; j++)
            {
                Vector2 tempPosition = new Vector2(i, j + offSet);
                GameObject backGroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
                backGroundTile.transform.parent = this.transform;
                backGroundTile.name = "( " + i + " , " + j + " )";

                int dotToUse = Random.Range(0, dots.Length);
                int maxIteration = 0;
                while(MatchesAt(i, j, dots[dotToUse]) && maxIteration < 100)
                {
                    dotToUse = Random.Range(0, dots.Length);
                    maxIteration++;
                }
                maxIteration = 0;
                GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                dot.GetComponent<Dot>().row = j;
                dot.GetComponent<Dot>().column = i;

                dot.transform.parent = this.transform;
                dot.name = "( " + i + " , " + j + " )";
                allDots[i, j] = dot;
            }
        }
    }

    public void Clear()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < heigth; j++)
            {
                Destroy(allDots[i, j].gameObject);
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if(column > 1 && row > 1)
        {
            if(allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
            {
                return true;
            }
            if (allDots[column, row -1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
            {
                return true;
            }
        }else if(column <= 1 && row <= 1)
        {
            if(row > 1)
            {
                if (allDots[column, row - 1].tag == piece.tag || allDots[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
            if(column > 1)
            {
                if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if(allDots[column, row].GetComponent<Dot>().isMatched)
        {
            Destroy(allDots[column, row]);
            allDots[column, row] = null;
        }
    }

    public void DestroyMatches(){
        for(int i =0; i< width; i++)
        {
            for(int j =0; j<heigth; j++)
            {
                if(allDots[i,j] != null){
                    DestroyMatchesAt(i, j);
                 
                }
                
            }
        }
        StartCoroutine(DecreaseRowCo());
        UpScore();
    }

    private IEnumerator DecreaseRowCo()
    {
        int nullCount = 0;
        for(int i =0; i < width; i++)
        {
            for(int j = 0; j < heigth; j++)
            {
                if (allDots[i, j] == null)
                {
                    nullCount++;
                    score+= 5;
                }else if(nullCount > 0)
                {
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard()
    {
        for(int i =0; i < width; i++){
            for(int j =0; j < heigth; j++){
                if(allDots[i, j] == null){
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int dotToUse = Random.Range(0, dots.Length);
                    GameObject piece = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    allDots[i, j] = piece;
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().column = i;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < heigth; j++)
            {
                if(allDots[i, j]!= null)
                {
                    if(allDots[i, j].GetComponent<Dot>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(.5f);

        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
    }

    public void UpScore()
    {
        //score += 20;
        scoreText.text = "Score: " + score;
    }

    public void SetTrueShoot()
    {
        for(int i=0; i<width; i++)
        {
            for(int j=0; j<heigth; j++)
            {
                allDots[i, j].GetComponent<Dot>().canShoot = true;
            }
        }
        StartCoroutine(waitToChangeFalse());
    }

    IEnumerator waitToChangeFalse()
    {
        yield return new WaitForSeconds(3.0f);
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < heigth; j++)
            {
                allDots[i, j].GetComponent<Dot>().canShoot = false;
            }
        }
        DestroyMatches();
    }
}
