using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatus : NetworkBehaviour
{
    private int hp = 100;
    private int maxHp = 100;

    public int HP { get { return hp; } }
    public int MaxHP { get {  return maxHp; } }


}
