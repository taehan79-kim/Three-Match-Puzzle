using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unjong
{
    public enum GameState
    {
        wait,
        move
    }

    public class Board : MonoBehaviour
    {
        public GameState m_CurrentState = GameState.move;
        public int m_Width;
        public int m_Height;
        public float m_Offset;
        public int m_DistanceTile = 200;
        public int m_BasePieceValue = 1;
        public int m_StreakValue = 1;
        public GameObject[,] m_AllDots;
        public GameObject m_TilePrefab;
        public GameObject m_DestroyEffect;
        public GameObject m_EffectPoint;
        public GameObject[] m_Dots;
        public AudioSource m_DestroyAudio;


        private BackgroundTile[,] m_AllTiles;
        private FindMatches m_FindMatches;
        private ScoreManager m_ScoreManager;
        private RectTransform m_canvasRect;


        // Start is called before the first frame update
        void Start()
        {
            m_canvasRect = FindObjectOfType<Canvas>().gameObject.GetComponent<RectTransform>();
            m_Offset = m_Offset / m_canvasRect.localScale.y;

            m_ScoreManager = FindObjectOfType<ScoreManager>();
            m_FindMatches = FindObjectOfType<FindMatches>();
            m_AllTiles = new BackgroundTile[m_Width, m_Height];
            m_AllDots = new GameObject[m_Width, m_Height];
            SetUp();
        }

        private void SetUp()
        {
            for (int i = 0; i < m_Width; i++)
            {
                for (int j = 0; j < m_Height; j++)
                {
                    Vector2 m_TempPosition = new Vector2(i * m_DistanceTile, (j + m_Offset) * m_DistanceTile);
                    GameObject backgroundTile = Instantiate(m_TilePrefab, m_TempPosition, Quaternion.identity, transform);
                    backgroundTile.GetComponent<RectTransform>().anchoredPosition = m_TempPosition;
                    backgroundTile.name = "( " + i + ", " + j + " )";
                    int dotToUse = Random.Range(0, m_Dots.Length);

                    int maxIterations = 0;
                    while (MatchesAt(i, j, m_Dots[dotToUse]) && maxIterations < 100)
                    {
                        dotToUse = Random.Range(0, m_Dots.Length);
                        maxIterations++;
                    }
                    maxIterations = 0;

                    GameObject dot = Instantiate(m_Dots[dotToUse], m_TempPosition, Quaternion.identity, this.transform);//transform.position + m_TempPosition
                    dot.GetComponent<RectTransform>().anchoredPosition = m_TempPosition;
                    dot.GetComponent<Dot>().m_Row = j;
                    dot.GetComponent<Dot>().m_Column = i;
                    dot.name = "( " + i + ", " + j + " )";
                    m_AllDots[i, j] = dot;
                }
            }

        }

        public void Reset()
        {
            ShuffleBoard();
            m_ScoreManager.ResetScore();
        }

        public void SuffleB()
        {
            ShuffleBoard();
        }

        private bool MatchesAt(int m_Column, int m_Row, GameObject piece)
        {
            if (m_Column > 1 && m_Row > 1)
            {
                if (m_AllDots[m_Column - 1, m_Row].tag == piece.tag && m_AllDots[m_Column - 2, m_Row].tag == piece.tag)
                {
                    return true;
                }
                if (m_AllDots[m_Column, m_Row - 1].tag == piece.tag && m_AllDots[m_Column, m_Row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
            else if (m_Column <= 1 || m_Row <= 1)
            {
                if (m_Row > 1)
                {
                    if (m_AllDots[m_Column, m_Row - 1].tag == piece.tag && m_AllDots[m_Column, m_Row - 2].tag == piece.tag)
                    {
                        return true;
                    }
                }
                if (m_Column > 1)
                {
                    if (m_AllDots[m_Column - 1, m_Row].tag == piece.tag && m_AllDots[m_Column - 2, m_Row].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void DestroyMatchesAt(int m_Column, int m_Row)
        {
            if (m_AllDots[m_Column, m_Row].GetComponent<Dot>().m_IsMatched)
            {
                if (m_AllDots[m_Column, m_Row].tag == "Dot_White")
                {
                    // Special Dot fuction
                }
                m_ScoreManager.IncreaseScore(m_BasePieceValue * m_StreakValue);

                m_FindMatches.currentMatches.Remove(m_AllDots[m_Column, m_Row]);
                GameObject particle = Instantiate(m_DestroyEffect, m_AllDots[m_Column, m_Row].transform.position, Quaternion.identity, m_EffectPoint.transform);
                Destroy(particle, 0.5f);
                if (!m_DestroyAudio.isPlaying)
                {
                    m_DestroyAudio.Play();
                }
                Destroy(m_AllDots[m_Column, m_Row]);
                m_AllDots[m_Column, m_Row] = null;
            }
        }

        public void DestroyMatches()
        {
            for (int i = 0; i < m_Width; i++)
            {
                for (int j = 0; j < m_Height; j++)
                {
                    if (m_AllDots[i, j] != null)
                    {
                        DestroyMatchesAt(i, j);
                    }
                }
            }
            StartCoroutine(DecreaseRowCo());
        }

        private IEnumerator DecreaseRowCo()
        {
            int nullCount = 0;
            for (int i = 0; i < m_Width; i++)
            {
                for (int j = 0; j < m_Height; j++)
                {
                    if (m_AllDots[i, j] == null)
                    {
                        nullCount++;
                    }
                    else if (nullCount > 0)
                    {
                        m_AllDots[i, j].GetComponent<Dot>().m_Row -= nullCount;
                        m_AllDots[i, j] = null;
                    }
                }
                nullCount = 0;
            }
            yield return new WaitForSeconds(.4f);
            StartCoroutine(FillBoardCo());

        }

        private void RefillBoard()
        {
            for (int i = 0; i < m_Width; i++)
            {
                for (int j = 0; j < m_Height; j++)
                {
                    if (m_AllDots[i, j] == null)
                    {
                        Vector2 m_TempPosition = new Vector2(i * m_DistanceTile, (j + m_Offset) * m_DistanceTile);
                        int dotToUse = Random.Range(0, m_Dots.Length);
                        GameObject piece = Instantiate(m_Dots[dotToUse], m_TempPosition, Quaternion.identity, transform); // transform.position +
                        piece.GetComponent<RectTransform>().anchoredPosition = m_TempPosition;
                        piece.name = "( " + i + ", " + j + " )";
                        m_AllDots[i, j] = piece;
                        piece.GetComponent<Dot>().m_Row = j;
                        piece.GetComponent<Dot>().m_Column = i;
                    }
                }
            }
        }

        private bool MatchesOnBoard()
        {
            for (int i = 0; i < m_Width; i++)
            {
                for (int j = 0; j < m_Height; j++)
                {
                    if (m_AllDots[i, j] != null)
                    {
                        if (m_AllDots[i, j].GetComponent<Dot>().m_IsMatched)
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
            if (MatchesOnBoard())
            {
                while (MatchesOnBoard())
                {
                    m_StreakValue += 1;
                    yield return new WaitForSeconds(.5f);
                    DestroyMatches();
                }
            }
            else
            {
                yield return new WaitForSeconds(.5f);
                if (IsDeadlocked())
                {
                    m_CurrentState = GameState.wait;
                    ShuffleBoard();
                    //Debug.Log("DeadLocked!");
                }
                m_StreakValue = 1;
                m_CurrentState = GameState.move;
            }
        }

        private void SwitchPieces(int m_Column, int m_Row, Vector2 direction)
        {
            GameObject holder = m_AllDots[m_Column + (int)direction.x, m_Row + (int)direction.y] as GameObject;
            m_AllDots[m_Column + (int)direction.x, m_Row + (int)direction.y] = m_AllDots[m_Column, m_Row];
            m_AllDots[m_Column, m_Row] = holder;
        }

        private bool CheckForMatches()
        {
            for (int i = 0; i < m_Width; i++)
            {
                for (int j = 0; j < m_Height; j++)
                {
                    if (m_AllDots[i, j] != null)
                    {
                        if (i < m_Width - 2)
                        {
                            if (m_AllDots[i + 1, j] != null && m_AllDots[i + 2, j] != null)
                            {
                                if (m_AllDots[i + 1, j].tag == m_AllDots[i, j].tag && m_AllDots[i + 2, j].tag == m_AllDots[i, j].tag)
                                {
                                    return true;
                                }
                            }
                        }
                        if (j < m_Height - 2)
                        {
                            if (m_AllDots[i, j + 1] != null && m_AllDots[i, j + 2] != null)
                            {
                                if (m_AllDots[i, j + 1].tag == m_AllDots[i, j].tag && m_AllDots[i, j + 2].tag == m_AllDots[i, j].tag)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool SwitchAndCheck(int m_Column, int m_Row, Vector2 direction)
        {
            SwitchPieces(m_Column, m_Row, direction);
            if (CheckForMatches())
            {
                SwitchPieces(m_Column, m_Row, direction);
                return true;
            }
            SwitchPieces(m_Column, m_Row, direction);
            return false;
        }

        private bool IsDeadlocked()
        {
            for (int i = 0; i < m_Width; i++)
            {
                for (int j = 0; j < m_Height; j++)
                {
                    if (m_AllDots[i, j] != null)
                    {
                        if (i < m_Width - 1)
                        {
                            if (SwitchAndCheck(i, j, Vector2.right))
                            {
                                return false;
                            }
                        }
                        if (j < m_Height - 1)
                        {
                            if (SwitchAndCheck(i, j, Vector2.up))
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        private void ShuffleBoard()
        {
            List<GameObject> newBoard = new List<GameObject>();
            for (int i = 0; i < m_Width; i++)
            {
                for (int j = 0; j < m_Height; j++)
                {
                    if (m_AllDots[i, j] != null)
                    {
                        newBoard.Add(m_AllDots[i, j]);
                    }
                }
            }
            for (int i = 0; i < m_Width; i++)
            {
                for (int j = 0; j < m_Height; j++)
                {
                    if (m_AllDots[i, j] != null)
                    {
                        int pieceToUse = Random.Range(0, newBoard.Count);

                        // 3개짝이 이미 존재하는 지 확인
                        int maxIterations = 0;
                        while (MatchesAt(i, j, newBoard[pieceToUse]) && maxIterations < 100)
                        {
                            pieceToUse = Random.Range(0, newBoard.Count);
                            maxIterations++;
                        }
                        // 4,4와 4,3에서 3개짝이 만들어졌을 시 다시 셔플
                        if (i == 4 && maxIterations == 100)
                        {
                            maxIterations = 0;

                            Dot piece1 = newBoard[pieceToUse].GetComponent<Dot>();
                            piece1.m_Column = i;
                            piece1.m_Row = j;
                            m_AllDots[i, j] = newBoard[pieceToUse];
                            newBoard.Remove(newBoard[pieceToUse]);
                            if (j == 3)
                            {
                                j += 1;
                                pieceToUse = Random.Range(0, newBoard.Count);
                                piece1 = newBoard[pieceToUse].GetComponent<Dot>();
                                piece1.m_Column = i;
                                piece1.m_Row = j;
                                m_AllDots[i, j] = newBoard[pieceToUse];
                                newBoard.Remove(newBoard[pieceToUse]);
                            }
                            ShuffleBoard();
                        }
                        else
                        {
                            maxIterations = 0;

                            Dot piece = newBoard[pieceToUse].GetComponent<Dot>();
                            piece.m_Column = i;
                            piece.m_Row = j;
                            m_AllDots[i, j] = newBoard[pieceToUse];
                            newBoard.Remove(newBoard[pieceToUse]);
                        }

                    }
                }
            }

            if (IsDeadlocked())
            {
                ShuffleBoard();
            }
            m_CurrentState = GameState.move;
        }
    }
}
