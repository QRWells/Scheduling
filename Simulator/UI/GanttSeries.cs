#region FileInfo

// Copyright (c) 2022 Wang Qirui. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
// This file is part of Project Simulator.
// File Name   : GanttSeries.cs
// Author      : Qirui Wang
// Created at  : 2022/04/02 3:33
// Description :

#endregion

using Terminal.Gui;
using Terminal.Gui.Graphs;
using Attribute = Terminal.Gui.Attribute;

namespace Simulator.UI;

public class GanttSeries : BarSeries
{
    private static readonly Attribute[] Colors =
    {
        Application.Driver.MakeAttribute(Color.White, Color.Black),
        Application.Driver.MakeAttribute(Color.Blue, Color.Black),
        Application.Driver.MakeAttribute(Color.Brown, Color.Black),
        Application.Driver.MakeAttribute(Color.Cyan, Color.Black),
        Application.Driver.MakeAttribute(Color.Gray, Color.Black),
        Application.Driver.MakeAttribute(Color.Green, Color.Black),
        Application.Driver.MakeAttribute(Color.Magenta, Color.Black),
        Application.Driver.MakeAttribute(Color.Red, Color.Black),
        Application.Driver.MakeAttribute(Color.BrightBlue, Color.Black),
        Application.Driver.MakeAttribute(Color.BrightRed, Color.Black),
        Application.Driver.MakeAttribute(Color.BrightCyan, Color.Black),
        Application.Driver.MakeAttribute(Color.BrightGreen, Color.Black),
        Application.Driver.MakeAttribute(Color.BrightMagenta, Color.Black),
        Application.Driver.MakeAttribute(Color.BrightYellow, Color.Black)
    };

    private Dictionary<int, int> _colorMap = new();
    private List<BarSeries> _coreSeries;
    private BarSeries _ioSeries;
    private Os _os;

    public GanttSeries(Os os)
    {
        _os = os;
    }

    protected override void DrawBarLine(GraphView graph, Point start, Point end, Bar beingDrawn)
    {
        var driver = Application.Driver;
        // TODO: draw the bar
    }
}