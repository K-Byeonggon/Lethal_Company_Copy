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
    private float typingTime = 0.3f;

    public TextMeshProUGUI textUI;
    public TextMeshProUGUI loadingUI;



    private string date = "21/03/2134";
    private string time = "8 : 00";
    private string osString = "The Star Terminal OS Personal Computer";
    private string osVersion = "1.02";

    private string m_bootText;
    private string m_resetText = @"
============================================
||         SYSTEM INITIALIZING...          ||
============================================
||                                         ||
||    LOADING CORE MODULES...              ||
||                                         ||
||    AUTHENTICATION REQUIRED...           ||
||                                         ||
||    ACCESS GRANTED.                      ||
||                                         ||
============================================
||                                         ||
||    RETRIEVING MISSION DATA...           ||
||                                         ||
||    CONNECTION SECURED.                  ||
||                                         ||
||    ENCRYPTING COMMUNICATION CHANNEL...  ||
||                                         ||
||    MISSION OBJECTIVE:                   ||
||    >> INFILTRATE THE ENEMY BASE         ||
||    >> EXTRACT THE TARGET                ||
||    >> RETURN TO SAFEHOUSE               ||
||                                         ||
============================================
||                                         ||
||    INITIALIZING SYSTEM CHECKS...        ||
||                                         ||
||    LOADING SATELLITE IMAGERY...         ||
||                                         ||
||    SCANNING FOR HOSTILES...             ||
||                                         ||
||    ALL SYSTEMS GO.                      ||
||                                         ||
============================================
";


    private char[] loadingChar = { '-','/','|','\\'};
    // Start is called before the first frame update
    public void StartSystem()
    {
        m_bootText = $"Date is {date}\n" +
                 $"Time is {time}\n\n\n" +
                 $"{osString}\n" +
                 $"Version {osVersion}\n";

        StartCoroutine(TypingChar());
    }

    IEnumerator TypingChar()
    {
        int count = 0;
        yield return new WaitForSeconds(startTime);

        // �ڷ�ƾ ���� �� ù ��°�� loadingUI�� null���� Ȯ���մϴ�.
        if (loadingUI == null)
        {
            Debug.LogError("loadingUI is not assigned!");
            yield break; // ���� loadingUI�� null�̸� �ڷ�ƾ ����
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

        // �ڷ�ƾ ���� �� ù ��°�� loadingUI�� null���� Ȯ���մϴ�.
        if (loadingUI == null)
        {
            Debug.LogError("loadingUI is not assigned!");
            yield break; // ���� loadingUI�� null�̸� �ڷ�ƾ ����
        }

        var resetTexts = m_resetText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        /*foreach (var resetText in resetTexts)
        {
            Debug.Log(resetText);
        }*/

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
