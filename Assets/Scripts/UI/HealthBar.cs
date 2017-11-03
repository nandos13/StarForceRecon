using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StarForceRecon;

public class HealthBar : MonoBehaviour {

    public Health health;
    public Image image;
    [SerializeField, Range(0.5f, 2f)]
    private float IconScale = 1.5f;
    public float screenZ;
    public bool inWorld = true;

    Rect initialRect;
    // Use this for initialization
    void Start ()
    {
        if (image && image.type != Image.Type.Filled)
        {
            image.type = Image.Type.Filled;
            image.fillMethod = Image.FillMethod.Horizontal;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (health == null)
            return;

        // scale the meter
        float pct = Mathf.Clamp01(health.health / health.maxHealth);
        image.fillAmount = pct * 0.75f;

        if (inWorld)
        {
            transform.position = health.transform.position + Vector3.up * IconScale;
            transform.forward = Camera.main.transform.forward;

            if (pct <= 0)
            {
                HealthBarManager.instance.RemoveHealthBar(this);
                Destroy(gameObject);
            }
        }
        else
        {
            if (health == SquadManager.GetCurrentSquaddie.transform.GetComponent<Health>())
            {
                transform.localScale = Vector3.one * IconScale;
                transform.SetAsLastSibling();
            }
            else
                transform.localScale = Vector3.one;
        }

    }
}
