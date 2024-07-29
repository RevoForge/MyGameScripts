using UniStorm;
using UnityEngine;

public class Thunderstormloader : MonoBehaviour
{
    public UniStormManager uniStorm;
    public WeatherType weatherType;
    private float volume = 0.25f;
    private float oldVolume;
    private float delay = 0.01f;


    void Start()
    {
        Invoke(nameof(Thunderstorm), delay);
        oldVolume = volume;
    }

    void Thunderstorm()
    {
        UniStormManager.Instance.ChangeWeatherInstantly(weatherType);
        UniStormManager.Instance.SetWeatherVolume(0.25f);
    }

    void Update()
    {
        if (volume != oldVolume)
        {
            oldVolume = volume;
            UniStormManager.Instance.SetWeatherVolume(volume);
        }
    }

    public void SetVolume(float slidervolume)
    {
        volume = slidervolume;
    }
}
