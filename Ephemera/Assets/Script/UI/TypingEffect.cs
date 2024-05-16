using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TypingEffect : MonoBehaviour
{
    [SerializeField]
    private float startTime = 0.5f;
    [SerializeField]
    private float typingTime = 0.1f;

    public TextMeshProUGUI textUI;
    public TextMeshProUGUI loadingUI;



    private string date = "21/03/1989";
    private string time = "13 : 25";
    private string osString = "The Star Terminal OS Personal Computer";
    private string osVersion = "1.02";

    private string m_text;
    

    private char[] loadingChar = { '-','/','|','\\'};
    // Start is called before the first frame update
    void Start()
    {
        m_text = $"Date is {date}\n" +
                 $"Time is {time}\n\n\n" +
                 $"{osString}\n" +
                 $"Version {osVersion}\n";

        StartCoroutine(_typing());
    }

    IEnumerator _typing()
    {
        int count = 0;
        yield return new WaitForSeconds(startTime);

        // 코루틴 시작 후 첫 번째로 loadingUI가 null인지 확인합니다.
        if (loadingUI == null)
        {
            Debug.LogError("loadingUI is not assigned!");
            yield break; // 만약 loadingUI가 null이면 코루틴 종료
        }

        for (int i = 0; i < m_text.Length; i++)
        {
            textUI.text = m_text.Substring(0, i);

            if (count >= loadingChar.Count())
                count = 0;
            loadingUI.text = loadingChar[count].ToString();
            count++;
            yield return new WaitForSeconds(typingTime);
        }
        loadingUI.text = "";
    }
}
