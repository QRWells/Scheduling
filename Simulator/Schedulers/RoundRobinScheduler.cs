#region FileInfo

// Copyright (c) 2022 Wang Qirui. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
// This file is part of Project Simulator.
// File Name   : RoundRobinScheduler.cs
// Author      : Qirui Wang
// Created at  : 2022/03/25 6:28
// Description :

#endregion

namespace Simulator.Schedulers;

public sealed class RoundRobinScheduler : IScheduler
{
    private readonly Queue<int> _readyQueue = new();
    private readonly int _timeSlice;
    private readonly Dictionary<int, int> _usedTimeSlice = new();
    private Os _os;

    public RoundRobinScheduler(int timeSlice = 10)
    {
        _timeSlice = timeSlice - 1;
    }

    Os IScheduler.Os
    {
        get => _os;
        set => _os = value;
    }

    public void OnProcessReady(int pid)
    {
        _readyQueue.Enqueue(pid);
    }

    public void SwitchProcess()
    {
        if (_readyQueue.TryDequeue(out var pid))
            _os.SwitchProcess(pid);
        else
            _os.SwitchProcess(-1);
    }

    public void OnProcessBurst(int pid)
    {
        var usedTimeSlice = _usedTimeSlice.GetValueOrDefault(pid, 0);
        if (usedTimeSlice >= _timeSlice && _os.IsProcessRunning(pid))
        {
            _readyQueue.Enqueue(pid);
            _usedTimeSlice[pid] = 0;
            SwitchProcess();
        }
        else
        {
            _usedTimeSlice[pid] = usedTimeSlice + _os.Interval;
        }
    }
}