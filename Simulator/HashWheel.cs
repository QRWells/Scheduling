#region FileInfo

// Copyright (c) 2022 Wang Qirui. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
// This file is part of Project Simulator.
// File Name   : HashedWheel.cs
// Author      : Qirui Wang
// Created at  : 2022/03/26 20:37
// Description :

#endregion

namespace Simulator;

internal struct Timeout<T>
{
    public T Item;
    public ulong SlotIndex;
    public ulong AbsExpire;
    public ulong Prev;
    public ulong Next;
}

public class HashWheel<T>
{
    /// <summary>
    ///     Current free should always stay BEHIND current tick
    /// </summary>
    private ulong _currFree;

    /// <summary>
    ///     Timeouts in the current tick slot have NOT expired yet
    /// </summary>
    private ulong _currTick;

    private readonly List<ulong> _slots = new();
    private Dictionary<ulong, Timeout<T>> _storage = new();

    public TimeSpan Duration { get; }
    public ulong MaxCapacity { get; }
    public TimeSpan MaxTimeout { get; set; }

    private void Insert()
    {
        throw new NotImplementedException();
    }

    public bool TryRemove(ulong key, out T timeout)
    {
        throw new NotImplementedException();
    }

    public bool Expired(out T timeout)
    {
        throw new NotImplementedException();
    }

    public void Tick()
    {
        ++_currTick;
    }

    private ulong CurrentFreeSlot()
    {
        return _currFree % (ulong)_slots.Count;
    }
}