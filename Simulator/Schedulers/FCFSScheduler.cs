#region FileInfo

// Copyright (c) 2022 Wang Qirui. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
// This file is part of Project Simulator.
// File Name   : FCFSScheduler.cs
// Author      : Qirui Wang
// Created at  : 2022/03/25 6:26
// Description :

#endregion

namespace Simulator.Schedulers;

public sealed class FCFSScheduler : IScheduler
{
    private readonly Queue<int> _readyQueue = new();
    private Os? _os = null;

    Os? IScheduler.Os
    {
        get => _os;
        set => _os = value;
    }

    public void OnProcessReady(int pid)
    {
        _readyQueue.Enqueue(pid);
    }

    public void SwitchProcess()
    {
        if (_readyQueue.TryDequeue(out var pid))
            _os!.SwitchProcess(pid);
        else
            _os!.SwitchProcess(-1);
    }
}