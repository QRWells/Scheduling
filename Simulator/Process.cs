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

public class Task
{
    public int Duration;
    public TaskType Type;
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
    private readonly Queue<Task> _tasks = new();

    public Process(int arriveTime)
    {
        ArriveTime = arriveTime;
    }

    /// <summary>
    ///     Time at which process completes its execution.
    /// </summary>
    public int CompleteTime { get; private set; } = -1;

    /// <summary>
    ///     Time at which the process arrives in the ready queue.
    /// </summary>
    public int ArriveTime { get; }

    /// <summary>
    ///     Time required by a process for CPU execution.
    /// </summary>
    public int BurstTime { get; private set; }

    /// <summary>
    ///     Time has been executed.
    /// </summary>
    public int TimeHaveBurst { get; private set; }

    /// <summary>
    ///     Remaining time for execution.
    /// </summary>
    public int RemainingTime => BurstTime - TimeHaveBurst;

    /// <summary>
    ///     Time from process arrival to its first execution.
    /// </summary>
    public int ResponseTime { get; } = -1;

    /// <summary>
    ///     total duration of the CPU bound tasks.
    /// </summary>
    public int CpuDuration { get; private set; }

    /// <summary>
    ///     total duration of the I/O bound tasks.
    /// </summary>
    public int IoDuration { get; private set; }

    /// <summary>
    ///     Optional process name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     the priority of the process.
    /// </summary>
    public int Priority { get; init; } = 0;

    /// <summary>
    ///     the current state of the process.
    /// </summary>
    public ProcessState State { get; private set; } = ProcessState.Runnable;

    /// <summary>
    ///     the id of the process.
    /// </summary>
    public int ProcessId { get; set; }

    /// <summary>
    ///     Time Difference between completion time and arrival time.
    /// </summary>
    public int TurnaroundTime => CompleteTime - ArriveTime;

    /// <summary>
    ///     Time Difference between turn around time and burst time.
    /// </summary>
    public int WaitTime => TurnaroundTime - BurstTime;

    /// <summary>
    ///     Indicate if the process is finished.
    /// </summary>
    public bool IsCompleted => State == ProcessState.Terminated;

    public void AddTask(int duration, TaskType taskType = TaskType.CpuBounding)
    {
        _tasks.Enqueue(new Task { Type = taskType, Duration = duration });
        CpuDuration += taskType == TaskType.CpuBounding ? duration : 0;
        IoDuration += taskType == TaskType.IoBounding ? duration : 0;
        BurstTime += duration;
    }

    public void SetComplete(int currentTime)
    {
        State = ProcessState.Terminated;
        CompleteTime = currentTime;
    }

    /// <summary>
    ///     Bump to next task to simulate await on I/O task.
    /// </summary>
    /// <param name="task">the current I/O task for log.</param>
    /// <returns>bool indicate if this process has completed.</returns>
    /// <exception cref="ArgumentException"></exception>
    public bool BumpToNext(out Task? task)
    {
        if (!_tasks.TryDequeue(out task))
            throw new ArgumentException();
        TimeHaveBurst += task.Duration;
        return TimeHaveBurst >= BurstTime;
    }

    /// <summary>
    ///     Burst 1 tick to simulate execution.
    /// </summary>
    /// <param name="clock"></param>
    /// <returns></returns>
    public Task? Burst(int clock)
    {
        ++TimeHaveBurst;

        // process finished.
        if (TimeHaveBurst >= BurstTime)
        {
            SetComplete(clock);
            return null;
        }

        --_tasks.Peek().Duration;

        return _tasks.Peek();
    }
}