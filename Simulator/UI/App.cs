#region FileInfo

// Copyright (c) 2022 Wang Qirui. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
// This file is part of Project Simulator.
// File Name   : App.cs
// Author      : Qirui Wang
// Created at  : 2022/03/25 18:23
// Description :

#endregion

using Terminal.Gui;

namespace Simulator.UI;

public sealed class App : Toplevel
{
    private readonly GraphView _graphView = new() { X = Pos.Center(), Y = Pos.Center() };
    private readonly ListView _listView = new() { X = Pos.Center(), Y = Pos.Center() };
    private readonly Os _os = new();

    public App()
    {
        var menu = new MenuBar(new[]
        {
            new MenuBarItem("_Scheduler", new[]
            {
                new MenuItem("_FCFS", "", () => { }, null, null, Key.ShiftMask | Key.F),
                new MenuItem("_SJF", "", () => { }, null, null, Key.ShiftMask | Key.D),
                new MenuItem("_STCF", "", () => { }, null, null, Key.ShiftMask | Key.T),
                new MenuItem("_RR", "", () => { }, null, null, Key.ShiftMask | Key.R),
                new MenuItem("_MLFQ", "", () => { }, null, null, Key.ShiftMask | Key.M)
            }),
            new MenuBarItem("_Help", new[]
            {
                new MenuItem("_About", "",
                    () => MessageBox.Query("About UI Catalog", "Build with GUI.CS", "_Ok"),
                    null, null, Key.ShiftMask | Key.A)
            })
        });

        Add(menu);

        var statusBar = new StatusBar(new[]
        {
            new(Key.F1, "~S~ Step", Step),
            new StatusItem(Key.CtrlMask | Key.A, "~^A~ Add Process", AddProcess),
            new StatusItem(Key.CtrlMask | Key.Q, "~^Q~ Quit", () =>
            {
                if (Quit()) RequestStop();
            })
        });

        Add(statusBar);

        var tab = new TabView { X = 0, Y = 0, Width = Dim.Fill(), Height = Dim.Fill() };
        tab.AddTab(new TabView.Tab("Graph", _graphView), true);
        tab.AddTab(new TabView.Tab("SysInfo", _listView), false);

        var window = new Window { X = 0, Y = 0, Width = Dim.Fill(), Height = Dim.Fill() };

        Add(window);

        window.Add(tab);
    }

    private static bool Quit()
    {
        var n = MessageBox.Query(50, 7,
            "Quit Demo",
            "Are you sure you want to quit this demo?", "Yes", "No");
        return n == 0;
    }

    private void Step()
    {
        _os.Step();
        _graphView.Reset();

        _graphView.Series.Add(new GanttSeries(_os));
        _graphView.SetNeedsDisplay();
    }

    private void AddProcess()
    {
        var frame = new FrameView("Process Options")
        {
            X = 0,
            Y = 0,
            Width = Dim.Percent(20),
            Height = 5
        };

        var label = new Label("Arrive at:")
        {
            X = 0,
            Y = 0,
            Width = 10,
            Height = 1,
            TextAlignment = TextAlignment.Left
        };
        var widthEdit = new TextField("0")
        {
            X = Pos.Right(label) + 1,
            Y = Pos.Top(label),
            Width = 5,
            Height = 1
        };
        frame.Add(label);
        frame.Add(widthEdit);


        var frameTask = new FrameView("Tasks")
        {
            X = 0,
            Y = Pos.Bottom(frame),
            Width = Dim.Percent(80),
            Height = Dim.Percent(60)
        };
        var tasks = new List<Task>();
        var taskList = new ListView(tasks) { X = 0, Y = 11, Width = Dim.Percent(80), Height = Dim.Percent(50) };
        frameTask.Add(taskList);

        var labelTime = new Label("Time :")
        {
            X = 0,
            Y = 0,
            Width = 5,
            Height = 1,
            TextAlignment = TextAlignment.Left
        };
        var timeEdit = new TextField("10")
        {
            X = Pos.Right(labelTime) + 1,
            Y = Pos.Top(labelTime),
            Width = 15,
            Height = 1
        };
        var taskType = new[] { "CPU Bounding", "I/O Bounding" };
        var taskTypeList = new ListView(taskType)
        {
            X = 0,
            Y = Pos.Bottom(labelTime),
            Width = 15,
            Height = 10
        };
        taskTypeList.SelectedItem = 0;
        var cpu = false;
        taskTypeList.SelectedItemChanged += args => cpu = args.Item == 0;
        var addTask = new Button("_Add", true)
        {
            X = Pos.Right(timeEdit) + 1,
            Y = Pos.Top(timeEdit),
            Width = 15,
            Height = 1,
            TextAlignment = TextAlignment.Left
        };
        addTask.Clicked += () => { };
        var removeTask = new Button("_Remove", true)
        {
            X = Pos.Right(timeEdit) + 1,
            Y = Pos.Bottom(addTask),
            Width = 15,
            Height = 1,
            TextAlignment = TextAlignment.Left
        };

        var frameAdd = new FrameView("New Task")
        {
            X = Pos.Right(frame) + 1,
            Y = 0,
            Width = Dim.Percent(60),
            Height = 5
        };

        frameAdd.Add(addTask, removeTask, labelTime, timeEdit, taskTypeList);

        var okButton = new Button("_Ok");
        var cancelButton = new Button("_Cancel");
        cancelButton.Clicked += () => Application.RequestStop();

        var dialog = new Dialog("Add Process", okButton, cancelButton);

        dialog.Add(frame, frameAdd, frameTask);

        Application.Run(dialog);
        
        // TODO: task adding logic.
    }
}