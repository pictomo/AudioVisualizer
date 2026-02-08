using UnityEngine;

class MicAudioSource : MonoBehaviour
{
    [SerializeField] private string m_DeviceName;
    // private const int SAMPLE_RATE = 48000;
    private const int SAMPLE_RATE = 44100;
    private const int RESOLUTION = 1024;
    private AudioSource m_MicAudioSource;

    [SerializeField] private LineRenderer m_LineRenderer;
    private readonly Vector3[] m_Positions = new Vector3[1024];
    [SerializeField, Range(1, 300)] private float m_AmpGain = 300;

    private void Awake()
    {
        m_MicAudioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        string targetDevice = "";

        foreach (var device in Microphone.devices)
        {
            Debug.Log($"Device Name: {device}");
            if (device.Equals(m_DeviceName))
            {
                targetDevice = device;
            }
        }

        Debug.Log($"=== Device Set: {targetDevice} ===");
        MicStart(targetDevice);

        // LineRenderer初期化
        for (int i = 0; i < RESOLUTION; i++)
        {
            var x = 10 * (i / 512f - 1);
            m_Positions[i] = new Vector3(x, 0, 0);
        }

        m_LineRenderer.SetPositions(m_Positions);

        Debug.Log($"Sample rate: {AudioSettings.outputSampleRate}");
    }

    void Update()
    {
        DrawSpectrum();
    }

    private void DrawSpectrum()
    {
        if (!m_MicAudioSource.isPlaying) return;

        float[] spectrum = new float[RESOLUTION];
        m_MicAudioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        for (int i = 0; i < RESOLUTION; i++)
        {
            m_Positions[i].y = spectrum[i] * m_AmpGain;
        }

        m_LineRenderer.SetPositions(m_Positions);
    }

    private void MicStart(string device)
    {
        if (device.Equals("")) return;

        m_MicAudioSource.clip = Microphone.Start(device, true, 1, SAMPLE_RATE);

        //マイクデバイスの準備ができるまで待つ
        while (Microphone.GetPosition("") <= 0) { }

        m_MicAudioSource.Play();
    }
}