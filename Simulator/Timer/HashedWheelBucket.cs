#region FileInfo

// Copyright (c) 2022 Wang Qirui. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
// This file is part of Project Simulator.
// File Name   : HashedWheelBucket.cs
// Author      : Qirui Wang
// Created at  : 2022/03/27 4:00
// Description :

#endregion

namespace Simulator.Timer;

internal class HashedWheelBucket<T>
{
    private readonly LinkedList<HashedWheelTimeout<T>> _timeouts = new();

    internal void AddTimeout(HashedWheelTimeout<T> timeout)
    {
        _timeouts.AddLast(timeout);
    }

    /// <summary>
    ///     Expire all HashedWheelTimeout for the given deadline.
    /// </summary>
    internal IEnumerable<T> ExpireTimeouts()
    {
        var result =
            from timeout in _timeouts
            where timeout.Rounds <= 0
            select timeout;
        var hashedWheelTimeouts = result.ToList();

        foreach (var timeout in hashedWheelTimeouts)
            _timeouts.Remove(timeout);

        return from item in hashedWheelTimeouts select item.Item;
    }

    internal void Tick()
    {
        foreach (var timeout in _timeouts) timeout.TickARound();
    }
}