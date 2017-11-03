using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarManager : MonoBehaviour {

    public GameObject prefab;

    public static HealthBarManager instance;

    List<HealthBar> hbs = new List<HealthBar>();

    // Use this for initialization
    void Awake () {
        instance = this;	
	}

    public void AddHealthBar(Health health)
    {
        GameObject go = Instantiate(prefab);
        go.transform.parent = transform;
        HealthBar hb = go.GetComponent<HealthBar>();
        hb.health = health;
        hb.name = "HB_" + health.gameObject.name;

        // add to our global list
        hbs.Add(hb);
    }

    public void RemoveHealthBar(HealthBar hb)
    {
        hbs.Remove(hb);
    }

    public void Update()
    {
        // calculate the health bar z of every child
        foreach (HealthBar hb in hbs)
            hb.screenZ = Camera.main.WorldToScreenPoint(hb.transform.position).z;

        // sort  them in order of screen depth
        hbs.Sort(delegate (HealthBar a, HealthBar b) {
            return a.screenZ.CompareTo(b.screenZ);
        });

        // sort them in the hierarchy based on sorted list
        foreach (HealthBar hb in hbs)
            hb.transform.SetAsFirstSibling();
    }
}
