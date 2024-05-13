using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : SingleTon<ScenesManager>
{
    public void LoadScene(string sceneName)
    {
        /*if(GameManager.Instance.UserData != null)
            GameManager.Instance.UserData.Save();
        SceneManager.LoadScene(sceneName);*/
        //StartCoroutine(LoadAsyncScene(sceneName));
    }

    IEnumerator LoadAsyncScene(string sceneName)
    {
        //비동기적으로 씬을 로드
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        //로드가 완료될 때까지 대기
        while (!asyncLoad.isDone)
        {
            Debug.Log("Loading progress: " + (asyncLoad.progress * 100) + "%");

            yield return null;
        }
        PoolManager.Instance.SceneRig = GameObject.Find("SceneInstaller");
    }
}
