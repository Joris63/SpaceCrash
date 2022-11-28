using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pad : MonoBehaviour
{
    public GameObject particlesPrefab;

    private PadEffect currentEffect;
    private PadsController padController;
    private Collider collider;
    private MeshRenderer meshRenderer;
    private ParticleSystem ps;

    [ColorUsage(true, true)]
    private Color inactiveColor;
    private bool activated = false;

    private void Awake()
    {
        padController = GetComponentInParent<PadsController>();
        collider = GetComponent<Collider>();
        meshRenderer = GetComponent<MeshRenderer>();

        inactiveColor = meshRenderer.material.color;
    }

    public void Activate(PadEffect effect)
    {
        currentEffect = effect;

        collider.enabled = true;

        ps = Instantiate(particlesPrefab, transform).GetComponent<ParticleSystem>();

        ParticleSystemRenderer psRenderer = ps.GetComponent<ParticleSystemRenderer>();
        ParticleSystem particleSys = ps.GetComponent<ParticleSystem>();
        ParticleSystem.ShapeModule shape = particleSys.shape;
        Vector3 upDir = (GravityController.main.transform.position - ps.transform.position).normalized;

        shape.rotation = Quaternion.LookRotation(upDir).eulerAngles;

        psRenderer.mesh = GetComponent<MeshFilter>().mesh;
        psRenderer.material = effect.material;

        StartCoroutine(TransitionPad());
    }

    private IEnumerator TransitionPad(bool activating = true)
    {
        float transitionTime = padController.transitionTime;

        float timeElapsed = 0;
        Color startingColor = activating ? inactiveColor : meshRenderer.material.color;
        Color targetColor = activating ? currentEffect.color : inactiveColor;

        while (timeElapsed < transitionTime)
        {
            meshRenderer.material.color = Color.Lerp(startingColor, targetColor, timeElapsed / transitionTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        activated = true;
    }

    public void Deactivate()
    {
        activated = false;

        if (currentEffect)
        {
            currentEffect = null;
            collider.enabled = false;

            if (ps)
                ps.Stop();
        }

        StartCoroutine(TransitionPad(false));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentEffect && activated)
        {
            activated = false;
            StartCoroutine(TransitionPad());
            currentEffect.ApplyEffect(other.attachedRigidbody);
        }
    }
}
