#region FileInfo

// Copyright (c) 2022 Wang Qirui. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
// This file is part of Project UnitTest.
// File Name   : STCFTest.cs
// Author      : Qirui Wang
// Created at  : 2022/03/30 1:25
// Description :

#endregion

using Simulator;
using Simulator.Schedulers;

namespace UnitTest;

public class STCFTest
{
    [Fact]
    public void Test1()
    {
        var os = new Os();

        var p1 = new Process().WithArriveTime().WithTasks(
            new Task { Duration = 30, Type = TaskType.CpuBounding });
        var p2 = new Process().WithArriveTime(5).WithTasks(
            new Task { Duration = 20, Type = TaskType.CpuBounding });
        var p3 = new Process().WithArriveTime(10).WithTasks(
            new Task { Duration = 10, Type = TaskType.CpuBounding });

        os.AddProcess(p1, p2, p3);

        Assert.NotEqual(p1.ProcessId, p2.ProcessId);
        Assert.NotEqual(p1.ProcessId, p3.ProcessId);

        os.SetSchedule(new STCFScheduler());
        os.Step();

        // Start Running
        os.Step();

        // Current time is 1 (init is -1).
        Assert.Equal(1, os.Clock);
        Assert.Equal(p1.ProcessId, os.CurrentProcess()!.ProcessId);

        Assert.False(p1.IsCompleted);
        Assert.Equal(1, p1.TimeHaveBurst);
        Assert.Equal(29, p1.RemainingTime);

        Assert.False(p2.IsCompleted);
        Assert.Equal(0, p2.TimeHaveBurst);
        Assert.Equal(20, p2.RemainingTime);

        Assert.False(p3.IsCompleted);
        Assert.Equal(0, p3.TimeHaveBurst);
        Assert.Equal(10, p3.RemainingTime);

        os.Step(5);

        // Current time is 6, switch to p2.
        Assert.Equal(6, os.Clock);
        Assert.Equal(p2.ProcessId, os.CurrentProcess()!.ProcessId);

        Assert.False(p1.IsCompleted);
        Assert.Equal(5, p1.TimeHaveBurst);
        Assert.Equal(25, p1.RemainingTime);

        Assert.False(p2.IsCompleted);
        Assert.Equal(1, p2.TimeHaveBurst);
        Assert.Equal(19, p2.RemainingTime);

        Assert.False(p3.IsCompleted);
        Assert.Equal(0, p3.TimeHaveBurst);
        Assert.Equal(10, p3.RemainingTime);

        os.Step(5);

        // Current time is 11, switch to p3.
        Assert.Equal(11, os.Clock);
        Assert.Equal(p3.ProcessId, os.CurrentProcess()!.ProcessId);

        Assert.False(p1.IsCompleted);
        Assert.Equal(5, p1.TimeHaveBurst);
        Assert.Equal(25, p1.RemainingTime);

        Assert.False(p2.IsCompleted);
        Assert.Equal(5, p2.TimeHaveBurst);
        Assert.Equal(15, p2.RemainingTime);

        Assert.False(p3.IsCompleted);
        Assert.Equal(1, p3.TimeHaveBurst);
        Assert.Equal(9, p3.RemainingTime);

        os.Step(10);

        // Current time is 21, switch to p2.
        Assert.Equal(21, os.Clock);
        Assert.Equal(p2.ProcessId, os.CurrentProcess()!.ProcessId);

        Assert.False(p1.IsCompleted);
        Assert.Equal(5, p1.TimeHaveBurst);
        Assert.Equal(25, p1.RemainingTime);

        Assert.False(p2.IsCompleted);
        Assert.Equal(6, p2.TimeHaveBurst);
        Assert.Equal(14, p2.RemainingTime);

        Assert.True(p3.IsCompleted);
        Assert.Equal(0, p3.ResponseTime);
        Assert.Equal(10, p3.TurnaroundTime);

        os.Step(15);

        // Current time is 36, switch to p1.
        Assert.Equal(36, os.Clock);
        Assert.Equal(p1.ProcessId, os.CurrentProcess()!.ProcessId);

        Assert.False(p1.IsCompleted);
        Assert.Equal(6, p1.TimeHaveBurst);
        Assert.Equal(24, p1.RemainingTime);

        Assert.True(p2.IsCompleted);
        Assert.Equal(0, p2.ResponseTime);
        Assert.Equal(30, p2.TurnaroundTime);

        Assert.True(p3.IsCompleted);
        Assert.Equal(0, p3.ResponseTime);
        Assert.Equal(10, p3.TurnaroundTime);

        os.Run();
        Assert.Equal(60, os.Clock);
    }
}