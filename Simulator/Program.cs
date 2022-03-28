using Simulator;
using Simulator.Schedulers;
using Task = Simulator.Task;

var os = new Os();
var sjf = new SjfScheduler();
os.SetSchedule(sjf);

var p1 = new Process().WithArriveTime().WithTasks(
    new Task { Duration = 40, Type = TaskType.CpuBounding });
var p2 = new Process().WithArriveTime(5).WithTasks(
    new Task { Duration = 30, Type = TaskType.CpuBounding });

os.AddProcess(p1, p2);

os.Step(70);

os.Step();