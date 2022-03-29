#region FileInfo
// Copyright (c) 2022 Wang Qirui. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
// This file is part of Project UnitTest.
// File Name   : SJFTest.cs
// Author      : Qirui Wang
// Created at  : 2022/03/30 1:25
// Description :
#endregion

using Simulator;
using Simulator.Schedulers;

namespace UnitTest;

public class SJFTest
{
    [Fact]
    public void Test1()
    {
        var os = new Os();

        var p1 = new Process().WithArriveTime().WithTasks(
            new Task { Duration = 40, Type = TaskType.CpuBounding });
        var p2 = new Process().WithArriveTime().WithTasks(
            new Task { Duration = 30, Type = TaskType.CpuBounding });

        os.AddProcess(p1, p2);
        
        Assert.NotEqual(p1.ProcessId, p2.ProcessId);
        
        os.SetSchedule(new SJFScheduler());
        os.Step();
        
        // Start Running
        os.Step();

        // Current time is 1 (init is -1).
        Assert.Equal(1, os.Clock);
        Assert.Equal(p2.ProcessId, os.CurrentProcess()!.ProcessId);
        
        Assert.False(p1.IsCompleted);
        Assert.Equal(0, p1.TimeHaveBurst);
        Assert.Equal(40, p1.RemainingTime);
        
        Assert.False(p2.IsCompleted);
        Assert.Equal(1, p2.TimeHaveBurst);
        Assert.Equal(29, p2.RemainingTime);

        os.Step(30);

        // Current time is 41.
        Assert.Equal(p1.ProcessId, os.CurrentProcess()!.ProcessId);
        
        Assert.False(p1.IsCompleted);
        Assert.Equal(30, p2.TurnaroundTime);
        
        Assert.True(p2.IsCompleted);
        Assert.Equal(39, p1.RemainingTime);
        
        os.Step(40);
        
        // Current time is 71.
        Assert.Equal(70, p1.TurnaroundTime);
    }
}