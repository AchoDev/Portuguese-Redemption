using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum StoryProgression {
    Prologue,
    DayOne,
    DefeatRamza,
    DayOneAfternoon,
}

public class StaticSceneInformation
{

    public static StoryProgression currentProgression = StoryProgression.Prologue;
}
