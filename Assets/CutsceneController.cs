using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CutsceneController : MonoBehaviour
{
    public PlayableDirector pD;
    public TimelineAsset IntroCutscene;
    public TimelineAsset LevelSwapCutscene1;
    public TimelineAsset LevelSwapCutscene2;
    public TimelineAsset OutroCutscene;


    private void Start() {
        //swap out timeline asset and start playing it
        if (pD != null) {
            switch (OverworldManager.Instance.nextCutscene) {
                case 0:
                    pD.playableAsset = IntroCutscene; break;
                case 1:
                    pD.playableAsset = LevelSwapCutscene1; break;
                case 2:
                    pD.playableAsset = LevelSwapCutscene2; break;
                case 3:
                    pD.playableAsset = OutroCutscene; break;
                default:
                    pD.playableAsset = null; break;
            }
        }
        else {
            Debug.LogWarning("No Playable Director");
        }

        pD.Play();
    }

}
