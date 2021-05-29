using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBrain : Singleton<GameBrain> {
    public float masterVolume = -6.0f;
    public float musicVolume = 0.0f;
    public float sfxVolume = 0.0f;
    public int currentFunnyPoints = 0;
}
