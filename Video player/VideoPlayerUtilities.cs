using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public static class VideoPlayerUtilities
{
    /// <summary>
    /// Toggle to a specific videoPlayer, then when it is finished, switch back to the previous player.
    /// </summary>
    /// <param name="idlePlayer"></param>
    /// <param name="playerToPlay"></param>
    /// <returns></returns>
    public static IEnumerator PlayVideoClipOneShot(VideoPlayer playerToPlay, VideoPlayer idlePlayer, Action EndAction = null)
    {
        if (!playerToPlay.isPrepared)
        {
            playerToPlay.Prepare();
            yield return new WaitUntil(() => !playerToPlay.isPrepared);
        }
        playerToPlay.Play();
        yield return new WaitForSeconds(.5f);
        yield return new WaitUntil(() => playerToPlay.isPlaying);
        idlePlayer.GetComponent<Renderer>().enabled = false;
        playerToPlay.GetComponent<Renderer>().enabled = true;
        yield return new WaitUntil(() => !playerToPlay.isPlaying);
        idlePlayer.GetComponent<Renderer>().enabled = true;
        playerToPlay.GetComponent<Renderer>().enabled = false;
        EndAction?.Invoke();
    }
}
