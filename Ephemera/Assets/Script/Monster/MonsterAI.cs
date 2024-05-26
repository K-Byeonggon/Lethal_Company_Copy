using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAI : NetworkBehaviour
{
    protected bool beWatched = false;
    protected GameObject watchedBy;
    
    public bool BeWatched { get { return beWatched; } set {  beWatched = value; } }
    public GameObject WatchedBy { get {  return watchedBy; } set {  watchedBy = value; } }

}
