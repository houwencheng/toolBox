public class MutThread
{
    private Task<int> RunCalacTask()
    {
        return Task.Run<int>(() =>
        {
            PrintThreadID("B");
            System.Threading.Thread.Sleep(5000);
            PrintThreadID("B2");
            // mutex?.ReleaseMutex();
            eventWaitHandle?.Set();
            PrintThreadID("B3");
            return 200;
        });
    }

    // private async Task<int> RunCalacAsync()
    // {
    //     return await RunCalacTask();
    // }

    System.Threading.Mutex? mutex;
    System.Threading.EventWaitHandle? eventWaitHandle;
    System.Threading.Thread th;
    public async void Test()
    {

        th = new Thread(new ParameterizedThreadStart(ThreadJob));
        th.Start();
        // System.Threading.Thread.CurrentThread.SetApartmentState(ApartmentState.STA);
        // mutex = new Mutex(true);
        eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        PrintThreadID("A");
        await RunCalacTask();
        PrintThreadID("A2");
        // mutex.WaitOne();
        eventWaitHandle.WaitOne();
        PrintThreadID("A3");

        th.Resume();
        PrintThreadID("A4");
        //A,A2,B,B2,B3,A3
    }

    private void ThreadJob(object? o)
    {
        PrintThreadID("C");
        System.Threading.Thread.CurrentThread.Suspend();
        PrintThreadID("C2");
    }

    private void PrintThreadID(string flag)
    {
        Console.WriteLine("ThreadID_{0}:{1}", flag, System.Threading.Thread.CurrentThread.ManagedThreadId);
    }
}