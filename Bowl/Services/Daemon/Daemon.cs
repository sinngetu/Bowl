using Serilog;

namespace Bowl.Services.Daemon
{
    public static class Daemon
    {
        static List<Thread> pools = [];
        static readonly List<Action> tasks = [
            LocalSocket.Service.Start,
        ];

        static Thread ThreadStart(Action action)
        {
            Thread thread = new Thread(new ThreadStart(action));
            thread.Start();
            return thread;
        }

        static void Threads()
        {
            // Startup
            foreach(var task in tasks)
                pools.Add(ThreadStart(task));

            while (true)
            {
                try
                {
                    for(int i = 0; i < pools.Count; i++)
                    {
                        Thread thread = pools[i];

                        // Check if the thread is alive
                        if (!thread.IsAlive)
                        {
                            // When the thread isn't alive, we need to restart
                            Action action = tasks[i];
                            pools[i] = ThreadStart(action);
                        }
                    }

                    // Check interval is 30s, due to now there is a data reciever service, and min interval is 1m
                    // If the number of threads is too large and the types of business are too different
                    // We should consider using a event to manage
                    Thread.Sleep(30000);
                }
                catch (Exception ex) {
                    Log.Error(ex.ToString());
                }
            }
        }

        public static Thread Start()
        {
            return new Thread(new ThreadStart(Threads));
        }
    }
}
