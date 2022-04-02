#region FileInfo

// Copyright (c) 2022 Wang Qirui. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
// This file is part of Project Simulator.
// File Name   : FairShareScheduler.cs
// Author      : Qirui Wang
// Created at  : 2022/04/02 19:30
// Description :

#endregion

namespace Simulator.Schedulers;

public class FairShareScheduler : IScheduler
{
    private readonly Dictionary<int, int> _processTicket = new();
    private readonly Random _random = new();
    private int _nextPId = -1;
    private Os _os;
    private int _totalTicket;

    Os IScheduler.Os
    {
        get => _os;
        set => _os = value;
    }

    public void OnProcessReady(int pid)
    {
        var ticket = _os.GetProcess(pid).Priority * 100;
        _totalTicket += ticket;
        _processTicket.Add(pid, ticket);
    }

    public void SwitchProcess()
    {
        _os.SwitchProcess(_nextPId);
    }

    public void OnProcessBurst(int pid)
    {
        var list = new List<int>();
        foreach (var (id, _) in _processTicket)
        {
            var p = _os.GetProcess(id);
            if (!p.IsCompleted) continue;
            _totalTicket -= p.Priority * 100;
            list.Add(id);
        }

        foreach (var id in list) _processTicket.Remove(id);

        if (_processTicket.Count == 0)
        {
            _nextPId = -1;
            SwitchProcess();
            return;
        }

        var winner = _random.Next(1, _totalTicket);
        foreach (var (id, ticket) in _processTicket)
        {
            winner -= ticket;
            if (winner > 0) continue;
            _nextPId = id;
            SwitchProcess();
            return;
        }
    }
}