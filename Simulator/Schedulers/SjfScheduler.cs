#region FileInfo

// Copyright (c) 2022 Wang Qirui. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
// This file is part of Project Simulator.
// File Name   : SjfScheduler.cs
// Author      : Qirui Wang
// Created at  : 2022/03/25 6:26
// Description :

#endregion

namespace Simulator.Schedulers;

public class SjfScheduler : IScheduler
{
    private readonly PriorityQueue<int, int> _shortestJobFirstQueue = new();
    private Os _os;

    Os IScheduler.Os
    {
        get => _os;
        set => _os = value;
    }

    public void OnProcessReady(int pid)
    {
        var process = _os.GetProcess(pid);
        var burstTime = process.BurstTime;
        _shortestJobFirstQueue.Enqueue(pid, burstTime);
    }

    public void SwitchProcess()
    {
        if (_shortestJobFirstQueue.TryDequeue(out var pid, out _))
            _os.SwitchProcess(pid);
        else
            _os.Stop();
    }
}