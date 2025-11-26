using System.Net;
using System.Text;
using System.Net.WebSockets;

class KeyInput
{
    private readonly List<string> currentKeys = []; // Last batch of pressed keys
    private readonly Lock keyLock = new(); // For thread safety on ReadAll method

    public KeyInput()
    {
        // Create socket listener
        HttpListener listener = new();
        listener.Prefixes.Add("http://localhost:1337/");
        listener.Start();

        // Server src/util/web-input/index.html and WebSocket input
        Task.Run(async () =>
        {
            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                try
                {
                    if (request == null || response == null)
                    {
                        continue;
                    }

                    // If client requests WebSocket upgrade, accept and handle messages
                    if (request.IsWebSocketRequest)
                    {
                        HttpListenerWebSocketContext wsContext = await context.AcceptWebSocketAsync(subProtocol: null);
                        WebSocket ws = wsContext.WebSocket;

                        var wsBuffer = new byte[4096];
                        try
                        {
                            while (ws.State == WebSocketState.Open)
                            {
                                var result = await ws.ReceiveAsync(new ArraySegment<byte>(wsBuffer), CancellationToken.None);
                                if (result.MessageType == WebSocketMessageType.Close)
                                {
                                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                                    break;
                                }

                                int count = result.Count;
                                // Handle multi-frame messages
                                while (!result.EndOfMessage)
                                {
                                    if (count >= wsBuffer.Length)
                                    {
                                        // message too large, drop
                                        break;
                                    }
                                    result = await ws.ReceiveAsync(new ArraySegment<byte>(wsBuffer, count, wsBuffer.Length - count), CancellationToken.None);
                                    count += result.Count;
                                }

                                if (result.MessageType == WebSocketMessageType.Text)
                                {
                                    string payload = Encoding.UTF8.GetString(wsBuffer, 0, count);
                                    string[] keys = payload.Split(',', StringSplitOptions.RemoveEmptyEntries);
                                    lock (keyLock)
                                    {
                                        currentKeys.Clear();
                                        currentKeys.AddRange(keys);
                                    }
                                }
                            }
                        }
                        catch
                        {
                            try { ws.Abort(); } catch { }
                        }

                        continue;
                    }

                    // Serve static files for GET requests
                    string localPath = request.Url!.LocalPath;
                    if (localPath == "/") localPath = "Tetris/src/util/web-input/input.html";
                    if (localPath == "/input.js") localPath = "Tetris/src/util/web-input/input.js";
                    string relPath = Uri.UnescapeDataString(localPath).TrimStart('/');
                    string filePath = Path.GetFullPath(relPath);

                    if (!File.Exists(filePath))
                    {
                        response.StatusCode = 404;
                        byte[] notFound = Encoding.UTF8.GetBytes($"404 - Not Found (Local path: {filePath})");
                        response.ContentType = "text/plain; charset=utf-8";
                        response.ContentLength64 = notFound.Length;
                        response.OutputStream.Write(notFound, 0, notFound.Length);
                        response.Close();
                        continue;
                    }

                    string ext = Path.GetExtension(filePath).ToLowerInvariant();
                    string contentType = ext switch
                    {
                        ".html" => "text/html; charset=utf-8",
                        ".htm" => "text/html; charset=utf-8",
                        ".js" => "application/javascript; charset=utf-8",
                        ".css" => "text/css; charset=utf-8",
                        ".json" => "application/json; charset=utf-8",
                        ".png" => "image/png",
                        ".jpg" or ".jpeg" => "image/jpeg",
                        ".svg" => "image/svg+xml",
                        ".gif" => "image/gif",
                        _ => "application/octet-stream"
                    };

                    byte[] fileBuffer = File.ReadAllBytes(filePath);
                    response.ContentType = contentType;
                    response.ContentLength64 = fileBuffer.Length;
                    response.OutputStream.Write(fileBuffer, 0, fileBuffer.Length);
                    response.Close();
                }
                catch
                {
                    try { response.StatusCode = 500; response.Close(); } catch { }
                }
            }
        });
    }

    public string[] ReadAll()
    {
        lock (keyLock)
        {
            return [.. currentKeys];
        }
    }
}
