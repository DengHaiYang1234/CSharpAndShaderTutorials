using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Clock : MonoBehaviour
{


    public Transform hoursTransform;
    public Transform minTransform;
    public Transform secondsTransform;

    public bool continuous;

    const float degreesPerHour = 30f, degreesPerMinute = 6f, degreesPerSecond = 6f;



    void Awake()
    {
        // hoursTransform.localRotation = Quaternion.Euler(0f,DateTime.Now.Hour * degreesPerHour,0f);
        // minTransform.localRotation = Quaternion.Euler(0f,DateTime.Now.Minute * degreesPerMinute,0f);
        // secondsTransform.localRotation = Quaternion.Euler(0f,DateTime.Now.Second * degreesPerSecond,0f);
    }

    // Use this for initialization
    void Start()
    {

    }

	void Update()
	{
		if(continuous)
			UpdateContinuous();
		else
			UpdateDiscrete();	
	}
	
    // Update is called once per frame
    void UpdateDiscrete()
    {
        var time = DateTime.Now;
        hoursTransform.localRotation = Quaternion.Euler(0f, time.Hour * degreesPerHour, 0f);
        minTransform.localRotation = Quaternion.Euler(0f, time.Minute * degreesPerMinute, 0f);
        secondsTransform.localRotation = Quaternion.Euler(0f, time.Second * degreesPerSecond, 0f);
    }

    void UpdateContinuous()
    {
        var time = DateTime.Now.TimeOfDay;
        hoursTransform.localRotation = Quaternion.Euler(0f, (float)(time.TotalHours * degreesPerHour), 0f);
        minTransform.localRotation = Quaternion.Euler(0f, (float)(time.TotalMinutes * degreesPerMinute), 0f);
        secondsTransform.localRotation = Quaternion.Euler(0f, (float)(time.TotalSeconds * degreesPerSecond), 0f);
    }
}
