using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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



    private string date = "21/03/2134";
    private string time = "8 : 00";
    private string osString = "The Star Terminal OS Personal Computer";
    private string osVersion = "1.02";

    private string m_bootText;
    private string m_resetText;
    

    private char[] loadingChar = { '-','/','|','\\'};
    // Start is called before the first frame update
    void Start()
    {
        m_bootText = $"Date is {date}\n" +
                 $"Time is {time}\n\n\n" +
                 $"{osString}\n" +
                 $"Version {osVersion}\n";

        m_resetText = $"1\n" +
                      $"2\n" +
                      $"3\n" +
                      $"4\n" +
                      $"5\n" +
                      $"6\n" +
                      $"7\n" +
                      $"8\n" +
                      $"9\n" +
                      $"1\n" +
                      $"2\n" +
                      $"3\n" +
                      $"4\n" +
                      $"5\n" +
                      $"6\n" +
                      $"7\n" +
                      $"8\n" +
                      $"9\n" +
                      $"0\n" +
                      $"1\n" +
                      $"2\n" +
                      $"3\n" +
                      $"4\n" +
                      $"5\n" +
                      $"6\n" +
                      $"6\n" +
                      $"6\n" +
                      $"6\n" +
                      $"6\n" +
                      $"6\n" +
                      $"6\n" +
                      $"6\n" +
                      $"6\n" +
                      $"6\n" +
                      $"6\n" +
                      $"6\n" +
                      $"6\n" +
                      $"6\n" +
                      $"6\n" +
                      $"6\n" +
                      $"6\n" +
                      $"6\n" +
                      $"6\n" +
                      $"6\n" +
                      $"6\n" +
                      $"6\n" +
                      $"6\n" +
                      $"6\n" +
                      $"6\n" +
                      $"6\n" +
                      $"6\n" +
                      $"6\n" +
                      $"6\n" +
                      $"7\n" +
                      $"8\n" +
                      $"9\n" +
                      $"0\n";

        StartCoroutine(TypingChar());
    }

    IEnumerator TypingChar()
    {
        int count = 0;
        yield return new WaitForSeconds(startTime);

        // 코루틴 시작 후 첫 번째로 loadingUI가 null인지 확인합니다.
        if (loadingUI == null)
        {
            Debug.LogError("loadingUI is not assigned!");
            yield break; // 만약 loadingUI가 null이면 코루틴 종료
        }

        for (int i = 0; i < m_bootText.Length; i++)
        {
            textUI.text = m_bootText.Substring(0, i);

            if (count >= loadingChar.Count())
                count = 0;
            loadingUI.text = loadingChar[count].ToString();
            count++;
            yield return new WaitForSeconds(typingTime);
        }
        loadingUI.text = "";

        yield return new WaitForSeconds(1f);

        StartCoroutine(TypingLine());
    }

    IEnumerator TypingLine()
    {
        int count = 0;
        yield return new WaitForSeconds(startTime);

        // 코루틴 시작 후 첫 번째로 loadingUI가 null인지 확인합니다.
        if (loadingUI == null)
        {
            Debug.LogError("loadingUI is not assigned!");
            yield break; // 만약 loadingUI가 null이면 코루틴 종료
        }

        var resetTexts = m_resetText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        foreach (var resetText in resetTexts)
        {
            Debug.Log(resetText);
        }

        for (int i = 0; i < resetTexts.Length; i++)
        {
            StringBuilder sb = new StringBuilder();
            int startIndex = (i - 20) >= 0 ? i - 20 : 0;

            for (int k = startIndex; k <= i; k++)
            {
                sb.AppendLine(resetTexts[k]);
            }
            textUI.text = sb.ToString();

            if (count >= loadingChar.Count())
                count = 0;
            loadingUI.text = loadingChar[count].ToString();
            count++;
            yield return new WaitForSeconds(typingTime);
        }
        yield return new WaitForSeconds(1f);

        textUI.text = "";
        loadingUI.text = "";

        this.gameObject.SetActive(false);
        UIController.Instance.SetActivateUI(typeof(UI_Game));
    }
}
