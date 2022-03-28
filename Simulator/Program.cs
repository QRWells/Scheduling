using Simulator;
using Simulator.Schedulers;

var os = new Os();
var fifo = new FcfsScheduler();
os.SetSchedule(fifo);

var p1 = new Process(0);
p1.AddTask(10);

os.AddProcess(p1);

os.Step();