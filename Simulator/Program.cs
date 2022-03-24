using Simulator;
using Simulator.Schedulers;

Console.WriteLine("Hello, World!");

var fifo = new FcfsScheduler();
var os = new Os(fifo);

os.Run();