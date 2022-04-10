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

using System.Globalization;
using System.Text;
using NStack;
using Simulator.Schedulers;
using Terminal.Gui;

namespace Simulator.UI;

public sealed class App : Toplevel
{
    private readonly GraphView _graphView = new()
        { X = 0, Y = 0, Width = Dim.Fill(), Height = Dim.Fill(), ColorScheme = Colors.TopLevel };

    private readonly Os _os = new();
    private readonly ProcessDataTable _processes = new();
    private readonly TableView _processTableView;
    private readonly TimeLineSeries _timeLineSeries = new();

    public App()
    {
        ColorScheme = Colors.TopLevel;
        _os.SetScheduler(new FCFSScheduler());
        _processTableView = new TableView(_processes) { X = 0, Y = 0, Width = Dim.Fill(), Height = Dim.Percent(90) };

        void Reset(IScheduler scheduler)
        {
            _os.Reset();
            _os.SetScheduler(scheduler);
            _processes.ResetInfo();
        }

        var menu = new MenuBar(new[]
        {
            new MenuBarItem("_Scheduler", new[]
            {
                new MenuItem("_FCFS", "", () => Reset(new FCFSScheduler()), null, null,
                    Key.ShiftMask | Key.F),
                new MenuItem("_SJF", "", () => Reset(new SJFScheduler()), null, null,
                    Key.ShiftMask | Key.D),
                new MenuItem("_STCF", "", () => Reset(new STCFScheduler()), null, null,
                    Key.ShiftMask | Key.T),
                new MenuItem("_RR", "", () => Reset(new RoundRobinScheduler()), null, null,
                    Key.ShiftMask | Key.R),
                new MenuItem("_MLFQ", "", () => Reset(new MLFQScheduler()), null, null,
                    Key.ShiftMask | Key.M)
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
            new(Key.S, "~S~ Step", Step),
            new StatusItem(Key.CtrlMask | Key.A, "~Ctrl-A~ Add Process", AddProcess),
            new StatusItem(Key.CtrlMask | Key.Q, "~Ctrl-Q~ Quit", () =>
            {
                if (Quit()) RequestStop();
            })
        });

        Add(statusBar);

        _graphView.ScrollOffset = new PointF(0, _graphView.Bounds.Height);

        _graphView.MarginLeft = 5;
        _graphView.MarginBottom = 2;
        _graphView.AxisX.Text = "Tick";
        _graphView.AxisX.Increment = 10;
        _graphView.AxisX.ShowLabelsEvery = 1;

        _graphView.AxisY.Text = "Process Id";
        _graphView.AxisY.Increment = _timeLineSeries.Width;
        _graphView.AxisY.LabelGetter = i => (i.Value / _timeLineSeries.Width).ToString(CultureInfo.CurrentCulture);
        _graphView.Series.Add(_timeLineSeries);

        var tab = new TabView { X = 0, Y = 0, Width = Dim.Fill(), Height = Dim.Fill(), ColorScheme = Colors.TopLevel };
        tab.AddTab(new TabView.Tab("Graph", _graphView), true);
        tab.AddTab(new TabView.Tab("SysInfo", _processTableView), false);

        var window = new Window
            { X = 0, Y = 0, Width = Dim.Fill(), Height = Dim.Fill(), ColorScheme = Colors.TopLevel };

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
        _timeLineSeries.Tick(_os.CurrentPid(), _os.Clock);
        _processes.Update();
        _processTableView.SetNeedsDisplay();
        _graphView.SetNeedsDisplay();
    }

    private void AddProcess()
    {
        var infoFrame = new FrameView("Process Options")
        {
            X = 0,
            Y = 0,
            Width = Dim.Percent(40),
            Height = 5
        };

        var nameLabel = new Label("Name: ")
            { X = 0, Y = 0, Width = 10, Height = 1, TextAlignment = TextAlignment.Left };
        var nameEdit = new TextField("")
        {
            X = Pos.Right(nameLabel) + 1,
            Y = Pos.Top(nameLabel),
            Width = 20,
            Height = 1
        };
        var arriveLabel = new Label("Arrive at:")
        {
            X = 0,
            Y = Pos.Bottom(nameLabel) + 1,
            Width = 10,
            Height = 1,
            TextAlignment = TextAlignment.Left
        };
        var arriveEdit = new TextField("0")
        {
            X = Pos.Right(arriveLabel) + 1,
            Y = Pos.Top(arriveLabel),
            Width = 20,
            Height = 1
        };
        arriveEdit.TextChanging += args =>
        {
            if (args.NewText.Length <= 0) return;
            if (!Unicode.IsNumber(args.NewText[0]))
                args.Cancel = true;
        };
        infoFrame.Add(arriveLabel, arriveEdit);
        infoFrame.Add(nameLabel, nameEdit);


        var frameTask = new FrameView("Tasks")
        {
            X = 0,
            Y = Pos.Bottom(infoFrame),
            Width = Dim.Fill(),
            Height = Dim.Percent(60)
        };
        var tasks = new List<Task>();
        var taskList = new ListView(tasks)
            { X = 1, Y = 1, Width = Dim.Percent(90), Height = Dim.Percent(90), AllowsMultipleSelection = false };
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
        timeEdit.TextChanging += args =>
        {
            if (args.NewText.Length <= 0) return;
            if (!Unicode.IsNumber(args.NewText[0]))
                args.Cancel = true;
        };
        var taskType = new[] { "CPU Bounding", "I/O Bounding" };
        var taskTypeList = new ListView(taskType)
        {
            X = 0,
            Y = Pos.Bottom(labelTime),
            Width = 15,
            Height = 10,
            SelectedItem = 0
        };
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
        addTask.Clicked += () =>
        {
            tasks.Add(new Task
            {
                Duration = int.Parse(Encoding.UTF8.GetString(timeEdit.Text.ToByteArray())),
                Type = cpu ? TaskType.CpuBounding : TaskType.IoBounding
            });
            taskList.SetNeedsDisplay();
        };
        var removeTask = new Button("_Remove", true)
        {
            X = Pos.Right(timeEdit) + 1,
            Y = Pos.Bottom(addTask),
            Width = 15,
            Height = 1,
            TextAlignment = TextAlignment.Left
        };
        removeTask.Clicked += () =>
        {
            if (tasks.Count == 0) return;
            tasks.RemoveAt(taskList.SelectedItem);
            taskList.SetNeedsDisplay();
        };

        var frameAdd = new FrameView("New Task")
        {
            X = Pos.Right(infoFrame) + 1,
            Y = 0,
            Width = Dim.Percent(40),
            Height = 5
        };

        frameAdd.Add(addTask, removeTask, labelTime, timeEdit, taskTypeList);

        var okButton = new Button("_Ok");
        okButton.Clicked += () =>
        {
            if (tasks.Count != 0)
            {
                var proc = new Process(int.Parse(Encoding.UTF8.GetString(arriveEdit.Text.ToByteArray())), tasks)
                    { Name = Encoding.UTF8.GetString(nameEdit.Text.ToByteArray()) };
                _os.AddProcess(proc);
                _processes.AddProcess(proc);
                _processTableView.SetNeedsDisplay();
                _timeLineSeries.AddProcess(proc);
            }

            Application.RequestStop();
        };

        var cancelButton = new Button("_Cancel");
        cancelButton.Clicked += () => Application.RequestStop();

        var dialog = new Dialog("Add Process", okButton, cancelButton);

        dialog.Add(infoFrame, frameAdd, frameTask);

        Application.Run(dialog);
    }
}