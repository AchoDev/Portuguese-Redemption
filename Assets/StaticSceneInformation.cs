using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum StoryProgression {
    Prologue,
    DayOne,
    DefeatRamza,
    DayOneAfternoon,
    DayOneNight,
}

public class StaticSceneInformation
{

    // public static StoryProgression currentProgression = StoryProgression.Prologue;
    public static StoryProgression currentProgression = StoryProgression.DayOneNight;
}
