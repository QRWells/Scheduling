#region FileInfo

// Copyright (c) 2022 Wang Qirui. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
// This file is part of Project Simulator.
// File Name   : OS.cs
// Author      : Qirui Wang
// Created at  : 2022/03/25 6:14
// Description :

#endregion

using Simulator.Schedulers;

namespace Simulator;

public class Os
{
    private bool _running = false;
    private ulong _clock = 0;
    private ulong _interval;
    private readonly Dictionary<ulong, Process> _processes = new();
    private readonly IScheduler _scheduler;

    public Os(IScheduler scheduler, ulong interval = 1)
    {
        _interval = interval;
        _scheduler = scheduler;
    }

    public void Run()
    {
        while (!_running)
        {
            Tick();
        }
    }

    public void AddProcess(ref Process process)
    {
        var pid = GeneratePId();
        process.ProcessId = pid;
        _processes.Add(pid, process);
    }

    private ulong GeneratePId()
    {
        return 0;
    }

    private void Tick()
    {
        _clock += _interval;
        _scheduler.Tick();
    }
}