using UnityEngine;
using UnityEngine.UI; // Import the UI namespace

public class QuitButton : MonoBehaviour
{
    [SerializeField] private Button m_quitButton; // Reference to the button in the Inspector

    private void Start()
    {
        // Add an onClick listener to the button
        m_quitButton.onClick.AddListener(Quit);
    }

    private void Quit()
    {
        // Close the application
        Application.Quit();
    }
}