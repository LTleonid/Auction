using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp29
{
    internal class Program
    {
        static AuctionHouse auctionHouse = new AuctionHouse();
        static Person me = new Person { Id = 1, Name = "Player" };

        static int selectedIndex = 0;

        static List<AuctionItem> activeAuctions = new();
        static List<AuctionItem> myBids = new();

        static List<string> notifications = new();
        const int maxNotifications = 10;

        static async Task Main()
        {
            Console.CursorVisible = false;

            await auctionHouse.PlaceBidAsync(me, 100, "Laptop", TimeSpan.FromMinutes(5));
            await auctionHouse.PlaceBidAsync(me, 500, "iPhone", TimeSpan.FromMinutes(4));
            await auctionHouse.PlaceBidAsync(me, 50, "Mouse", TimeSpan.FromMinutes(3));
            await auctionHouse.PlaceBidAsync(me, 200, "Keyboard", TimeSpan.FromMinutes(6));

            me.AuctionEndedNotification += NotificationHandler;
            me.PriceChangedNotification += NotificationHandler;

            while (true)
            {
                activeAuctions = (List<AuctionItem>)await AuctionHouse.GetActiveAuctions();
                DrawUI();

                var key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = Math.Max(0, selectedIndex - 1);
                        break;

                    case ConsoleKey.DownArrow:
                        selectedIndex = Math.Min(activeAuctions.Count - 1, selectedIndex + 1);
                        break;

                    case ConsoleKey.Spacebar:
                        if (activeAuctions.Count > 0)
                            await OpenBidWindow(activeAuctions[selectedIndex]);
                        break;

                    case ConsoleKey.N:
                        await CreateAuctionWindow();
                        break;

                    case ConsoleKey.Escape:
                        Console.Clear();
                        Console.CursorVisible = true;
                        return;
                }
            }
        }
        #region UI
        
        #region Render
        static void DrawUI()
        {
            Console.Clear();
            int Center = Console.WindowWidth / 2;

            Console.SetCursorPosition(2, 1);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("[ Active Auctions ]");
            Console.ResetColor();

            for (int i = 0; i < activeAuctions.Count; i++)
            {
                var a = activeAuctions[i];
                Console.SetCursorPosition(2, 3 + i);

                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.White;
                }

                Console.Write($"[{a.Id}] {a.Name} | {a.Current_price}$ ");

                Console.ResetColor();
            }

            Console.SetCursorPosition(Center + 2, 1);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[ My Bids ]");
            Console.ResetColor();

            int line = 3;
            foreach (var a in myBids)
            {
                Console.SetCursorPosition(Center + 2, line++);
                Console.Write($"[{a.Id}] {a.Name} | {a.Current_price}$");
            }
            DrawNotifications();
            DrawShortKeys();
            
        }

        static void DrawShortKeys()
        {
            string Text = "Space: Choice | N: Create bid | Arrows: controlling | ESC: Return";
            int w = Console.WindowWidth;
            int startX = (Console.WindowWidth - Text.Length) /2;
            int startY = Console.WindowHeight - 1;
            
            Console.ResetColor();
            Console.BackgroundColor = ConsoleColor.DarkGray;

            Console.SetCursorPosition(0, startY);
            Console.Write(" ".PadRight(w));
            Console.SetCursorPosition(startX, startY);

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;

            Console.Write(Text);


            Console.ResetColor();
        }

        static void DrawNotifications()
        {
            int w = 56;
            int startX = Console.WindowWidth -w - 2;
            int startY = Console.WindowHeight - maxNotifications - 3;

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.SetCursorPosition(startX, startY);
            Console.Write("[ Notifications ]");
            Console.ResetColor();

            var last = notifications.Count > maxNotifications ? notifications.GetRange(notifications.Count - maxNotifications, maxNotifications) : notifications;

            int y = startY + 2;
            foreach (var msg in last)
            {
                Console.SetCursorPosition(startX, y++);
                Console.Write(msg.PadRight(w));
            }
        }
        #endregion
        
        #region Notification
        static void AddNotification(string msg)
        {
            notifications.Add($"{DateTime.Now:HH:mm:ss} | {msg}");

            if (notifications.Count > 100)
                notifications.RemoveAt(0);
        }

        static void NotificationHandler(object? sender, string msg)
        {
            AddNotification(msg);
        }
        #endregion
       
        #region UserControlling
        static async Task OpenBidWindow(AuctionItem item)
        {
            decimal? val = ShowNumericInputWindow(
                title: $"Bid for: {item.Name}",
                label: $"Current price: {item.Current_price}$\nYour bid: "
            );

            if (val == null) return;

            var res = await auctionHouse.MakeBid(me, item.Id, (int)val);

            if (!myBids.Contains(item))
                myBids.Add(item);

            AddNotification($"Bid result: {res}");
        }

        static async Task CreateAuctionWindow()
        {
            string? name = ShowTextInput("Enter auction name:");
            if (name == null) return;

            decimal? price = ShowNumericInputWindow(
                title: "Starting price",
                label: "Enter start price: "
            );
            if (price == null) return;

            decimal? minutes = ShowNumericInputWindow(
                title: "Duration (min)",
                label: "Enter minutes: "
            );
            if (minutes == null) return;

            await auctionHouse.PlaceBidAsync(me, price.Value, name, TimeSpan.FromMinutes((double)minutes));
            AddNotification($"Created auction '{name}'");
        }
        #endregion
        
        #region WindowInput

        static string? ShowTextInput(string title)
        {
            return ShowInputWindow(title, acceptDigits: false);
        }

        static decimal? ShowNumericInputWindow(string title, string label)
        {
            string? res = ShowInputWindow($"{title}\n{label}", acceptDigits: true);
            if (res == null) return null;

            if (decimal.TryParse(res, out var n))
                return n;

            AddNotification("Invalid number");
            return null;
        }

        static string? ShowInputWindow(string title, bool acceptDigits)
        {
            int w = 50, h = 7;
            int x = (Console.WindowWidth - w) / 2;
            int y = (Console.WindowHeight - h) / 2;

            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Gray;

            for (int i = 0; i < h; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write(new string(' ', w));
            }

            Console.SetCursorPosition(x + 2, y + 1);
            Console.Write(title);
            Console.ResetColor();
            Console.CursorVisible = true;

            string input = "";
            while (true)
            {
                Console.SetCursorPosition(x + 2, y + 4);
                Console.Write(new string(' ', w - 4));
                Console.SetCursorPosition(x + 2, y + 4);
                Console.Write(input);

                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Enter)
                    break;
                if (key.Key == ConsoleKey.Escape)
                {
                    Console.CursorVisible = false;
                    return null;
                }
                if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input = input[..^1];
                    continue;
                }
                if (!acceptDigits || char.IsDigit(key.KeyChar))
                    input += key.KeyChar;
            }

            Console.CursorVisible = false;
            return input;
        }
        #endregion
        
        #endregion
    }
}
