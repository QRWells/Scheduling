using Simulator;
using Simulator.Schedulers;
using Terminal.Gui;

Application.Run<App>();

Console.WriteLine("Hello, World!");

var os = new Os();
var fifo = new FcfsScheduler();
os.SetSchedule(fifo);

var p1 = new Process(0);
p1.AddTask(10,TaskType.CpuBounding);

os.AddProcess(ref p1);

os.Step();