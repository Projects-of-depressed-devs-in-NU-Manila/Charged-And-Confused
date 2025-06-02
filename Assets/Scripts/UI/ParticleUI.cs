using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ParticleUI : MonoBehaviour
{
    public TMP_InputField magnitude;
    public Button destroy;
    private Particle targetParticle;

    public void Bind(Particle particle)
    {
        targetParticle = particle;
        magnitude.text = particle.charge.ToString();
        magnitude.onValueChanged.AddListener(OnMagnitudeChanged);
        destroy.onClick.AddListener(() =>
        {
            ParticleManager.Instance.RemoveParticle(targetParticle);
            Destroy(targetParticle.gameObject);
            Destroy(gameObject);
        });
    }

    private void OnMagnitudeChanged(string value)
    {
        if (targetParticle == null) return;

        if (float.TryParse(value, out float parsedCharged))
        {
            targetParticle.charge = parsedCharged;
        }
    }
}
