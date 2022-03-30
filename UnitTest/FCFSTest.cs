#region FileInfo

// Copyright (c) 2022 Wang Qirui. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
// This file is part of Project UnitTest.
// File Name   : FCFSTest.cs
// Author      : Qirui Wang
// Created at  : 2022/03/30 1:20
// Description :

#endregion

global using Xunit;
using Simulator;
using Simulator.Schedulers;

namespace UnitTest;

public class FCFSTest
{
    [Fact]
    public void Test1()
    {
        var os = new Os();

        var p1 = new Process().WithArriveTime().WithTasks(
            new Task { Duration = 40, Type = TaskType.CpuBounding });
        var p2 = new Process().WithArriveTime(5).WithTasks(
            new Task { Duration = 30, Type = TaskType.CpuBounding });

        os.AddProcess(p1, p2);

        Assert.NotEqual(p1.ProcessId, p2.ProcessId);

        os.SetSchedule(new FCFSScheduler());
        os.Step();

        // Start Running
        os.Step();

        // Current time is 1 (init is -1).
        Assert.Equal(1, os.Clock);

        Assert.Equal(p1.ProcessId, os.CurrentProcess()!.ProcessId);
        Assert.False(p1.IsCompleted);
        Assert.Equal(1, p1.TimeHaveBurst);
        Assert.Equal(39, p1.RemainingTime);

        Assert.False(p2.IsCompleted);
        Assert.Equal(0, p2.TimeHaveBurst);
        Assert.Equal(30, p2.RemainingTime);

        os.Step(40);

        // Current time is 41.
        Assert.Equal(p2.ProcessId, os.CurrentProcess()!.ProcessId);

        Assert.True(p1.IsCompleted);
        Assert.Equal(40, p1.TurnaroundTime);

        Assert.False(p2.IsCompleted);
        Assert.Equal(40, p2.ResponseTime);
        Assert.Equal(29, p2.RemainingTime);

        os.Step(30);

        // Current time is 71.
        Assert.True(p2.IsCompleted);
        Assert.Equal(70, p2.TurnaroundTime);
    }

    [Fact]
    public void Test2()
    {
        var os = new Os();

        var p1 = new Process().WithArriveTime().WithTasks(
            new Task { Duration = 10, Type = TaskType.CpuBounding });
        var p2 = new Process().WithArriveTime(15).WithTasks(
            new Task { Duration = 10, Type = TaskType.CpuBounding });

        os.AddProcess(p1, p2);

        Assert.NotEqual(p1.ProcessId, p2.ProcessId);

        os.SetSchedule(new FCFSScheduler());
        os.Step();

        // Start Running
        os.Step();

        // Current time is 1 (init is -1).
        Assert.Equal(1, os.Clock);
        Assert.Equal(p1.ProcessId, os.CurrentProcess()!.ProcessId);

        Assert.False(p1.IsCompleted);
        Assert.Equal(ProcessState.Running, p1.State);
        Assert.Equal(1, p1.TimeHaveBurst);
        Assert.Equal(9, p1.RemainingTime);

        Assert.False(p2.IsCompleted);
        Assert.Equal(ProcessState.Runnable, p2.State);
        Assert.Equal(0, p2.TimeHaveBurst);
        Assert.Equal(10, p2.RemainingTime);

        os.Step(10);
        // Current time is 11.
        Assert.Null(os.CurrentProcess());

        Assert.True(p1.IsCompleted);
        Assert.Equal(ProcessState.Terminated, p1.State);
        Assert.Equal(10, p1.TimeHaveBurst);
        Assert.Equal(0, p1.RemainingTime);

        Assert.False(p2.IsCompleted);
        Assert.Equal(ProcessState.Runnable, p2.State);
        Assert.Equal(0, p2.TimeHaveBurst);
        Assert.Equal(10, p2.RemainingTime);

        os.Step(10);
        // Current time is 21.
        Assert.Equal(p2.ProcessId, os.CurrentProcess()!.ProcessId);

        Assert.False(p2.IsCompleted);
        Assert.Equal(ProcessState.Running, p2.State);
        Assert.Equal(6, p2.TimeHaveBurst);
        Assert.Equal(4, p2.RemainingTime);
    }
}