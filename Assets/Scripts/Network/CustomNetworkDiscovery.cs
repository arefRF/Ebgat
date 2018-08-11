﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class CustomNetworkDiscovery : NetworkDiscovery {

    public Action<string, string> action;

    void Awake()
    {
        Initialize();
    }

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        if(action != null){
            action(fromAddress, data);
        }
    }
}
