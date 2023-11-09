using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Unjong
{
    public class Dot : MonoBehaviour
    , IPointerDownHandler
    , IPointerUpHandler
    {
        [Header("Board Variables")]
        public int m_Column;
        public int m_Row;
        public int m_TargetX;
        public int m_TargetY;
        public int m_PreviousColumn;
        public int m_PreviousRow;
        public bool m_IsMatched = false;
        public float m_SwipeAngle = 0f;
        public float m_SwipeResist = 100.0f;
        public int m_DistanceTile = 200;
        public GameObject m_MoveSound;

        private FindMatches m_FindMatches;
        private Board m_Board;
        private GameObject m_OtherDot;
        private Vector2 m_FirstTouchPosition;
        private Vector2 m_FinalTouchPosition;
        private Vector2 m_TempPosition;
        private RectTransform m_canvasRect;

        // Start is called before the first frame update
        void Start()
        {
            m_Board = FindObjectOfType<Board>();
            m_FindMatches = FindObjectOfType<FindMatches>();
            m_canvasRect = FindObjectOfType<Canvas>().gameObject.GetComponent<RectTransform>();

            m_SwipeResist = m_SwipeResist * m_canvasRect.localScale.y;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            //FindMatches();

            if (m_IsMatched)
            {
                Image myImage = GetComponent<Image>();
                myImage.color = new Color(myImage.color.r, myImage.color.g, myImage.color.b, .2f);
            }
            m_TargetX = m_Column;
            m_TargetY = m_Row;

            if (Mathf.Abs(m_TargetX - transform.GetComponent<RectTransform>().anchoredPosition.x / m_DistanceTile) > .01f)
            {
                m_TempPosition = new Vector2(m_TargetX * m_DistanceTile, transform.GetComponent<RectTransform>().anchoredPosition.y);
                transform.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(transform.GetComponent<RectTransform>().anchoredPosition, m_TempPosition, 0.2f);

                if (m_Board.m_AllDots[m_Column, m_Row] != this.gameObject)
                {
                    m_Board.m_AllDots[m_Column, m_Row] = this.gameObject;
                }
                m_FindMatches.FindAllMatches();
            }
            else
            {
                m_TempPosition = new Vector2(m_TargetX * m_DistanceTile, transform.GetComponent<RectTransform>().anchoredPosition.y);
                transform.GetComponent<RectTransform>().anchoredPosition = m_TempPosition;
                m_Board.m_AllDots[m_Column, m_Row] = this.gameObject;
            }
            if (Mathf.Abs(m_TargetY - transform.GetComponent<RectTransform>().anchoredPosition.y / m_DistanceTile) > .01f)
            {

                m_TempPosition = new Vector2(transform.GetComponent<RectTransform>().anchoredPosition.x, m_TargetY * m_DistanceTile);
                transform.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(transform.GetComponent<RectTransform>().anchoredPosition, m_TempPosition, 0.2f);
                if (m_Board.m_AllDots[m_Column, m_Row] != this.gameObject)
                {
                    m_Board.m_AllDots[m_Column, m_Row] = this.gameObject;
                }
                m_FindMatches.FindAllMatches();
            }
            else
            {
                m_TempPosition = new Vector2(transform.GetComponent<RectTransform>().anchoredPosition.x, m_TargetY * m_DistanceTile);
                transform.GetComponent<RectTransform>().anchoredPosition = m_TempPosition;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (m_Board.m_CurrentState == GameState.move)
            {
                m_FirstTouchPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, .0f);
            }
            //Debug.Log(m_FirstTouchPosition);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (m_Board.m_CurrentState == GameState.move)
            {
                if(m_FirstTouchPosition != new Vector2(0.0f,0.0f))
                {
                    m_FinalTouchPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, .0f);
                    CalculateAngle();
                }
            }

        }
        void CalculateAngle()
        {
            if ((Mathf.Abs(m_FinalTouchPosition.y - m_FirstTouchPosition.y) > m_SwipeResist) || (Mathf.Abs(m_FinalTouchPosition.x - m_FirstTouchPosition.x) > m_SwipeResist))
            {
                m_SwipeAngle = Mathf.Atan2(m_FinalTouchPosition.y - m_FirstTouchPosition.y, m_FinalTouchPosition.x - m_FirstTouchPosition.x);
                m_Board.m_CurrentState = GameState.wait;
                //Debug.Log(m_SwipeAngle);
                MovePieces();
            }
            else
            {
                m_Board.m_CurrentState = GameState.move;
            }
            
        }

        void MovePieces()
        {
            if (m_SwipeAngle > -0.8 && m_SwipeAngle < 0.8 && m_Column < m_Board.m_Width - 1)
            {
                //Right Swipe
                m_OtherDot = m_Board.m_AllDots[m_Column + 1, m_Row];
                m_PreviousColumn = m_Column;
                m_PreviousRow = m_Row;
                m_OtherDot.GetComponent<Dot>().m_Column -= 1;
                m_Column += 1;
            }
            else if (m_SwipeAngle > 0.8 && m_SwipeAngle <= 2.4 && m_Row < m_Board.m_Height - 1)
            {
                //Up Swipe
                m_OtherDot = m_Board.m_AllDots[m_Column, m_Row + 1];
                m_PreviousColumn = m_Column;
                m_PreviousRow = m_Row;
                m_OtherDot.GetComponent<Dot>().m_Row -= 1;
                m_Row += 1;
            }
            else if (m_SwipeAngle > -2.4 && m_SwipeAngle <= -0.8 && m_Row > 0)
            {
                //Down Swipe
                m_OtherDot = m_Board.m_AllDots[m_Column, m_Row - 1];
                m_PreviousColumn = m_Column;
                m_PreviousRow = m_Row;
                m_OtherDot.GetComponent<Dot>().m_Row += 1;
                m_Row -= 1;
            }
            else if ((m_SwipeAngle > 2.4 || m_SwipeAngle <= -2.4) && m_Column > 0)
            {
                //Left Swipe
                m_OtherDot = m_Board.m_AllDots[m_Column - 1, m_Row];
                m_PreviousColumn = m_Column;
                m_PreviousRow = m_Row;
                m_OtherDot.GetComponent<Dot>().m_Column += 1;
                m_Column -= 1;
            }
            GameObject particle = Instantiate(m_MoveSound, transform.position, Quaternion.identity);
            Destroy(particle, 0.5f);
            StartCoroutine(CheckMoveCo());
        }

        public IEnumerator CheckMoveCo()
        {
            yield return new WaitForSeconds(.5f);
            if (m_OtherDot != null)
            {
                if (!m_IsMatched && !m_OtherDot.GetComponent<Dot>().m_IsMatched)
                {
                    m_OtherDot.GetComponent<Dot>().m_Row = m_Row;
                    m_OtherDot.GetComponent<Dot>().m_Column = m_Column;
                    m_Row = m_PreviousRow;
                    m_Column = m_PreviousColumn;
                    yield return new WaitForSeconds(.5f);
                    m_Board.m_CurrentState = GameState.move;
                }
                else
                {
                    //board.basePieceValue = 5; 
                    m_Board.DestroyMatches();
                }
                m_OtherDot = null;
            }
            else
            {
                m_Board.m_CurrentState = GameState.move;
            }
        }
    }
}