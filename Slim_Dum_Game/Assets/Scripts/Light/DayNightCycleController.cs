using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


public interface DayNightInterface
{
    void GetComponent();
    void SetParameter(float time);
}
[ExecuteInEditMode]
public class DayNightCycleController : MonoBehaviour
{
    [Range(0,1)]
    public float time;
    public DayNightInterface[] setters;
    public bool day;

    private void OnEnable()
    {
        time = 0;
        day = true;
        GetSetters();
    }

    private void GetSetters()
    {
        setters = GetComponentsInChildren<DayNightInterface>();
        foreach (var setter in setters)
        {
            setter.GetComponent();
        }
    }

    private void Update()
    {
        if (setters.Length>0)
        {
            foreach (var setter in setters)
            {
                setter.SetParameter(time);
            }
        }
        if (time>1f)
            day = false;
        if (time < 0f)
            day = false;

        if (day)
        {
            time = Mathf.Lerp(time, 1.1f, Time.deltaTime * 0.0030f);
        }
        else if (!day)
        {
            time = Mathf.Lerp(time, -0.1f, Time.deltaTime * 0.0030f);
        }
    }
}
