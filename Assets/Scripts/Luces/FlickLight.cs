using UnityEngine;

public class FlickLight : MonoBehaviour
{
    
[SerializeField] Light myLight;

float interval = 1;
float timer;

void Update()
{
    timer += Time.deltaTime;
    if (timer > interval)
    {
        myLight.enabled = !myLight.enabled;
        interval = Random.Range(0f, 1f);
        timer = 0;
    }
}

}
