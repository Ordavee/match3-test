using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class TheGem : MonoBehaviour
{
    public float swipeAngle = 0, swipeResist = 1f;
    public int column, row;
    public int targetX, targetY;
    public int previousColumn, previousRow;
    public bool isMatched = false;
    public AudioClip[] clipSound;
    public AudioSource audioSound;

    private MatchFinder findMatches;
    private Vector2 firstTouchPosition, finalTouchPosition;
    private Vector2 tilePosition;
    private GameObject otherGem;
    private Board board;

    void Start()
    {
        audioSound = GetComponent<AudioSource>();

        board = FindObjectOfType<Board>();
        findMatches = FindObjectOfType<MatchFinder>();

        //CONVERTENDO FLOAT PRA INT:
        targetX = (int)transform.position.x;    
        targetY = (int)transform.position.y;
    }

    void Update()
    {
        #region COLORINDO MATCHES
        if (isMatched)
        {
            SpriteRenderer matchSprite = GetComponent<SpriteRenderer>();
            matchSprite.color = new Color(0f, 0f, 0f, 2f);
        }
        #endregion

        //REDECLARANDO CONEXÃO:
        targetX = column; 
        targetY = row;

        #region MOVIMENTAÇÃO DE GEMS
        //TROCA DE GEMS EIXO X:
        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {
            tilePosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tilePosition, .6f);

            if(board.allGems[column, row] = this.gameObject)
            {
                board.allGems[column, row] = this.gameObject;
            }

            findMatches.FindAllMatches();
        }
        else
        {
            tilePosition = new Vector2(targetX, transform.position.y);
            transform.position = tilePosition;
        }

        //TROCA DE GEMS EIXO Y:
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            tilePosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tilePosition, .6f);

            if (board.allGems[column, row] = this.gameObject)
            {
                board.allGems[column, row] = this.gameObject;
            }

            findMatches.FindAllMatches();
        }
        else
        {
            tilePosition = new Vector2(transform.position.x, targetY);
            transform.position = tilePosition;
        }
        #endregion

    }

    #region COROUTINA ENCONTRA MATCHES
    IEnumerator Find()
    {
        yield return new WaitForSeconds(1f);

        FindMatches();  //CHAMANDO FUNÇÃO

        StartCoroutine(Find());
    }
    #endregion

    #region ENTRADA E SAÍDA DO MOUSE
    private void OnMouseDown()
    {
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngle();
    }
    #endregion

    #region CALCULANDO ANGULO
    void CalculateAngle()
    {
        if(Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            MovePieces();
        }
    }
    #endregion

    #region TROCA DE GEMS
    void MovePieces()
    {
        if(swipeAngle > -45 && swipeAngle <= 45 && column < 7)  //RIGHT SWIPE
        {
            previousColumn = column;
            previousRow = row;
            otherGem = board.allGems[column + 1, row];
            otherGem.GetComponent<TheGem>().column -= 1;
            column += 1;
        } else if (swipeAngle > 45 && swipeAngle <= 135 && row < 7) //UP SWIPE
        {
            previousColumn = column;
            previousRow = row;
            otherGem = board.allGems[column, row + 1];
            otherGem.GetComponent<TheGem>().row -= 1;
            row += 1;
        } else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)  //LEFT SWIPE
        {
            previousColumn = column;
            previousRow = row;
            otherGem = board.allGems[column - 1, row];
            otherGem.GetComponent<TheGem>().column += 1;
            column -= 1;
        } else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)   //DOWN SWIPE
        {
            previousColumn = column;
            previousRow = row;
            otherGem = board.allGems[column, row - 1];
            otherGem.GetComponent<TheGem>().row += 1;
            row -= 1;
        }

        TocarSom(0);

        StartCoroutine(CheckMove());
    }
    #endregion

    #region ENCONTRANDO MATCHES
    void FindMatches()
    {
        if(column > 0 && column < board.width - 1)
        {
            GameObject leftGem1 = board.allGems[column - 1, row];
            GameObject rightGem1 = board.allGems[column + 1, row];

            if(leftGem1 != null && rightGem1 != null)
            {
                if (leftGem1.tag == this.gameObject.tag && rightGem1.tag == this.gameObject.tag)
                {
                    leftGem1.GetComponent<TheGem>().isMatched = true;
                    rightGem1.GetComponent<TheGem>().isMatched = true;
                    isMatched = true;
                }
            }
        }

        if (row > 0 && row < board.hight - 1)
        {
            GameObject upGem1 = board.allGems[column, row + 1];
            GameObject downGem1 = board.allGems[column, row - 1];

            if(upGem1 != null && downGem1 != null)
            {
                if (upGem1.tag == this.gameObject.tag && downGem1.tag == this.gameObject.tag)
                {
                    upGem1.GetComponent<TheGem>().isMatched = true;
                    downGem1.GetComponent<TheGem>().isMatched = true;
                    isMatched = true;
                }
            }
        }
    }
    #endregion

    #region LIMITAÇÃO DE MOVIMENTO
    IEnumerator CheckMove()
    {
        yield return new WaitForSeconds(.5f);

        if(otherGem != null)
        {
            if (!isMatched && !otherGem.GetComponent<TheGem>().isMatched)
            {
                otherGem.GetComponent<TheGem>().row = row;
                otherGem.GetComponent<TheGem>().column = column;
                row = previousRow;
                column = previousColumn;
            }
            else
            {
                board.DestroyMatches();
            }

            otherGem = null;
        }
    }
    #endregion

    #region AUDIO
    public void TocarSom(int s)
    {
        audioSound.clip = clipSound[s];
        audioSound.Play();
    }
    #endregion
}
