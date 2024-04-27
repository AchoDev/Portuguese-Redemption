using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

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
    public static bool leave = false;
    bool firstFrame = true;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        base.OnBehaviourPlay(playable, info);
        firstFrame = true;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {

        if(leave) Debug.Log("LEEEEEEEEAAAAAAAAAAVE");

        LoopScript current = (playable.GetGraph().GetResolver() as PlayableDirector).gameObject.GetComponent<LoopScript>();
        if(firstFrame) {
            firstFrame = false;
            current?.StartLoop(0, (float)playable.GetDuration());
        } else {
            current?.Check(info.deltaTime);
        }
    }
}
