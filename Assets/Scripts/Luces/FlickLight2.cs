using UnityEngine;

public class FlickLight2 : MonoBehaviour
{
    [SerializeField] Light myLight;
    [SerializeField] float maxWait;
    [SerializeField] float maxFlicker;

    float timer;
    float interval;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > interval)
        {
            ToggleLight();
        }
    }

    void ToggleLight()
    {
        myLight.enabled = !myLight.enabled;
        if (myLight.enabled)
        {
            interval = Random.Range(0, maxWait);
        }
        else
        {
            interval = Random.Range(0, maxFlicker);
        }

        timer = 0;
    }
}
