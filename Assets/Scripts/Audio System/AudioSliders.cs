using UnityEngine;
using UnityEngine.UI;

public class AudioSliders : MonoBehaviour
{
    #region Sliders

    [SerializeField]    private Slider _masterSlider = null;
    [SerializeField]    private Slider _effectsSlider = null;

    #endregion

    #region Shorthand Const Definitions

    private const AudioSourceManager.AudioChannel MASTER = AudioSourceManager.AudioChannel.Master;
    private const AudioSourceManager.AudioChannel EFFECTS = AudioSourceManager.AudioChannel.Effects;

    #endregion

    void Awake()
    {
        SetSliderValues();

        if (_masterSlider != null)
            _masterSlider.onValueChanged.AddListener(delegate 
            { valueChanged(MASTER); }
            );

        if (_effectsSlider != null)
            _effectsSlider.onValueChanged.AddListener(delegate
            { valueChanged(EFFECTS); }
            );
    }

    void OnEnable()
    {
        SetSliderValues();
    }

    private void SetSliderValues()
    {
        AudioSourceManager manager = AudioSourceManager.instance;

        if (_masterSlider != null)
            _masterSlider.normalizedValue = manager.GetChannelValueUnscaled(MASTER);

        if (_effectsSlider != null)
            _effectsSlider.normalizedValue = manager.GetChannelValueUnscaled(EFFECTS);
    }

    private void valueChanged(AudioSourceManager.AudioChannel channel)
    {
        AudioSourceManager manager = AudioSourceManager.instance;

        if (manager != null)
        {
            switch (channel)
            {
                case MASTER:
                    {
                        if (_masterSlider != null)
                            manager.SetChannelValue(MASTER, _masterSlider.normalizedValue);
                        break;
                    }

                case EFFECTS:
                    {
                        if (_effectsSlider != null)
                            manager.SetChannelValue(EFFECTS, _effectsSlider.normalizedValue);
                        break;
                    }

                default:
                    break;
            }
        }
    }
}
