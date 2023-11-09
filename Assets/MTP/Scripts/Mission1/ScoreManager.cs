using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Unjong
{
    public class ScoreManager : MonoBehaviour
    {
        public TMP_Text m_ScoreText;
        public int m_Score;
        public GameObject m_CompleteEffect;
        public GameObject m_EffectPoint;
        public GameObject m_CreditPanel;
        public GameObject m_CompletePanel;
        public GameObject m_ConversationPanel;
        public int m_Goal = 10;

        private AudioManager m_audioManager;
        // Start is called before the first frame update
        void Start()
        {
            m_audioManager = FindObjectOfType<AudioManager>();
        }

        public void IncreaseScore(int amountToIncrease)
        {
            m_Score += amountToIncrease;
            m_ScoreText.text = m_Score.ToString();

            if (m_Score >= m_Goal)
            {
                StartCoroutine(CompleteMission());
            }
        }

        public void ResetScore()
        {
            m_Score = 0;
            m_ScoreText.text = m_Score.ToString();
        }

        public IEnumerator CompleteMission()
        {
            yield return new WaitForSeconds(1.0f);
            GameObject particle = Instantiate(m_CompleteEffect, m_EffectPoint.transform.position, Quaternion.identity, m_EffectPoint.transform);
            Destroy(particle, 3.5f);
            yield return new WaitForSeconds(1.5f);

            m_CompletePanel.SetActive(true);
            //m_ConversationPanel.SetActive(false);
        }
    }
}