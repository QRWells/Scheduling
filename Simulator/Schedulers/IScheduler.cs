﻿#region FileInfo

// Copyright (c) 2022 Wang Qirui. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
// This file is part of Project Simulator.
// File Name   : IScheduler.cs
// Author      : Qirui Wang
// Created at  : 2022/03/25 6:14
// Description :

#endregion

namespace Simulator.Schedulers;

public interface IScheduler
{
    void OnProcessReady(Os os, ulong pid);
    void SwitchProcess(Os os);

    void OnTick(Os os)
    {
        BurstProcess(os);
    }

    void BurstProcess(Os os)
    {
    }

    void RunTask(Os os, TaskType type, ulong duration, ulong pid)
    {
        switch (type)
        {
            case TaskType.IoBounding:
                RunIoBoundTask(os, pid, duration);
                break;
            case TaskType.CpuBounding:
                RunCpuBoundTask(os, pid, duration);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        ;
    }

    void RunIoBoundTask(Os os, ulong duration, ulong pid)
    {
        var clock = os.Clock;
        var p = os.GetProcess(pid);
        var (task, id, isCompleted) = p.Next();
        if (isCompleted)
            os.CompleteProcess(id);
        else
            os.AwaitProcess(pid, duration);
        if (os.IsProcessRunning(pid)) SwitchProcess(os);
    }

    void RunCpuBoundTask(Os os, ulong duration, ulong pid)
    {
    }
}