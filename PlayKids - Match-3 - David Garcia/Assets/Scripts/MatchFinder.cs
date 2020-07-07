using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchFinder : MonoBehaviour
{
    private Board board;

    public List<GameObject> currentMatches = new List<GameObject>();

    void Start()
    {
        board = FindObjectOfType<Board>();
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCO());
    }

    public IEnumerator FindAllMatchesCO()
    {
        yield return new WaitForSeconds(.2f);
        for(int i = 0; i < board.width; i++)
        {
            for(int j = 0; j < board.hight; j++)
            {
                GameObject currentGem = board.allGems[i, j];
                if(currentGem != null)
                {
                    if(i > 0 && i < board.width - 1)
                    {
                        GameObject leftGem = board.allGems[i - 1, j];
                        GameObject rightGem = board.allGems[i + 1, j];

                        if(leftGem != null && rightGem != null)
                        {
                            if(leftGem.tag == currentGem.tag && rightGem.tag == currentGem.tag)
                            {
                                if (!currentMatches.Contains(leftGem))
                                {
                                    currentMatches.Add(leftGem);
                                }

                                leftGem.GetComponent<TheGem>().isMatched = true;

                                if (!currentMatches.Contains(rightGem))
                                {
                                    currentMatches.Add(rightGem);
                                }

                                rightGem.GetComponent<TheGem>().isMatched = true;

                                if (!currentMatches.Contains(currentGem))
                                {
                                    currentMatches.Add(leftGem);
                                }

                                currentGem.GetComponent<TheGem>().isMatched = true;
                            }
                        }
                    }

                    if (j > 0 && j < board.hight - 1)
                    {
                        GameObject upGem = board.allGems[i, j + 1];
                        GameObject downGem = board.allGems[i, j - 1];

                        if (upGem != null && downGem != null)
                        {
                            if (upGem.tag == currentGem.tag && downGem.tag == currentGem.tag)
                            {
                                if (!currentMatches.Contains(upGem))
                                {
                                    currentMatches.Add(upGem);
                                }

                                upGem.GetComponent<TheGem>().isMatched = true;

                                if (!currentMatches.Contains(downGem))
                                {
                                    currentMatches.Add(downGem);
                                }

                                downGem.GetComponent<TheGem>().isMatched = true;

                                if (!currentMatches.Contains(currentGem))
                                {
                                    currentMatches.Add(currentGem);
                                }

                                currentGem.GetComponent<TheGem>().isMatched = true;
                            }
                        }
                    }
                }
            }
        }
    }

}
