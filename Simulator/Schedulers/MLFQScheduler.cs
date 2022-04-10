#region FileInfo

// Copyright (c) 2022 Wang Qirui. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
// This file is part of Project Simulator.
// File Name   : MLFQScheduler.cs
// Author      : Qirui Wang
// Created at  : 2022/03/25 6:28
// Description :

#endregion

namespace Simulator.Schedulers;

public sealed class MLFQScheduler : IScheduler
{
    private readonly LinkedList<int>[] _queues = new LinkedList<int>[3];
    private readonly int[] _timeSlice = new int[2];
    private readonly Dictionary<int, int> _usedTimeSlice = new();
    private Os? _os;

    /// <summary>
    ///     pid, priority
    /// </summary>
    private (int, int) _runningProcess;

    Os? IScheduler.Os
    {
        get => _os;
        set => _os = value;
    }

    public void OnProcessReady(int pid)
    {
        _queues[0].AddFirst(pid);
    }

    public void SwitchProcess()
    {
        for (var i = 0; i < _queues.Length; i++)
        {
            if (_queues[i].Count == 0)
                continue;

            var pid = _queues[i].Last!.Value;
            _queues[i].RemoveLast();
            _runningProcess = (pid, i);
            _os!.SwitchProcess(pid);
            return;
        }

        _runningProcess.Item1 = -1;
        _os!.SwitchProcess(-1);
    }

    public void OnProcessBurst(int pid)
    {
        var priority = GetPriority(pid);
        var last = _queues.Length - 1;
        if (priority >= last)
        {
            if (_queues[..last].All(q => q.Count == 0)) return;
            _queues[last].AddLast(pid);
            SwitchProcess();
        }
        else
        {
            var usedTimeSlice = _usedTimeSlice.GetValueOrDefault(pid, 0);
            if (usedTimeSlice >= _timeSlice[priority] && _os!.IsProcessRunning(pid))
            {
                LevelDown(pid);
                _usedTimeSlice.Add(pid, 0);
                SwitchProcess();
            }
            else
            {
                _usedTimeSlice.Add(pid, usedTimeSlice + _os!.Interval);
            }
        }
    }

    private int GetPriority(int pid)
    {
        if (_runningProcess.Item1 == pid)
            return _runningProcess.Item2;

        for (var i = 0; i < _queues.Length; i++)
            if (_queues[i].Contains(pid))
                return i;

        return 0;
    }

    private bool IsProcessRunning(int pid)
    {
        return _runningProcess.Item1 == pid;
    }

    private void LevelDown(int pid)
    {
        var priority = GetPriority(pid);
        if (priority >= _queues.Length - 1)
            return;
        _queues[priority].Remove(pid);
        _queues[priority + 1].AddLast(pid);
    }
}