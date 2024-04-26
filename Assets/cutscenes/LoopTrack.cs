using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class LoopTrack : PlayableAsset
{
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<LoopBehaviour>.Create(graph);
        return playable;
    }
}

public class LoopBehaviour : PlayableBehaviour
{
    bool firstFrame = true;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        base.OnBehaviourPlay(playable, info);
        firstFrame = true;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        LoopScript current = (playable.GetGraph().GetResolver() as PlayableDirector).gameObject.GetComponent<LoopScript>();
        current?.Check();
        if(!firstFrame) return;
        firstFrame = false;

        // get track start
        


        current?.StartLoop(0, (float)playable.GetDuration());
    }
}
