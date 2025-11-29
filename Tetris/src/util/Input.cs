using SDL2;

class KeyInput
{
    private static Dictionary<SDL.SDL_Keycode, bool> pressedKeys = [];

    public KeyInput()
    {
        if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
        {
            Console.WriteLine("SDL init failed: " + SDL.SDL_GetError());
            return;
        }

        IntPtr window = SDL.SDL_CreateWindow(
            "Tetris",
            SDL.SDL_WINDOWPOS_CENTERED,
            SDL.SDL_WINDOWPOS_CENTERED,
            800, 600,
            SDL.SDL_WindowFlags.SDL_WINDOW_HIDDEN
        );

        if (window == IntPtr.Zero)
        {
            Log.Add("Window creation failed: " + SDL.SDL_GetError());
            SDL.SDL_Quit();
            return;
        }

        bool running = true;
        while (running)
        {
            // 1) Poll SDL events
            while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
            {
                switch (e.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                        running = false;
                        break;

                    case SDL.SDL_EventType.SDL_KEYDOWN:
                        pressedKeys[e.key.keysym.sym] = true;
                        break;

                    case SDL.SDL_EventType.SDL_KEYUP:
                        pressedKeys[e.key.keysym.sym] = false;
                        break;
                }
            }

            SDL.SDL_Delay(16); // crude ~60 FPS cap
        }

        SDL.SDL_DestroyWindow(window);
        SDL.SDL_Quit();
    }

    public static string[] ReadAll()
    {
        // return [];
        return [.. pressedKeys.Where(kvp => kvp.Value).Select(kvp => kvp.Key.ToString())];
    }
}