#region FileInfo
// Copyright (c) 2022 Wang Qirui. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
// This file is part of Project Simulator.
// File Name   : IScheduler.cs
// Author      : Qirui Wang
// Created at  : 2022/03/25 6:14
// Description :
#endregion

namespace Simulator.Schedulers;

public interface IScheduler
{
    public void Tick();
}