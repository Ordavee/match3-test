using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;

public class Board : MonoBehaviour
{
    public int scorePlus;
    public int width, hight;
    public int offSet;
    public GameObject tilePrefab;
    public GameObject[] gems = new GameObject[7];
    public GameObject[,] allGems;
    private MatchFinder findMatches;
    public AudioSource audioSound;

    private BackgroundTile[,] tiles;

    void Start()
    {
        findMatches = FindObjectOfType<MatchFinder>();
        tiles = new BackgroundTile[width, hight];
        allGems = new GameObject[width, hight];
        SetUp();
    }

    #region SETANDO MATRIZ
    private void SetUp()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < hight; j++)
            {
                Vector2 tilePosition = new Vector2(i, j + offSet);
                GameObject backgroundTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "( " + i + " , " + j + " )";

                int gemsToUse = Random.Range(0, 7);

                #region RECONFIGURAÇÃO DAS PEÇAS
                int maxChanges = 0;

                while(Matches(i, j, gems[gemsToUse]) && maxChanges < 100)
                {
                    gemsToUse = Random.Range(0, 7);
                    maxChanges++;
                }
                maxChanges = 0;
                #endregion

                GameObject gem = Instantiate(gems[gemsToUse], tilePosition, Quaternion.identity);
                gem.GetComponent<TheGem>().row = j;
                gem.GetComponent<TheGem>().column = i;
                gem.transform.parent = this.transform;
                gem.name = "( " + i + " , " + j + " )";
                allGems[i, j] = gem;
            }
        }
    }
    #endregion

    #region ANALISANDO MATCHES
    private bool Matches(int column, int row, GameObject piece)
    {
        if(column > 1 && row > 1)
        {
            if(allGems[column - 1, row].tag == piece.tag && allGems[column - 2, row])
            {
                return true;
            }

            if (allGems[column, row - 1].tag == piece.tag && allGems[column, row - 2])
            {
                return true;
            }
        } else if(column <= 1 || row <= 1)
        {
            if(row > 1)
            {
                if(allGems[column, row - 1].tag == piece.tag && allGems[column, row - 2])
                {
                    return true;
                }
            }

            if (column > 1)
            {
                if (allGems[column - 1, row].tag == piece.tag && allGems[column - 2, row])
                {
                    return true;
                }
            }
        }

        return false;
    }
    #endregion

    #region AUXILIAR DE DESTRUIÇÃO
    private void DestroyMatchesAux(int column, int row)
    {
        if(allGems[column, row].GetComponent<TheGem>().isMatched)
        {
            findMatches.currentMatches.Remove(allGems[column, row]);
            Destroy(allGems[column, row]);
            allGems[column, row] = null;
        }
    }
    #endregion

    #region DESTRUINDO MATCHES
    public void DestroyMatches()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < hight; j++)
            {
                if(allGems[i, j] != null)
                {
                    DestroyMatchesAux(i, j);
                }
            }
        }

        Score.scoreCount = Score.scoreCount + scorePlus;
        audioSound.Play();

        StartCoroutine(DecreaseRow());
    }
    #endregion

    #region DECREASE ROW
    private IEnumerator DecreaseRow()
    {
        int nullCount = 0;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < hight; j++)
            {
                if (allGems[i, j] == null)
                {
                    nullCount++;
                }else if (nullCount > 0)
                {
                    allGems[i, j].GetComponent<TheGem>().row -= nullCount;
                    allGems[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoard());
    }
    #endregion

    #region REPONDO O TABULEIRO
    private void RefilBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for(int j = 0; j < hight; j++)
            {
                if(allGems[i, j] == null)
                {
                    Vector2 tilePosition = new Vector2(i, j + offSet);
                    int gemsToUse = Random.Range(0, gems.Length);
                    GameObject piece = Instantiate(gems[gemsToUse], tilePosition, Quaternion.identity);
                    allGems[i, j] = piece;
                    piece.GetComponent<TheGem>().row = j;
                    piece.GetComponent<TheGem>().column = i;
                }
            }
        }
    }
    #endregion

    #region ENCONTRANDO MATCHES
    private bool MatchesOnBoard()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < hight; j++)
            {
                if(allGems[i, j] != null)
                {
                    if(allGems[i, j].GetComponent<TheGem>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    #endregion

    #region COROUTINE QUE CHAMA REFLBOARD() E DESTROYMATCHES()
    private IEnumerator FillBoard()
    {
        RefilBoard();
        yield return new WaitForSeconds(.5f);

        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
    }
    #endregion
}
