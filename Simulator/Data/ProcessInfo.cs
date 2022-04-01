#region FileInfo

// Copyright (c) 2022 Wang Qirui. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
// This file is part of Project Simulator.
// File Name   : ProcessInfo.cs
// Author      : Qirui Wang
// Created at  : 2022/03/31 22:11
// Description :

#endregion

namespace Simulator.Data;

public record struct ProcessInfo(
    int PId,
    int Cpu
);