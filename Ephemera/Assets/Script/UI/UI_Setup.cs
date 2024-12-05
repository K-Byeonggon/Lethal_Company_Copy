using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Setup : MonoBehaviour
{
    [SerializeField]
    TypingEffect typingEffect;
    public void StartSystem()
    {
        typingEffect.StartSystem();
    }
}
