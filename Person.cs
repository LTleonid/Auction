namespace ConsoleApp29
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public event EventHandler<string> AuctionEndedNotification;
        public event EventHandler<string> PriceChangedNotification;

        public void SubscribeForEvent(AuctionItem item)
        {
            item.AuctionEnded += (state) =>
            {
                AuctionEndedNotification?.Invoke(this, $"Auction '[{item.Id}]|{item.Name}' Ended!");
            };

            item.PriceChanged += (newPrice) =>
            {
                PriceChangedNotification?.Invoke(this, $"Change Price in '[{item.Id}]|{item.Name}': {newPrice}");
            };

        }

        public void ShowNotification(AuctionItem sender, string message)
        {
            Console.WriteLine($"[{sender.Name}] {message}");
        }
    }
}