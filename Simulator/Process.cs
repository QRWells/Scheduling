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
    public int ArriveTime { get; private set; }

    /// <summary>
    ///     Time required by a process for CPU execution.
    /// </summary>
    public int BurstTime { get; init; }

    public int TimeHaveBurst { get; private set; }

    public int RemainingTime => BurstTime - TimeHaveBurst;

    /// <summary>
    ///     Time at which process completes its execution.
    /// </summary>
    private int _completeTime;

    private Job _job;

    public Process(int processId, Job job, int arriveTime)
    {
        ProcessId = processId;
        _job = job;
        ArriveTime = arriveTime;
        BurstTime = job.TotalDuration;
    }

    public string? Name { get; set; }
    public int Priority { get; init; } = 0;
    public ProcessState State { get; private set; } = ProcessState.Runnable;

    public int ProcessId { get; set; }

    /// <summary>
    ///     Time Difference between completion time and arrival time.
    /// </summary>
    public int TurnaroundTime => _completeTime - ArriveTime;

    /// <summary>
    ///     Time Difference between turn around time and burst time.
    /// </summary>
    public int WaitTime => TurnaroundTime - BurstTime;

    public bool IsCompleted => State == ProcessState.Terminated;

    public void SetComplete(int currentTime)
    {
        State = ProcessState.Terminated;
        _completeTime = currentTime;
    }

    public (TaskType, int, bool) Next()
    {
        // TODO: Need implementation
        return (TaskType.CpuBounding, ArriveTime, false);
    }

    public (TaskType?, int) Burst(int ticks)
    {
        // TODO: Need implementation
        TimeHaveBurst += ticks;
        if (RemainingTime <= 0)
        {
            return (null, 0);
        }

        return (TaskType.CpuBounding, RemainingTime);
    }
}