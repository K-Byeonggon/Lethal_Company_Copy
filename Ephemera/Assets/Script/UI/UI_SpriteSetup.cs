using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SpriteSetup : MonoBehaviour
{
    //Dictionary<>
    [SerializeField]
    List<RectTransform> rects;
    

    public void OnSetup()
    {
        Dictionary<RectTransform, float> uiSpriteWidthDic = new Dictionary<RectTransform, float>();
        rects.ForEach(rect => { uiSpriteWidthDic.Add(rect, (rect).rect.width); rect.sizeDelta = new Vector2(0, rect.rect.height); });
        StartCoroutine(UISetUp(uiSpriteWidthDic));
    }
    IEnumerator UISetUp(Dictionary<RectTransform, float> uiSpriteWidthDic)
    {
        float widthRatio = 0;
        while (true)
        {
            foreach (var item in uiSpriteWidthDic)
            {
                widthRatio = Mathf.Lerp(widthRatio, 100, 0.2f);
                item.Key.sizeDelta = new Vector2(item.Value / 100 * widthRatio, item.Key.rect.height);
            }
            yield return YieldInstructionCache.WaitForSeconds(0.1f);
            Debug.Log(widthRatio);

            if (widthRatio > 99)
            {
                foreach (var item in uiSpriteWidthDic)
                {
                    item.Key.sizeDelta = new Vector2(item.Value, item.Key.rect.height);
                }
                Debug.Log("End UI SetUp");
                break;
            }
        }
    }
}
