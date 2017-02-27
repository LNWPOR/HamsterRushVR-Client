﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public string URL = "http://localhost:8000";
    public PlayerData playerData;

    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("GameManager").AddComponent<GameManager>();

            }
            return _instance;
        }
    }

    void Awake()
    {
        playerData = new PlayerData("58b46ac0dafcde22a4bce7bd", "LNWPOR", 0, 0);
        DontDestroyOnLoad(gameObject);
    }
}
