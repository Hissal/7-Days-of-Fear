using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
public class VidPlayer : MonoBehaviour
{
    [SerializeField] private string videoFileName;
    [SerializeField] private GameObject videoImage;

    public void PlayVideo()
    {

        VideoPlayer videoPlayer = gameObject.GetComponent<VideoPlayer>();

        if (videoPlayer)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            videoImage.SetActive(true);
            string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
            Debug.Log("Playing video: " + videoPath);
            videoPlayer.url = videoPath;
            videoPlayer.Play();
            StartCoroutine(WaitForVideoToPlay(videoPlayer));
        }
    }

    private IEnumerator WaitForVideoToPlay(VideoPlayer videoPlayer)
    {
        while (videoPlayer.isPlaying)
        {
            print("video player playing");
            yield return null;
        }
        VideoEnded();
    }

    private void VideoEnded()
    {
        print("Video Ended");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        videoImage.SetActive(false);
    }

}
