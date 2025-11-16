using System.Timers;
namespace ConsoleApp29
{
    public enum AuctionState { ChangedPrice, Ended, Started }

    public class AuctionItem
    {


        //Product Info
        public int Id { get; set; }
        public string Name { get; set; }
        public AuctionState State { get; set; }
        public System.Timers.Timer Timer { get; set; }
        public event TimerCallback AuctionEnded;
        public event PriceChangedEvent PriceChanged;
        //Price field 
        public decimal Start_price { get; set; }
        public decimal Current_price { get; set; }
        public decimal? End_price { get; set; }
        public readonly object _lock;
        //Person info
        public Person Seller { get; set; }
        public Person? Buyer { get; set; }
        public List<Person> Bidders { get; set; }
        public AuctionItem(int id, string name, AuctionState state, int start_price, int current_price, int end_price, Person seller, Person buyer, System.Timers.Timer timer)
        {
            Id = id;
            Name = name;
            State = state;
            Start_price = start_price;
            Current_price = current_price;
            End_price = end_price;
            Seller = seller;
            Buyer = buyer;
            Timer = timer;
        }
        //Create for Auc
        public AuctionItem(int id, string name, AuctionState state, decimal start_price, Person seller, TimeSpan duration)
        {
            Id = id;
            Name = name;
            State = state;
            Start_price = start_price;
            Current_price = start_price;
            End_price = null;
            Seller = seller;
            Buyer = null;

            Timer = new System.Timers.Timer(duration.TotalMilliseconds);
            Timer.AutoReset = false;
            Timer.Elapsed += (sender, e) =>
            {
                State = AuctionState.Ended;
                AuctionEnded?.Invoke(sender);
            };
            Timer.Start();
        }
        public bool setPrice(Person person, decimal price)
        {
            lock (_lock)
            {
                if (price < Current_price) return false;
                if (!Bidders.Contains(person)) Bidders.Add(person);
                Current_price = price;

                PriceChanged?.Invoke(Current_price);

                return true;
            }
        }

    }

}