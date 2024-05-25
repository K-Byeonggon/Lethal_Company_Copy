using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    protected bool beWatched = false;
    protected GameObject watchedBy;
    protected float openDoorDelay;

    public bool BeWatched { get { return beWatched; } set {  beWatched = value; } }
    public GameObject WatchedBy { get {  return watchedBy; } set {  watchedBy = value; } }
    public float OpenDoorDelay { get {  return openDoorDelay; } set {  openDoorDelay = value; } }

}
