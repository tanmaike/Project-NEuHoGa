using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageActivator : MonoBehaviour
{
    public GameObject imageToActivate;
    private static GameObject currentImage = null;

    // This method is called when the button is clicked.
    public void ActivateMyImage()
    {
        // Check if the imageToActivate object is assigned to prevent errors.
        if (imageToActivate != null)
        {
            //deactivate the current image
            if (currentImage != null)
            {
                currentImage.SetActive(false);
            }
            // Activate the GameObject that holds the image.
            imageToActivate.SetActive(true);
            //set the image that was activated to the current one
            currentImage = imageToActivate;

        }
    }

    public void clear()
    {
        currentImage.SetActive(false);
        currentImage = null;
    }
}