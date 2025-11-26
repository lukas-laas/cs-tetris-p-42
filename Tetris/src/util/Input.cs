using System.Net;
using System.Text;

class KeyInput
{
    public KeyInput()
    {
        // Create web server on localhost:1337
        HttpListener listener = new();
        listener.Prefixes.Add("http://localhost:1337/");
        listener.Start();

        // Server src/util/web-input/index.html
        Task.Run(() =>
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

                    byte[] buffer = File.ReadAllBytes(filePath);
                    response.ContentType = contentType;
                    response.ContentLength64 = buffer.Length;
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                    response.Close();
                }
                catch
                {
                    try { response.StatusCode = 500; response.Close(); } catch { }
                }
            }
        });
    }

    public string? Read()
    {
        return null;
    }
}
