using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialData
{
    public TutorialConfig config;
    public int step;
    public bool done;

    public TutorialData(TutorialConfig tutorialConfig)
    {
        this.config = tutorialConfig;
        step = 0;
    }
    public TutorialStep CurStep()
    {
        if(step >= config.steps.Count) return null;
        return config.steps[step];
    }
}
