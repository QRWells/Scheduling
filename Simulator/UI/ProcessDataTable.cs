#region FileInfo

// Copyright (c) 2022 Wang Qirui. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
// This file is part of Project Simulator.
// File Name   : ProcessDataTable.cs
// Author      : Qirui Wang
// Created at  : 2022/04/06 21:52
// Description :

#endregion

using System.Data;

namespace Simulator.UI;

public class ProcessDataTable : DataTable
{
    private readonly List<Process> _processes = new();

    public ProcessDataTable()
    {
        Columns.Add(new DataColumn { ColumnName = "ProcessID", DataType = typeof(int) });
        Columns.Add(new DataColumn { ColumnName = "ProcessName", DataType = typeof(string) });
        Columns.Add(new DataColumn { ColumnName = "ProcessState", DataType = typeof(string) });
        Columns.Add(new DataColumn { ColumnName = "ArriveTime", DataType = typeof(int) });
        Columns.Add(new DataColumn { ColumnName = "BurstTime", DataType = typeof(int) });
        Columns.Add(new DataColumn { ColumnName = "TimeHaveBurst", DataType = typeof(int) });
    }

    public void AddProcess(Process proc)
    {
        _processes.Add(proc);
        Update();
    }

    internal void Update()
    {
        _processes.Sort((p1, p2) => p1.ProcessId.CompareTo(p2.ProcessId));
        Rows.Clear();
        foreach (var process in _processes) Rows.Add(GetDataRow(process));
    }

    private DataRow GetDataRow(Process proc)
    {
        var res = NewRow();
        res["ProcessID"] = proc.ProcessId;
        res["ProcessName"] = proc.Name ?? "No Name";
        res["ProcessState"] = proc.State.ToString();
        res["ArriveTime"] = proc.ArriveTime;
        res["BurstTime"] = proc.BurstTime;
        res["TimeHaveBurst"] = proc.TimeHaveBurst;
        return res;
    }
}