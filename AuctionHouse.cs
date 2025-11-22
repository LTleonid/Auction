namespace ConsoleApp29
{
    public struct BidResult
    {
        public AuctionState State;
        public decimal Start_price;
        public System.Timers.Timer timer;

        public BidResult(AuctionState state, decimal start_price, System.Timers.Timer timer)
        {
            State = state;
            Start_price = start_price;
            this.timer = timer;
        }
    }

    public class AuctionHouse
    {
        private static List<AuctionItem> Auctions = new();
        public readonly static ReaderWriterLockSlim Reader = new ReaderWriterLockSlim();
        public readonly static object _BidLock;

        public async Task<BidResult> PlaceBidAsync(Person player, decimal price, string name, TimeSpan duration)
        {
            return await Task.Run(() =>
            {
                Reader.EnterWriteLock();
                try
                {
                    var index = Auctions.Count;

                    var item = new AuctionItem(index, name, AuctionState.Started, price, player, duration);

                    Auctions.Add(item);

                    return new BidResult(item.State, item.Start_price, item.Timer);
                }
                finally
                {
                    Reader.ExitWriteLock();
                }
            });
        }

        public async Task<string> MakeBid(Person person, int id, int price)
        {
            var AList = await Task.Run(GetActiveAuctions);
            var item = AList[id];

            if (!item.setPrice(person, price))
                return "need more";

            return "Success";
        }

        public static async Task<IReadOnlyList<AuctionItem>> GetActiveAuctions()
        {
            Reader.EnterReadLock();
            try
            {
                return Auctions.Where(x => x.State == AuctionState.Started).ToList();
            }
            finally { Reader.ExitReadLock(); }
        }
    }

}