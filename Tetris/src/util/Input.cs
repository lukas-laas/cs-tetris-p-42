using SDL2;

class KeyInput
{
    private readonly Dictionary<KeyCode, bool> pressedKeys = [];
    private readonly Lock pressedKeysLock = new();
    private CancellationTokenSource? cts;
    private Task? eventLoopTask;

    public KeyInput()
    {
        if (eventLoopTask != null && !eventLoopTask.IsCompleted)
        {
            Log.Add("KeyInput event loop already running.");
            return;
        }

        cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;
        eventLoopTask = Task.Run(() => RunEventLoop(token), token);
    }

    private void RunEventLoop(CancellationToken cancellationToken)
    {
        if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
        {
            Log.Add("SDL init failed: " + SDL.SDL_GetError());
            return;
        }

        IntPtr window = SDL.SDL_CreateWindow(
            "Tetris",
            SDL.SDL_WINDOWPOS_CENTERED,
            SDL.SDL_WINDOWPOS_CENTERED,
            100, 100,
            SDL.SDL_WindowFlags.SDL_WINDOW_HIDDEN
        );

        if (window == IntPtr.Zero)
        {
            SDL.SDL_Quit();
            throw new Exception("Window creation failed: " + SDL.SDL_GetError());
        }

        bool running = true;
        while (running && !cancellationToken.IsCancellationRequested)
        {
            while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
            {
                switch (e.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                        running = false;
                        break;

                    case SDL.SDL_EventType.SDL_KEYDOWN:
                        lock (pressedKeysLock)
                        {
                            pressedKeys[e.key.keysym.sym] = true;
                        }
                        break;

                    case SDL.SDL_EventType.SDL_KEYUP:
                        lock (pressedKeysLock)
                        {
                            pressedKeys[e.key.keysym.sym] = false;
                        }
                        break;
                }
            }

            SDL.SDL_Delay(10);
        }

        SDL.SDL_DestroyWindow(window);
        SDL.SDL_Quit();
    }

    public KeyCode[] ReadAll()
    {
        lock (pressedKeysLock)
        {
            return [.. pressedKeys
                .Where(kvp => kvp.Value)
                .Select(kvp => kvp.Key)
            ];
        }
    }

    public async Task StopAsync()
    {
        if (cts == null || eventLoopTask == null)
        {
            Log.Add("KeyInput event loop not running when attempting to stop.");
            return;
        }

        try
        {
            cts.Cancel();
            await eventLoopTask.ConfigureAwait(false);
        }
        catch (OperationCanceledException) { }
        finally
        {
            cts.Dispose();
            cts = null;
            eventLoopTask = null;
        }
    }
}