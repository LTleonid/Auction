namespace ConsoleApp29
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public event EventHandler<string>? AuctionEndedNotification;
        public event EventHandler<string>? PriceChangedNotification;

        public void SubscribeForEvent(AuctionItem item)
        {
            item.AuctionEnded += async (sender, e) =>
            {
                await Task.Run(() =>
                {
                    ShowNotification(item, $"Auction '[{item.Id}]|{item.Name}' ended", false);
                });
            };

            item.PriceChanged += async (sender, newPrice) =>
            {
                await Task.Run(() =>
                {
                    ShowNotification(item, $"New price: {newPrice}", true);
                });
            };
        }

        public void UnSubscribeForEvent(AuctionItem item)
        {
            item.AuctionEnded -= (sender, e) =>
            {
                ShowNotification(item, $"Auction '[{item.Id}]|{item.Name}' ended", false);
            };
            item.PriceChanged -= (sender, newPrice) =>
            {
                ShowNotification(item, $"New price: {newPrice}", true);
            };
        }

        public void ShowNotification(AuctionItem sender, string message, bool isPriceChange)
        {
            if (isPriceChange)
            {
                PriceChangedNotification?.Invoke(this, message);
            }
            else
            {
                AuctionEndedNotification?.Invoke(this, message);
            }
        }
    }
}
