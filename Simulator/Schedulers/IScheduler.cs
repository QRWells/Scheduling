#region FileInfo

// Copyright (c) 2022 Wang Qirui. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
// This file is part of Project Simulator.
// File Name   : IScheduler.cs
// Author      : Qirui Wang
// Created at  : 2022/03/25 6:14
// Description :

#endregion

using System.Diagnostics;

namespace Simulator.Schedulers;

public interface IScheduler
{
    internal Os Os { get; set; }

    void OnProcessReady(int pid);
    void SwitchProcess();

    void OnTick()
    {
        foreach (var pid in Os.ExpiredTimeout()) OnProcessReady(pid);

        BurstProcess();
    }

    void BurstProcess()
    {
        var clock = Os.Clock;
        var process = Os.CurrentProcess();
        if (process == null) SwitchProcess();

        Debug.Assert(process != null);
        // burst 1 tick, 
        var task = process.Burst(clock);
        var complete = process.IsCompleted;
        var pid = process.ProcessId;

        if (task != null)
        {
            RunTask(task.Type, task.Duration, pid);
        }
        else if (complete)
        {
            Os.CompleteProcess(pid);
            if (Os.IsProcessRunning(pid))
                SwitchProcess();
        }

        // Preemptive
        OnProcessBurst(pid);
    }


    void RunTask(TaskType type, int duration, int pid)
    {
        switch (type)
        {
            case TaskType.IoBounding:
                RunIoBoundTask(pid, duration);
                break;
            case TaskType.CpuBounding:
                RunCpuBoundTask(pid, duration);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    /// <summary>
    ///     Switch out for I/O task.
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="pid"></param>
    void RunIoBoundTask(int duration, int pid)
    {
        var clock = Os.Clock;
        var p = Os.GetProcess(pid);
        var isCompleted = p.BumpToNext(out _);
        // TODO: log
        if (isCompleted)
            Os.CompleteProcess(pid);
        else
            Os.AwaitProcess(pid, duration);
        if (Os.IsProcessRunning(pid)) SwitchProcess();
    }

    /// <summary>
    ///     Do nothing
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="pid"></param>
    void RunCpuBoundTask(int duration, int pid)
    {
    }

    /// <summary>
    ///     Implemented by schedulers with preemption
    /// </summary>
    /// <param name="pid"></param>
    void OnProcessBurst(int pid)
    {
    }
}