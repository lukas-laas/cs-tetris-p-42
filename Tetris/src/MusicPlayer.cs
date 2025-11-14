using SDL2;
using System;

class Mp3LoopPlayer
{
    private IntPtr music;

    public Mp3LoopPlayer(string filePath)
    {
        // Initialize SDL audio
        if (SDL.SDL_Init(SDL.SDL_INIT_AUDIO) < 0)
            throw new Exception("SDL_Init failed");

        // Init SDL_mixer for MP3 support
        if (SDL_mixer.Mix_OpenAudio(44100, SDL_mixer.MIX_DEFAULT_FORMAT, 2, 2048) < 0)
            throw new Exception("Mix_OpenAudio failed");

        // Load MP3 file
        music = SDL_mixer.Mix_LoadMUS(filePath);
        if (music == IntPtr.Zero)
            throw new Exception("Failed to load music: " + SDL.SDL_GetError());

        // Play loop: -1 means infinite loop
        SDL_mixer.Mix_PlayMusic(music, -1);
    }

    public void Stop()
    {
        SDL_mixer.Mix_HaltMusic();
        SDL_mixer.Mix_FreeMusic(music);
        SDL.SDL_Quit();
    }
}