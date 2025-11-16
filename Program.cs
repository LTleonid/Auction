namespace ConsoleApp29
{
    public delegate void PriceChangedEvent(decimal price);

    internal class Program
    {
        public static async Task Main()
        {
            var auction = new AuctionHouse();
            Person person = new Person();
            Person person1 = new Person();
            await Task.Run(async () => await auction.PlaceBidAsync(person, 100, "asd", new TimeSpan(0, 1, 0)));
            var Alist = await Task.Run(async () => await AuctionHouse.GetActiveAuctions());
            foreach(var item in Alist)
            {
                Console.WriteLine($"{item.Id} | {item.Name}");
            }

        }

    }
}

