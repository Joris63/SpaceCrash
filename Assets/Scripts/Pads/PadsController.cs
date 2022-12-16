using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PadsController : MonoBehaviour
{
    public int padsActive = 2;
    public float activationDelay = 0f;
    public float maxLifetime = 10f;
    public float transitionTime = .5f;
    public PadEffect[] availableEffects;

    private Pad[] pads;
    private List<Pad> deactivatedPads = new List<Pad>();
    private List<Pad> activatedPads = new List<Pad>();

    private bool currentlyActive = false;

    private void Awake()
    {
        pads = GetComponentsInChildren<Pad>();

        deactivatedPads = pads.ToList();
    }

    private void Update()
    {
        if (!currentlyActive)
        {
            StartCoroutine(GetReadyForActivation());
        }
    }

    private IEnumerator GetReadyForActivation()
    {
        currentlyActive = true;

        yield return new WaitForSeconds(activationDelay + transitionTime);

        StartCoroutine(ActivatePads());
    }

    private IEnumerator ActivatePads()
    {
        // Reset pads just in case
        ResetPads();

        // Find pads to activate
        for (int i = 0; i < padsActive; i++)
        {
            int index = Random.Range(0, deactivatedPads.Count);

            activatedPads.Add(deactivatedPads[index]);
            deactivatedPads.RemoveAt(index);
        }

        // Give activated pads random effects
        foreach (Pad pad in activatedPads)
        {
            pad.Activate(availableEffects[Random.Range(0, availableEffects.Length)]);
        }

        yield return new WaitForSeconds(maxLifetime);

        currentlyActive = false;
        ResetPads();
    }

    private void ResetPads()
    {
        deactivatedPads = pads.ToList();
        activatedPads.Clear();

        foreach (Pad pad in pads)
        {
            pad.Deactivate();
        }
    }
}
