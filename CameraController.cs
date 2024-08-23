using System.IO;
using UnityEngine;
using SFB;
using System.Threading; // Namespace for StandaloneFileBrowser

public class CameraController : MonoBehaviour
{
    public Camera playerCamera; // Reference to the main camera
    public GalleryManager galleryManager; // Reference to the GalleryManager
    public GameObject CameraEffects;

    public float normalFOV = 60f; // Field of view for normal gameplay
    public float photoFOV = 30f; // Field of view for photo mode

    private bool isPhotoMode = false;


    [SerializeField] private AudioSource CameraClick;

    void Start()
    {
        // Initialize the camera field of view
        playerCamera.fieldOfView = normalFOV;
        CameraEffects.SetActive(false);
        ////Cursor.lockState = CursorLockMode.Locked;
        ////Cursor.visible = false;
    }

    void Update()
    {
        // Toggle photo mode with the "C" key
        if (Input.GetKeyDown(KeyCode.C))
        {
            TogglePhotoMode();
        }
        
        if (isPhotoMode)
        {
            CameraEffects.SetActive(true);
        }
        else if (!isPhotoMode)
        {
            CameraEffects.SetActive(false);
        }

        // Take a photo with the left mouse button in photo mode
        if (isPhotoMode && Input.GetMouseButtonDown(0))
        {
            CapturePhoto();
            CameraClick.Play();
            
        }
    }

    // Toggles between normal mode and photo mode
    void TogglePhotoMode()
    {
        isPhotoMode = !isPhotoMode;

        // Adjust the camera's field of view based on the mode
        playerCamera.fieldOfView = isPhotoMode ? photoFOV : normalFOV;

        // Lock or unlock the cursor based on the mode
        //Cursor.lockState = isPhotoMode ? CursorLockMode.None : CursorLockMode.Locked;
        //Cursor.visible = isPhotoMode;
    }

    // Captures a photo using the player camera
    void CapturePhoto()
    {
        try
        {
            // Set up render texture
            RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
            playerCamera.targetTexture = renderTexture;
            Texture2D photo = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

            // Render photo
            playerCamera.Render();
            RenderTexture.active = renderTexture;
            photo.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            photo.Apply();

            // Open a save file dialog to choose the save location
            string path = StandaloneFileBrowser.SaveFilePanel("Save Photo", "", "Photo.png", "png");
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    // Save photo to the selected path
                    File.WriteAllBytes(path, photo.EncodeToPNG());

                    // Notify the player where the photo was saved
                    galleryManager.NotifyPhotoSaved(path);

                    Debug.Log("Photo captured and saved to: " + path);
                }
                catch (IOException ex)
                {
                    Debug.LogError("Failed to save photo: " + ex.Message);
                    galleryManager.NotifyPhotoError("Failed to save photo. Please check permissions and disk space.");
                }
            }
            else
            {
                Debug.Log("Photo saving canceled by the user.");
            }

            // Cleanup
            playerCamera.targetTexture = null;
            RenderTexture.active = null;
            Destroy(renderTexture);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error capturing photo: " + ex.Message);
            galleryManager.NotifyPhotoError("An error occurred while capturing the photo.");
        }
    }
}
