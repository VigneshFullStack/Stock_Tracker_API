using Microsoft.AspNetCore.Http;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StockTracker.API
{
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;

        public WebSocketMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                await HandleWebSocketConnection(webSocket);
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }

        private async Task HandleWebSocketConnection(WebSocket webSocket)
        {
            try
            {
                // Continuously send mock stock data to connected clients
                Random random = new Random();
                while (webSocket.State == WebSocketState.Open)
                {
                    var stockData = GenerateRandomStockData();
                    var bytes = Encoding.UTF8.GetBytes(stockData);
                    await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                    await Task.Delay(TimeSpan.FromSeconds(1)); // Simulate stock update every second
                }
            }
            catch (Exception ex)
            {
                // Log or handle any exceptions that occur during WebSocket communication
                Console.WriteLine($"WebSocket communication error: {ex.Message}");
            }
            finally
            {
                if (webSocket.State != WebSocketState.Closed)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "WebSocket connection closed", CancellationToken.None);
                }
            }
        }

        private string GenerateRandomStockData()
        {
            // Example: Generate random stock data with price and timestamp
            var random = new Random();
            var price = random.Next(100, 1000);
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return $"{{\"price\":{price},\"timestamp\":\"{timestamp}\"}}";
        }
    }
}
