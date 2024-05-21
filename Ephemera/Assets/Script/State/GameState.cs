using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameState
{
    protected GameManager gameManager;

    protected GameState(GameManager manager)
    {
        gameManager = manager;
    }

    public abstract void OnStateEnter();
    public abstract void OnStateExit();
}
