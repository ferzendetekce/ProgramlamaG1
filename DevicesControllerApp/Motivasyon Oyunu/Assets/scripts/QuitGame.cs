using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void Quit()
    {
        // Editörde çalýþmaz ama build'de çalýþýr
        Application.Quit();

        // Editörde test etmek için (isteðe baðlý)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
