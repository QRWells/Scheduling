#region FileInfo

// Copyright (c) 2022 Wang Qirui. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
// This file is part of Project Simulator.
// File Name   : Process.cs
// Author      : Qirui Wang
// Created at  : 2022/03/25 5:57
// Description :

#endregion

namespace Simulator;

public enum TaskType
{
    IoBounding,
    CpuBounding
}

public enum ProcessState
{
    Runnable,
    Running,
    Waiting,
    Terminated
}

public class Process
{
    /// <summary>
    ///     Time at which the process arrives in the ready queue.
    /// </summary>
    private readonly ulong _arriveTime;

    /// <summary>
    ///     Time required by a process for CPU execution.
    /// </summary>
    private ulong _burstTime;

    /// <summary>
    ///     Time at which process completes its execution.
    /// </summary>
    private ulong _completeTime;

    private Job _job;

    public Process(ulong processId, Job job, ulong arriveTime)
    {
        ProcessId = processId;
        _job = job;
        _arriveTime = arriveTime;
        _completeTime = job.TotalDuration;
    }

    public string? Name { get; set; }
    public int Priority { get; init; } = 0;
    public ProcessState State { get; private set; } = ProcessState.Runnable;

    public ulong ProcessId { get; set; }

    /// <summary>
    ///     Time Difference between completion time and arrival time.
    /// </summary>
    public ulong TurnaroundTime => _completeTime - _arriveTime;

    /// <summary>
    ///     Time Difference between turn around time and burst time.
    /// </summary>
    public ulong WaitTime => TurnaroundTime - _burstTime;

    public void SetComplete(ulong currentTime)
    {
        State = ProcessState.Terminated;
        _completeTime = currentTime;
    }

    public (TaskType, ulong, bool) Next()
    {
        return (TaskType.CpuBounding, _arriveTime, false);
    }
}