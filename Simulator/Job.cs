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
    private readonly Queue<(TaskType, int)> _tasks = new();
    public int CpuDuration { get; private set; }
    public int IoDuration { get; private set; }
    public int TotalDuration { get; private set; }

    public void AddTask(TaskType taskType, int duration)
    {
        _tasks.Enqueue((taskType, duration));
        CpuDuration += taskType == TaskType.CpuBounding ? duration : 0;
        IoDuration += taskType == TaskType.IoBounding ? duration : 0;
        TotalDuration += duration;
    }

    public (TaskType, int) FetchTask()
    {
        var (taskType, duration) = _tasks.Dequeue();
        return (taskType, duration);
    }

    public void Tick(int ticks)
    {
        throw new NotImplementedException();
    }
}