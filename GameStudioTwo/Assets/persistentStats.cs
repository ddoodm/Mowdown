﻿using UnityEngine;
using System.Collections;

public class persistentStats : MonoBehaviour {

	public Color playerColor;
	public Equipment[] playerItems;
    public bool spike;

	// Use this for initialization
	void Start () {
		for (int i = 0; i < 5; i++)
		{
			playerItems[i] = Equipment.EMPTY;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
