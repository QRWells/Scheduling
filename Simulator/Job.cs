#region FileInfo

// Copyright (c) 2022 Wang Qirui. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
// This file is part of Project Simulator.
// File Name   : Job.cs
// Author      : Qirui Wang
// Created at  : 2022/03/25 5:57
// Description :

#endregion

namespace Simulator;

public class Job
{
    private readonly Queue<(TaskType, ulong)> _tasks = new();
    public ulong CpuDuration { get; private set; }
    public ulong IoDuration { get; private set; }
    public ulong TotalDuration { get; private set; }

    public void AddTask(TaskType taskType, ulong duration)
    {
        _tasks.Enqueue((taskType, duration));
        CpuDuration += taskType == TaskType.CpuBounding ? duration : 0;
        IoDuration += taskType == TaskType.IoBounding ? duration : 0;
        TotalDuration += duration;
    }

    public (TaskType, ulong) FetchTask()
    {
        var (taskType, duration) = _tasks.Dequeue();
        return (taskType, duration);
    }
}