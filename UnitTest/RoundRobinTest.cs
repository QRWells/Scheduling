#region FileInfo

// Copyright (c) 2022 Wang Qirui. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
// This file is part of Project UnitTest.
// File Name   : RoundRobinTest.cs
// Author      : Qirui Wang
// Created at  : 2022/03/31 0:28
// Description :

#endregion

using Simulator;
using Simulator.Schedulers;

namespace UnitTest;

public class RoundRobinTest
{
    [Fact]
    public void Test1()
    {
        var os = new Os();

        var p1 = new Process().WithArriveTime().WithTasks(
            new Task { Duration = 10, Type = TaskType.CpuBounding });
        var p2 = new Process().WithArriveTime().WithTasks(
            new Task { Duration = 10, Type = TaskType.CpuBounding });
        var p3 = new Process().WithArriveTime().WithTasks(
            new Task { Duration = 10, Type = TaskType.CpuBounding });

        os.AddProcess(p1, p2, p3);

        Assert.NotEqual(p1.ProcessId, p2.ProcessId);
        Assert.NotEqual(p1.ProcessId, p3.ProcessId);

        os.SetScheduler(new RoundRobinScheduler(1));
        os.Step();

        for (var i = 0; i < 10; ++i)
        {
            Assert.Equal(p1.ProcessId, os.CurrentProcess()!.ProcessId);
            os.Step();
            Assert.Equal(p2.ProcessId, os.CurrentProcess()!.ProcessId);
            os.Step();
            Assert.Equal(p3.ProcessId, os.CurrentProcess()!.ProcessId);
            os.Step();
        }
    }

    [Fact]
    public void Test2()
    {
        var os = new Os();

        var p1 = new Process().WithArriveTime().WithTasks(
            new Task { Duration = 10, Type = TaskType.CpuBounding });
        var p2 = new Process().WithArriveTime().WithTasks(
            new Task { Duration = 10, Type = TaskType.CpuBounding });
        var p3 = new Process().WithArriveTime().WithTasks(
            new Task { Duration = 10, Type = TaskType.CpuBounding });

        os.AddProcess(p1, p2, p3);

        Assert.NotEqual(p1.ProcessId, p2.ProcessId);
        Assert.NotEqual(p1.ProcessId, p3.ProcessId);

        os.SetScheduler(new RoundRobinScheduler(2));
        os.Step();

        for (var i = 0; i < 5; ++i)
        {
            Assert.Equal(p1.ProcessId, os.CurrentProcess()!.ProcessId);
            os.Step(2);
            Assert.Equal(p2.ProcessId, os.CurrentProcess()!.ProcessId);
            os.Step(2);
            Assert.Equal(p3.ProcessId, os.CurrentProcess()!.ProcessId);
            os.Step(2);
        }
    }

    [Fact]
    public void Test3()
    {
        var os = new Os();

        var p1 = new Process().WithArriveTime().WithTasks(
            new Task { Duration = 20, Type = TaskType.CpuBounding });
        var p2 = new Process().WithArriveTime().WithTasks(
            new Task { Duration = 20, Type = TaskType.CpuBounding });
        var p3 = new Process().WithArriveTime().WithTasks(
            new Task { Duration = 20, Type = TaskType.CpuBounding });

        os.AddProcess(p1, p2, p3);

        Assert.NotEqual(p1.ProcessId, p2.ProcessId);
        Assert.NotEqual(p1.ProcessId, p3.ProcessId);

        os.SetScheduler(new RoundRobinScheduler(5));
        os.Step();

        for (var i = 0; i < 4; ++i)
        {
            Assert.Equal(p1.ProcessId, os.CurrentProcess()!.ProcessId);
            os.Step(5);
            Assert.Equal(p2.ProcessId, os.CurrentProcess()!.ProcessId);
            os.Step(5);
            Assert.Equal(p3.ProcessId, os.CurrentProcess()!.ProcessId);
            os.Step(5);
        }
    }
}