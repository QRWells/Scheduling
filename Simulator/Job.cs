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
    private Queue<(TaskType, ulong)> _tasks = new();
    public ulong CpuDuration { get; set; } = 0;
    public ulong IoDuration { get; set; } = 0;
    public ulong TotalDuration { get; set; } = 0;
}