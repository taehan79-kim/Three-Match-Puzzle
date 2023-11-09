using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unjong
{
    public class ButtonCtl : MonoBehaviour
    {
        public GameObject m_ActivePanel;
        public string m_SceneString;


        public void OpenPanel(float i)
        {
            StartCoroutine(OpenCo(i));

        }

        public IEnumerator OpenCo(float i)
        {
            yield return new WaitForSeconds(i);
            m_ActivePanel.SetActive(true);

        }

        public void Cancle(float i)
        {
            StartCoroutine(CancleCo(i));
        }

        public void OpenScene()
        {
            StartCoroutine(OpenSceneCo());
        }

        public void OpenSceneAdditive()
        {
            SceneManager.LoadScene(m_SceneString, LoadSceneMode.Additive);
        }
        

        public IEnumerator CancleCo(float i)
        {
            yield return new WaitForSeconds(i);
            this.gameObject.SetActive(false);

        }

        public IEnumerator OpenSceneCo()
        {
            yield return new WaitForSeconds(0.2f);
            SceneManager.LoadScene(m_SceneString);

        }

        public void SceneAfterSec(int i)
        {
            StartCoroutine(SceneOnAfterSec(i));
        }
        public IEnumerator SceneOnAfterSec(int i)
        {
            yield return new WaitForSeconds(i);
            SceneManager.LoadScene(m_SceneString);
        }

    }
}