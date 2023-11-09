using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unjong
{
    public class FindMatches : MonoBehaviour
    {
        private Board board;
        public List<GameObject> currentMatches = new List<GameObject>();

        private ScoreManager m_scoreManager;

        // Start is called before the first frame update
        void Start()
        {
            board = FindObjectOfType<Board>();
            m_scoreManager = FindObjectOfType<ScoreManager>();
        }

        public void FindAllMatches()
        {
            StartCoroutine(FindAllMatchesCo());
        }
        private IEnumerator FindAllMatchesCo()
        {
            yield return new WaitForSeconds(.2f);
            if (m_scoreManager.m_Score < m_scoreManager.m_Goal)
            {
                for (int i = 0; i < board.m_Width; i++)
                {
                    for (int j = 0; j < board.m_Height; j++)
                    {
                        GameObject currentDot = board.m_AllDots[i, j];
                        if (currentDot != null)
                        {
                            if (i > 0 && i < board.m_Width - 1)
                            {
                                GameObject leftDot = board.m_AllDots[i - 1, j];
                                GameObject rightDot = board.m_AllDots[i + 1, j];
                                if (leftDot != null && rightDot != null)
                                {
                                    if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag)
                                    {
                                        if (!currentMatches.Contains(leftDot))
                                        {
                                            currentMatches.Add(leftDot);
                                        }
                                        leftDot.GetComponent<Dot>().m_IsMatched = true;
                                        if (!currentMatches.Contains(rightDot))
                                        {
                                            currentMatches.Add(rightDot);
                                        }
                                        rightDot.GetComponent<Dot>().m_IsMatched = true;
                                        if (!currentMatches.Contains(currentDot))
                                        {
                                            currentMatches.Add(currentDot);
                                        }
                                        currentDot.GetComponent<Dot>().m_IsMatched = true;
                                    }
                                }
                            }

                            if (j > 0 && j < board.m_Height - 1)
                            {
                                GameObject upDot = board.m_AllDots[i, j + 1];
                                GameObject downDot = board.m_AllDots[i, j - 1];
                                if (upDot != null && downDot != null)
                                {
                                    if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag)
                                    {
                                        if (!currentMatches.Contains(upDot))
                                        {
                                            currentMatches.Add(upDot);
                                        }
                                        upDot.GetComponent<Dot>().m_IsMatched = true;
                                        if (!currentMatches.Contains(downDot))
                                        {
                                            currentMatches.Add(downDot);
                                        }
                                        downDot.GetComponent<Dot>().m_IsMatched = true;
                                        if (!currentMatches.Contains(currentDot))
                                        {
                                            currentMatches.Add(currentDot);
                                        }
                                        currentDot.GetComponent<Dot>().m_IsMatched = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }

    }
}