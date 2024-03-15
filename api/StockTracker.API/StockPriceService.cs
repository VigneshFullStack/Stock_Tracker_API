using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StockTracker.API
{
    public static class StockPriceService
    {
        public static async Task ListenToStockChanges(WebSocket webSocket)
        {
            // Continuously send mock stock data to connected clients
            Random random = new Random();
            while (webSocket.State == WebSocketState.Open)
            {
                var stockData = GenerateRandomStockData();
                var bytes = Encoding.UTF8.GetBytes(stockData);
                await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                await Task.Delay(TimeSpan.FromSeconds(5)); // Simulate stock update every 5 seconds
            }
        }

        private static string GenerateRandomStockData()
        {
            // Generate random stock data with price and timestamp
            var random = new Random();
            var price = random.Next(100, 1000);
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return $"{{\"price\":{price},\"timestamp\":\"{timestamp}\"}}";
        }
    }
}
