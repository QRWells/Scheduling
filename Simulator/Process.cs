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
    CpuBounding,
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
    public ProcessState State = ProcessState.Runnable;
    public ulong ProcessId { get; set; }
    private Job _job;

    public Process(ulong processId, Job job, ulong arriveTime, ulong runningTime)
    {
        ProcessId = processId;
        _job = job;
        _arriveTime = arriveTime;
        _runningTime = runningTime;
    }

    public ulong TurnaroundTime => _completeTime - _arriveTime;

    private ulong _arriveTime;
    private ulong _completeTime = ulong.MaxValue;
    private ulong _runningTime;

    public void setComplete(ulong currentTime)
    {
        _completeTime = currentTime;
    }
}