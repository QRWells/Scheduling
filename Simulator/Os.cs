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
using Simulator.Timer;

namespace Simulator;

public class Os
{
    /// <summary>
    ///     processes in the OS.
    /// </summary>
    private readonly SortedList<int, Process> _processes = new();

    /// <summary>
    ///     waiting queue of the OS.
    /// </summary>
    private readonly HashedWheel<int> _waitList = new();

    /// <summary>
    ///     The number of cores in the OS.
    ///     TODO: add multi-core support
    /// </summary>
    private uint _core = 1;

    /// <summary>
    ///     the pid of the current running process.
    /// </summary>
    private int _currentPid;

    /// <summary>
    ///     Indicates whether the OS is running.
    /// </summary>
    private bool _running;

    /// <summary>
    ///     The scheduler used in the OS.
    ///     TODO: add multi-scheduler support
    /// </summary>
    private IScheduler _scheduler = new FcfsScheduler();

    public Os()
    {
        _running = true;
    }

    /// <summary>
    ///     The interval for each step.
    /// </summary>
    public int Interval { get; set; } = 1;

    /// <summary>
    ///     Current clock of the OS.
    /// </summary>
    public int Clock { get; private set; } = -1;

    public void SetSchedule(IScheduler scheduler)
    {
        _scheduler = scheduler;
        _scheduler.Os = this;
    }

    public void Run()
    {
        while (_running)
            Tick();
    }

    public void Step(int time = 1)
    {
        if (time <= 0) throw new ArgumentOutOfRangeException(nameof(time));
        for (var i = 0; i < time && _running; ++i)
            Tick();
    }

    public void AddProcess(params Process[] processes)
    {
        foreach (var process in processes)
        {
            var pid = GetPId();

            process.ProcessId = pid;
            _processes.Add(pid, process);

            _waitList.AddTimeout(pid, process.ArriveTime);
        }
    }

    public Process GetProcess(int pid)
    {
        return _processes[pid];
    }

    public void SwitchProcess(int pid)
    {
        if (_currentPid > 0)
            if (_processes[_currentPid].State == ProcessState.Running)
            {
                _processes[_currentPid].State = ProcessState.Waiting;
                _waitList.AddTimeout(_currentPid, Clock);
            }

        if (_processes.ContainsKey(pid))
        {
            _currentPid = pid;
            _processes[_currentPid].State = ProcessState.Running;
        }
        else
        {
            throw new ArgumentException("pid not exist.", nameof(pid));
        }
    }

    public Process? CurrentProcess()
    {
        return _currentPid < 1 ? null : _processes[_currentPid];
    }

    public void CompleteProcess(int pid)
    {
        if (_processes.ContainsKey(pid))
            _processes[pid].SetComplete(Clock);
        else
            throw new ArgumentException("pid not exist.", nameof(pid));
    }

    private void Tick()
    {
        ++Clock;
        _waitList.Tick();
        _scheduler.OnTick();

        // clean up finished processes
        foreach (var process in
                 _processes
                     .Where(process => process.Value.State == ProcessState.Terminated)
                     .ToList())
            _processes.Remove(process.Key);
        _running = _processes.Count != 0 &&
                   _processes.Any(process => process.Value.State != ProcessState.Terminated);
    }

    /// <summary>
    ///     Switch out the current process.
    /// </summary>
    /// <param name="pid"></param>
    /// <param name="duration"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void AwaitProcess(int pid, int duration)
    {
        // schedule process pid to the I/O completion time.
        _waitList.AddTimeout(pid, Clock + duration);
    }

    public bool IsProcessRunning(int pid)
    {
        return pid == _currentPid;
    }

    public IEnumerable<int> ExpiredTimeout()
    {
        return _waitList.ExpireTimeouts();
    }

    private int GetPId()
    {
        var pid = 1;
        using var iter = _processes.Keys.GetEnumerator();
        while (iter.MoveNext() && pid == iter.Current)
            ++pid;
        return pid;
    }

    public void Stop()
    {
        _running = false;
    }
}