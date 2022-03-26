#region FileInfo

// Copyright (c) 2022 Wang Qirui. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
// This file is part of Project Simulator.
// File Name   : HashedWheelTimeout.cs
// Author      : Qirui Wang
// Created at  : 2022/03/27 4:00
// Description :

#endregion

namespace Simulator.Timer;

internal sealed class HashedWheelTimeout<T>
{
    internal HashedWheelTimeout(T item, int rounds)
    {
        Item = item;
        Rounds = rounds;
    }

    internal int Rounds { get; private set; }

    internal T Item { get; }

    internal void TickARound()
    {
        --Rounds;
    }
}