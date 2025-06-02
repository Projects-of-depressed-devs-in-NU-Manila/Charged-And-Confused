using UnityEngine;

public class QuitScript : MonoBehaviour
{
    public void IWantThisToEnd()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif

        Debug.Log("Left The Simulation");
    }
}
