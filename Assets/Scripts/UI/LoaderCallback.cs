using UnityEngine;

public class LoaderCallback1 : MonoBehaviour
{
    private bool isFirstUpdate = true;

    private void Update()
    {
        if (isFirstUpdate)
        {
            isFirstUpdate = false;
        }
    }
}
