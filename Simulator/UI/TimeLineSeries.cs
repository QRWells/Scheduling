#region FileInfo

// Copyright (c) 2022 Wang Qirui. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
// This file is part of Project Simulator.
// File Name   : ITimeLine.cs
// Author      : Qirui Wang
// Created at  : 2022/04/08 10:06
// Description :

#endregion

using Terminal.Gui;
using Terminal.Gui.Graphs;
using Attribute = Terminal.Gui.Attribute;

namespace Simulator.UI;

public class TimeLineSeries : ISeries
{
    public readonly List<Timeline> Timelines = new();

    public int Width { get; set; } = 1;

    public float Offset { get; set; } = 0;

    public bool DrawLabels { get; set; } = true;

    public Attribute? OverrideBarColor { get; set; }

    public void DrawSeries(GraphView graph, Rect drawBounds, RectangleF graphBounds)
    {
        for (var index = 0; index < Timelines.Count; ++index)
        {
            var y1 = Offset + (float)(index + 1) * Width;
            var screen1 = graph.GraphSpaceToScreen(new PointF(0f, y1));
            if (screen1.Y < 0 || screen1.Y > drawBounds.Height - graph.MarginBottom)
                continue;
            if (Timelines[index].Intervals.Count != 0)
                DrawTimeLine(graph, screen1.Y, Timelines[index]);
            if (DrawLabels && !string.IsNullOrWhiteSpace(Timelines[index].Name))
                graph.AxisY.DrawAxisLabel(graph, screen1.Y, Timelines[index].Name);
        }
    }

    protected virtual GraphCellToRender AdjustColor(
        GraphCellToRender graphCellToRender)
    {
        if (OverrideBarColor.HasValue)
            graphCellToRender.Color = OverrideBarColor;
        return graphCellToRender;
    }

    public void Tick(int pid, int clock)
    {
        if (!Timelines.Exists(time => time.PId == pid))
            Timelines.Add(new Timeline { PId = pid });

        foreach (var timeline in Timelines) timeline.Tick(pid, clock);
    }

    public void DrawTimeLine(GraphView graph, int height, Timeline beingDrawn)
    {
        var graphCellToRender = AdjustColor(beingDrawn.Fill);
        if (graphCellToRender.Color.HasValue)
            Application.Driver.SetAttribute(graphCellToRender.Color.Value);
        foreach (var interval in beingDrawn.Intervals)
        {
            var left = graph.GraphSpaceToScreen(new PointF(Math.Min(graph.Bounds.Width - 1, interval.Left), height));
            var right = graph.GraphSpaceToScreen(new PointF(Math.Min(graph.Bounds.Width - 1, interval.Right), height));
            graph.DrawLine(left, right, graphCellToRender.Rune);
        }

        graph.SetDriverColorToGraphColor();
    }

    public class Timeline
    {
        public int PId { get; set; }
        public string Name { get; set; }

        /// <summary>
        ///     The color and character that will be rendered in the console
        ///     when the bar extends over it
        /// </summary>
        public GraphCellToRender Fill { get; set; } =
            new(' ', new Attribute(Color.White, Color.Black));

        public List<Interval> Intervals { get; set; } = new();

        public void Tick(int pid, int clock)
        {
            if (pid != PId)
                return;
            if (Intervals.Count == 0)
            {
                Intervals.Add(new Interval { Left = clock, Right = clock });
            }
            else
            {
                if (Intervals.Last().Right == clock - 1)
                    Intervals.Last().Right = clock;
                else
                    Intervals.Add(new Interval { Left = clock, Right = clock });
            }
        }

        public class Interval
        {
            public int Left;
            public int Right;
        }
    }
}