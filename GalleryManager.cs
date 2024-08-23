using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GalleryManager : MonoBehaviour
{
    public Text notificationText; // Reference to the UI text component for notifications
    public float notificationDuration = 3.0f; // Duration to display the notification

    // Notify the player where the photo was saved
    public void NotifyPhotoSaved(string filePath)
    {
        StartCoroutine(ShowNotification("Photo saved to: " + filePath));
    }

    // Notify the player about an error
    public void NotifyPhotoError(string errorMessage)
    {
        StartCoroutine(ShowNotification(errorMessage));
    }

    // Coroutine to show the notification for a certain duration
    private IEnumerator ShowNotification(string message)
    {
        notificationText.text = message;
        notificationText.gameObject.SetActive(true);

        yield return new WaitForSeconds(notificationDuration);

        notificationText.gameObject.SetActive(false);
    }
}
