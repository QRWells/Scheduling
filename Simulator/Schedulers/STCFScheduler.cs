#region FileInfo

// Copyright (c) 2022 Wang Qirui. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
// This file is part of Project Simulator.
// File Name   : StcfScheduler.cs
// Author      : Qirui Wang
// Created at  : 2022/03/25 6:27
// Description :

#endregion

namespace Simulator.Schedulers;

public sealed class STCFScheduler : IScheduler
{
    private readonly PriorityQueue<int, int> _shortestJobFirstQueue = new();
    private Os? _os;

    Os? IScheduler.Os
    {
        get => _os;
        set => _os = value;
    }

    public void OnProcessReady(int pid)
    {
        var process = _os!.GetProcess(pid);
        var burstTime = process.RemainingTime;
        _shortestJobFirstQueue.Enqueue(pid, burstTime);
    }

    public void SwitchProcess()
    {
        if (_shortestJobFirstQueue.TryDequeue(out var pid, out _))
            _os!.SwitchProcess(pid);
        else
            _os!.SwitchProcess(-1);
    }

    public void OnProcessBurst(int pid)
    {
        var remainingTime = _os!.GetProcess(pid).RemainingTime;

        // Current process is shorter than the shortest job in the queue.
        if (!_shortestJobFirstQueue.TryPeek(out _, out var remain)
            || remain > remainingTime)
            return;

        SwitchProcess();
        _shortestJobFirstQueue.Enqueue(pid, remainingTime);
    }
}