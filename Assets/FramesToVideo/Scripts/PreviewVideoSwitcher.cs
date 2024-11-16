using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Video;

public class PreviewVideoSwitcher : MonoBehaviour
{
    private Texture texture;
    private RawImage img;
    private VideoPlayer videoPlayer;

    private int frameCounter = 0;

    public string folderName;
    public string imageSequenceName;
    public int numberOfFrames;

    public GameObject helpTextImageObject;
    public GameObject helpTextObject;

    private string baseName;
    private VideoManager sharedVideoManager;

    void Awake()
    {
        this.sharedVideoManager = VideoManager.getVideoManager();

        this.img = GetComponent<RawImage>();
        this.videoPlayer = GetComponent<VideoPlayer>();

        this.baseName = this.folderName + "/" + this.imageSequenceName;

        EnsureSingleAudioListener();
    }

    void Start()
    {
        changePracticeScreenVideo();

        if (!string.IsNullOrEmpty(baseName))
        {
            texture = (Texture)Resources.Load(baseName, typeof(Texture));
            ConfigureHelpText();
        }
    }

    void Update()
    {
        if (sharedVideoManager == null)
        {
            Debug.LogError("sharedVideoManager is null!");
            return;
        }

        if (sharedVideoManager.shouldChangeVideo)
        {
            Video curtVideo = sharedVideoManager.getCurtVideo();

            if (curtVideo == null)
            {
                Debug.LogError("curtVideo is null!");
                return;
            }

            UpdateVideoData(curtVideo);
            sharedVideoManager.shouldChangeVideo = false;

            StartCoroutine(PlayUsingVideoPlayer());
        }
    }

    private void UpdateVideoData(Video curtVideo)
    {
        this.folderName = curtVideo.folderName;
        this.imageSequenceName = curtVideo.fileName;
        this.numberOfFrames = curtVideo.frameNumber;
        this.frameCounter = 0;

        this.baseName = this.folderName + "/" + this.imageSequenceName;

        if (!string.IsNullOrEmpty(this.baseName))
        {
            texture = (Texture)Resources.Load(baseName, typeof(Texture));
            ConfigureHelpText();
        }
    }

    private void ConfigureHelpText()
    {
        if (helpTextImageObject)
        {
            SpriteRenderer helpTextImage = helpTextImageObject.GetComponent<SpriteRenderer>()
                                            ?? helpTextImageObject.AddComponent<SpriteRenderer>();

            helpTextImage.sortingLayerName = "UI layer";
            helpTextImage.sortingOrder = 3;

            string textImageName = sharedVideoManager?.curtVideo?.imageName ?? "default_image";
            helpTextImage.sprite = (Sprite)Resources.Load(textImageName, typeof(Sprite));
            helpTextImage.transform.localScale = new Vector3(1.1f, 1.1f, 0.0f);
            helpTextImage.transform.localPosition = Vector3.zero;

            if (helpTextObject)
            {
                SpriteRenderer helpTextBG = helpTextObject.GetComponent<SpriteRenderer>();
                string bgName = "VideoCaption/rect_" + sharedVideoManager.curtVideo.color;
                helpTextBG.sprite = (Sprite)Resources.Load(bgName, typeof(Sprite));
            }
        }
    }

    IEnumerator PlayUsingVideoPlayer()
    {
        yield return null;

#if UNITY_EDITOR
        videoPlayer.url = Application.dataPath + "/StreamingAssets/" + folderName + ".mp4";
#elif UNITY_ANDROID
        videoPlayer.url = "jar:file://" + Application.dataPath + "!/assets/" + folderName + ".mp4";
#elif UNITY_IOS
        videoPlayer.url = Application.dataPath + "/Raw/" + folderName + ".mp4";
#endif
        videoPlayer.Prepare();
        videoPlayer.Play();
    }

    public void changePracticeScreenVideo()
    {
        sharedVideoManager?.resetCurtVideo();
        sharedVideoManager.shouldChangeVideo = true;
    }

    private void EnsureSingleAudioListener()
    {
        AudioListener[] listeners = FindObjectsOfType<AudioListener>();
        for (int i = 1; i < listeners.Length; i++)
        {
            listeners[i].enabled = false;
        }
    }
}
