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
            item.AuctionEnded += (sender, e) =>
            {
                ShowNotification(item, $"Auction '[{item.Id}]|{item.Name}' ended");
            };

            item.PriceChanged += (sender, newPrice) =>
            {
                ShowNotification(item, $"New price: {newPrice}");
            };
        }


        public void ShowNotification(AuctionItem sender, string message)
        {
            Console.WriteLine($"[Person {Id}] | [{sender.Name}] {message}");
        }

    }
}