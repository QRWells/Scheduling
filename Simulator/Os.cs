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

using Simulator.Data;
using Simulator.Schedulers;
using Simulator.Timer;

namespace Simulator;

public class Os
{
    private readonly int[] _pIdOfCpu;

    /// <summary>
    ///     processes in the OS.
    /// </summary>
    private readonly SortedList<int, Process> _processes = new();

    /// <summary>
    ///     waiting queue of the OS.
    /// </summary>
    private readonly HashedWheel<int> _waitList = new();

    /// <summary>
    ///     the pid of the current running process.
    /// </summary>
    private int _currentPid;

    /// <summary>
    ///     Time remaining for context switch.
    /// </summary>
    private int _remainingTime;

    /// <summary>
    ///     Indicates whether the OS is running.
    /// </summary>
    private bool _running;

    /// <summary>
    ///     The scheduler used in the OS.
    ///     TODO: add multi-scheduler support
    /// </summary>
    private IScheduler _scheduler = new FCFSScheduler();

    /// <summary>
    ///     The number of cores in the OS.
    ///     TODO: add multi-core support
    /// </summary>
    internal uint Core = 1;

    public Os(uint core = 1)
    {
        if (core < 1)
            throw new ArgumentException("core must be greater than 0");
        _pIdOfCpu = new int[core];

        _running = true;
    }

    /// <summary>
    ///     Cost of context switch.
    /// </summary>
    public int ContextSwitchCost { get; set; } = 0;

    /// <summary>
    ///     The interval for each step.
    /// </summary>
    public int Interval { get; set; } = 1;

    /// <summary>
    ///     Current clock of the OS.
    /// </summary>
    public int Clock { get; private set; } = -1;

    public void SetScheduler(IScheduler scheduler)
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
            var pid = GeneratePId();

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
        _remainingTime = ContextSwitchCost;
        if (_currentPid > 0 && _processes[_currentPid].State == ProcessState.Running)
            _processes[_currentPid].State = ProcessState.Waiting;

        if (_processes.ContainsKey(pid))
        {
            _currentPid = pid;
            _processes[_currentPid].State = ProcessState.Running;
        }
        else if (pid == -1)
        {
            _currentPid = pid;
            _remainingTime = 0;
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

    /// <summary>
    ///     Get current process id.
    /// </summary>
    /// <returns> -2 for scheduling, -1 for idle, above 0 for processes</returns>
    public int CurrentPid()
    {
        return _remainingTime != 0 ? -2 : _currentPid;
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

        if (_remainingTime > 0)
        {
            --_remainingTime;
            _scheduler.OnSwitching();
        }
        else
        {
            _scheduler.OnTick();
        }


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
        _waitList.AddTimeout(pid, duration);
    }

    public bool IsProcessRunning(int pid)
    {
        return pid == _currentPid;
    }

    public IEnumerable<int> ExpiredTimeout()
    {
        return _waitList.ExpireTimeouts();
    }

    private int GeneratePId()
    {
        var pid = 1;
        using var iter = _processes.Keys.GetEnumerator();
        while (iter.MoveNext() && pid == iter.Current)
            ++pid;
        return pid;
    }

    private IEnumerable<int> GetFreeCores()
    {
        return from i in _pIdOfCpu
            where _pIdOfCpu[i] < 1
            select i;
    }

    public SystemInfo GetCurrentSystemInfo()
    {
        var procInfos = _processes.Select(process => process.Value.GetProcessInfo()).ToList();
        return new SystemInfo(Clock, Core, procInfos);
    }

    public void Stop()
    {
        _running = false;
    }

    public void Reset()
    {
        Clock = -1;
        _processes.Clear();
        _waitList.Clear();
        for (var i = 0; i < _pIdOfCpu.Length; ++i) _pIdOfCpu[i] = 0;
        _running = true;
        _currentPid = 0;
        _remainingTime = 0;
    }
}