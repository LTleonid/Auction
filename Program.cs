namespace ConsoleApp29
{
    internal class Program
    {
        public static async Task Main()
        {
            var auction = new AuctionHouse();

            Person seller = new Person { Id = 1, Name = "A" };
            Person p1 = new Person { Id = 2, Name = "B" };
            Person p2 = new Person { Id = 3, Name = "C" };

            var result = await auction.PlaceBidAsync(seller,100,"Laptop",TimeSpan.FromSeconds(10));
            var auctions = await AuctionHouse.GetActiveAuctions();
            var item = auctions[0];
            foreach (var auc in auctions)
                Console.WriteLine($"[{auc.Id}] | {auc.Name}");

            Console.WriteLine(await auction.MakeBid(p1, item.Id, 150));
            Console.WriteLine(await auction.MakeBid(p2, item.Id, 200));

            var a = await AuctionHouse.GetActiveAuctions();

            await Task.Delay((int)a[0].Timer.Interval);  //Ждём конец аукциона, я не хочу цикл тут

        }
    }
}
