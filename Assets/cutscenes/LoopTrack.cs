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
    bool firstFrame = false;
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if(firstFrame) return;

        
    }
}
