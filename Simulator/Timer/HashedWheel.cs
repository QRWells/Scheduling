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

namespace Simulator.Timer;

public sealed class HashedWheel<T>
{
    private readonly HashedWheelBucket<T>[] _buckets;

    public HashedWheel(int wheelSize = 8, int resolution = 1)
    {
        if (wheelSize < 1 || resolution < 1) throw new ArgumentException();
        // Avoid unreachable buckets.
        if (resolution != 1 && wheelSize % resolution == 0) throw new ArgumentException();
        _buckets = new HashedWheelBucket<T>[wheelSize];
        for (var i = 0; i < wheelSize; i++)
            _buckets[i] = new HashedWheelBucket<T>();

        Resolution = resolution;
    }

    public int CurrentTick { get; private set; } = -1;

    public int Resolution { get; }

    public int WheelSize => _buckets.Length;

    public bool Empty()
    {
        return _buckets.All(b => b.Empty());
    }

    public void Tick()
    {
        CurrentTick += Resolution;
        _buckets[CurrentTick % _buckets.Length].Tick();
    }

    public void AddTimeout(T value, int deadline)
    {
        var round = deadline / WheelSize + 1;
        var timeout = new HashedWheelTimeout<T>(value, round);
        _buckets[deadline % WheelSize].AddTimeout(timeout);
    }

    public IEnumerable<T> ExpireTimeouts()
    {
        return _buckets[CurrentTick % WheelSize].ExpireTimeouts();
    }
}