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
    private readonly Dictionary<ulong, Process> _processes = new();
    private readonly bool _running;
    private readonly IScheduler _scheduler;
    private uint _core = 1;
    private ulong _currentPid;
    private ulong _lastPid;

    public Os(IScheduler scheduler)
    {
        _scheduler = scheduler;
        _running = true;
    }

    public ulong Clock { get; private set; }

    public void Run()
    {
        while (!_running)
            Tick();
    }

    public void Step(ulong time = 1)
    {
        for (var i = 0ul; i < time; ++i)
        {
            if (!_running) break;
            Tick();
        }
    }

    public void AddProcess(ref Process process)
    {
        var pid = _lastPid++;
        process.ProcessId = pid;
        _processes.Add(pid, process);
    }

    public Process GetProcess(ulong pid)
    {
        return _processes[pid];
    }

    public void SwitchProcess(ulong pid)
    {
        if (_processes.ContainsKey(pid))
            _currentPid = pid;
        else
            throw new ArgumentException("pid not exist.", nameof(pid));
    }

    public Process CurrentProcess()
    {
        return _processes[_currentPid];
    }

    public void CompleteProcess(ulong pid)
    {
        throw new NotImplementedException();
    }

    private void Tick()
    {
        ++Clock;
        _scheduler.OnTick(this);

        // clean up finished processes
        foreach (var process in
                 _processes
                     .Where(process => process.Value.State == ProcessState.Terminated)
                     .ToList())
            _processes.Remove(process.Key);
    }

    public void AwaitProcess(ulong pid, ulong duration)
    {
        throw new NotImplementedException();
    }

    public bool IsProcessRunning(ulong pid)
    {
        return pid == _currentPid;
    }
}