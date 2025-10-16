using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop;

// Модели для проекции
public class ProductInfo
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string PriceCategory { get; set; } = string.Empty;
}

public class OrderSummary
{
    public int OrderId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public int ProductCount { get; set; }
}

abstract class Program
{
    static void Main()
    {
        try
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            CleanDatabase();
            CreateDatabase(); 
            DemonstrateOrderQueries();  
            DemonstrateFilter();
            DemonstrateProjection();
            DemonstrateSorting();
            DemonstrateExplicitJoins(); 
            DemonstrateGrouping();      
            DisplayOrdersWithDetails();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Всё, закончили!");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
        }
    }

    static void CleanDatabase()
    {
        using var db = new ShopContext();
        if (db.Database.CanConnect())
        {
            db.Database.EnsureDeleted();
            Console.WriteLine("База данных очищена!");
        }
    }
    
    static void CreateDatabase()
    {
        using var db = new ShopContext();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
        
        // Создание пользователей 
        var user1 = new User { Name = "Иван Петров" };
        var user2 = new User { Name = "Мария Сидорова" };
        var user3 = new User { Name = "Алексей Козлов" };
        var user4 = new User { Name = "Елена Малышева" };
        // Создание товаров
        var laptop = new Product { Name = "Ноутбук", Price = 50000 };
        var phone = new Product { Name = "Смартфон", Price = 30000 };
        var headphones = new Product { Name = "Наушники", Price = 5000 };
        var mouse = new Product { Name = "Мышь", Price = 1500 };
        var keyboard = new Product { Name = "Клавиатура", Price = 3500 };
        var monitor = new Product { Name = "Монитор", Price = 15000 };
        var tablet = new Product { Name = "Планшет", Price = 25000 };
        var camera = new Product { Name = "Фотоаппарат", Price = 45000 };
        var speaker = new Product { Name = "Колонка", Price = 7000 };
        var router = new Product { Name = "Роутер", Price = 3000 };
        var ssd = new Product { Name = "SSD накопитель", Price = 6000 };
        var powerbank = new Product { Name = "Power Bank", Price = 2500 };
        var printer = new Product { Name = "Принтер", Price = 9000 };
        var graphicsCard = new Product { Name = "Видеокарта", Price = 45000 };
        var processor = new Product { Name = "Процессор", Price = 22000 };
        var motherboard = new Product { Name = "Материнская плата", Price = 15000 };
        var ram = new Product { Name = "Оперативная память", Price = 8000 };
        var hdd = new Product { Name = "Жесткий диск", Price = 5000 };
        var smartwatch = new Product { Name = "Умные часы", Price = 12000 };
        var gamepad = new Product { Name = "Геймпад", Price = 4000 };
        var ups = new Product { Name = "ИБП", Price = 11000 };
        var microphone = new Product { Name = "Микрофон", Price = 7000 };
        var webcam = new Product { Name = "Веб-камера", Price = 4500 }; 
        
        // Создание заказов с разными данными
        var order1 = new Order 
        { 
            User = user1,
            OrderDate = DateTime.Now.AddDays(-2),
            PaymentMethod = "Карта",
            ShippingAddress = "Москва, ул. Ленина, 123",
            Status = "Delivered"
        };
        
        var order2 = new Order 
        { 
            User = user2,
            OrderDate = DateTime.Now.AddDays(-1),
            PaymentMethod = "Наличные", 
            ShippingAddress = "СПб, Невский пр., 45",
            Status = "Shipped"
        };
        
        var order3 = new Order 
        { 
            User = user1,
            OrderDate = DateTime.Now,
            PaymentMethod = "Онлайн",
            ShippingAddress = "Москва, ул. Пушкина, 67",
            Status = "Processing"
        };
        
        var order4 = new Order 
        { 
            User = user3,
            OrderDate = DateTime.Now.AddHours(-3),
            PaymentMethod = "Карта",
            ShippingAddress = "Казань, ул. Баумана, 89",
            Status = "Delivered"
        };
        
        order1.OrderItems.AddRange([
            new OrderItem { Product = laptop, Quantity = 1, UnitPrice = laptop.Price },
            new OrderItem { Product = mouse, Quantity = 1, UnitPrice = mouse.Price },
            new OrderItem { Product = keyboard, Quantity = 1, UnitPrice = keyboard.Price }
        ]);
        
        order2.OrderItems.AddRange([
            new OrderItem { Product = phone, Quantity = 1, UnitPrice = phone.Price },
            new OrderItem { Product = headphones, Quantity = 2, UnitPrice = headphones.Price }
        ]);
        
        order3.OrderItems.Add(new OrderItem 
        { 
            Product = headphones, 
            Quantity = 1, 
            UnitPrice = headphones.Price 
        });
        
        order4.OrderItems.AddRange([
            new OrderItem { Product = tablet, Quantity = 1, UnitPrice = tablet.Price },
            new OrderItem { Product = monitor, Quantity = 1, UnitPrice = monitor.Price },
            new OrderItem { Product = camera, Quantity = 1, UnitPrice = camera.Price }
        ]);
        
        // Расчет итоговых сумм
        order1.CalculateTotal();
        order2.CalculateTotal();
        order3.CalculateTotal();
        order4.CalculateTotal();
        
        db.Users.AddRange(user1, user2, user3,user4);
        db.Products.AddRange(laptop, phone, headphones, mouse, keyboard,gamepad, monitor, tablet, camera, speaker, router,ups, microphone, webcam,ssd,powerbank,printer,graphicsCard,processor,motherboard,ram,hdd,smartwatch);
        db.Orders.AddRange(order1, order2, order3, order4);
        
        db.SaveChanges();
        Console.WriteLine("База данных создана и заполнена!");
    }
    // ЗАПРОСЫ ДЛЯ ВЫВОДА СВЯЗАННЫХ ДАННЫХ (LINQ)
static void DemonstrateOrderQueries()
{
    using var db = new ShopContext();
    
    Console.WriteLine("\nЗАПРОСЫ ДЛЯ ЗАКАЗОВ");

    // 1. Заказ с покупателем
    Console.WriteLine("\n1. Заказ с покупателем:");
    var ordersWithCustomers = from o in db.Orders
                             join u in db.Users on o.UserId equals u.Id
                             select new
                             {
                                 OrderId = o.Id,
                                 Customer = u.Name,
                                 Total = o.TotalAmount,
                                 o.Status
                             };
    
    foreach (var item in ordersWithCustomers)
    {
        Console.WriteLine($"Заказ #{item.OrderId}: {item.Customer} - {item.Total} руб ({item.Status})");
    }

    Console.WriteLine();
    // 2. Заказ с товарами
    Console.WriteLine("\n2. Заказ с товарами:");
    var ordersWithProducts = from o in db.Orders
                            join oi in db.OrderItems on o.Id equals oi.OrderId
                            join p in db.Products on oi.ProductId equals p.Id
                            select new
                            {
                                OrderId = o.Id,
                                Product = p.Name,
                                oi.Quantity,
                                Price = oi.UnitPrice,
                                Total = oi.Quantity * oi.UnitPrice
                            };
    
    foreach (var item in ordersWithProducts)
    {
        Console.WriteLine($"Заказ #{item.OrderId}: {item.Product} x{item.Quantity} = {item.Total} руб");
    }

    Console.WriteLine();
    // 3. Заказ с покупателем и товаром
    Console.WriteLine("\n3. Заказ с покупателем и товаром:");
    var ordersWithCustomersAndProducts = from o in db.Orders
                                        join u in db.Users on o.UserId equals u.Id
                                        join oi in db.OrderItems on o.Id equals oi.OrderId
                                        join p in db.Products on oi.ProductId equals p.Id
                                        select new
                                        {
                                            OrderId = o.Id,
                                            Customer = u.Name,
                                            Product = p.Name,
                                            oi.Quantity,
                                            Price = oi.UnitPrice,
                                            Total = oi.Quantity * oi.UnitPrice
                                        };
    
    foreach (var item in ordersWithCustomersAndProducts)
    {
        Console.WriteLine($"Заказ #{item.OrderId}: {item.Customer} - {item.Product} x{item.Quantity} = {item.Total} руб");
    }

    Console.WriteLine();
    Console.WriteLine("4. Конкретный заказ");
    Console.Write("Введите ID заказа: ");

    if (!int.TryParse(Console.ReadLine(), out int orderId))
    {
        Console.WriteLine("Ошибка: Введено некорректное значение ID заказа.");
        return;
    }

    var specificOrder = from o in db.Orders
        join u in db.Users on o.UserId equals u.Id
        join oi in db.OrderItems on o.Id equals oi.OrderId
        join p in db.Products on oi.ProductId equals p.Id
        where o.Id == orderId
        select new
        {
            OrderId = o.Id,
            Customer = u.Name,
            Product = p.Name,
            p.Price
        };

    var orderData = specificOrder.ToList();

    if (!orderData.Any())
    {
        Console.WriteLine($"Заказ с ID {orderId} не найден.");
        return;
    }

    var firstItem = orderData.First();
    Console.WriteLine($"\nЗАКАЗ #{firstItem.OrderId}");
    Console.WriteLine($"Покупатель: {firstItem.Customer}");

    Console.WriteLine("Товары:");
    foreach (var item in orderData)
    {
        Console.WriteLine($"  {item.Product} Цена {item.Price:C}");
    }
    
    Console.WriteLine();
}
     // ЯВНЫЕ JOIN
    static void DemonstrateExplicitJoins()
    {
        using var db = new ShopContext();
        
        Console.WriteLine("\nЯВНЫЕ JOIN");
        
        // 1. JOIN между Order и User 
        Console.WriteLine("\n1. JOIN Orders и Users (метод Join):");
        var ordersWithUsers = from o in db.Orders
            join u in db.Users on o.UserId equals u.Id
            select new
            {
                OrderId = o.Id,
                UserName = u.Name,
                Total = o.TotalAmount,
                o.Status
            };
        foreach (var item in ordersWithUsers)
        {
            Console.WriteLine($"Заказ #{item.OrderId}: {item.UserName} - {item.Total} руб ({item.Status})");
        }
        
        // 2. JOIN между OrderItem, Order, User и Product (оператор join)
        Console.WriteLine("\n2. Множественный JOIN (оператор join):");
        //SELECT через LINQ синтаксис
        var orderDetails = from oi in db.OrderItems
                          join o in db.Orders on oi.OrderId equals o.Id
                          join u in db.Users on o.UserId equals u.Id
                          join p in db.Products on oi.ProductId equals p.Id
                          select new
                          {
                              OrderId = o.Id,
                              UserName = u.Name,
                              ProductName = p.Name,
                              oi.Quantity,
                              Price = oi.UnitPrice,
                              Total = oi.Quantity * oi.UnitPrice
                          };
        
        foreach (var detail in orderDetails)
        {
            Console.WriteLine($"Заказ #{detail.OrderId}: {detail.UserName} - {detail.ProductName} x{detail.Quantity} = {detail.Total} руб");
        }
        
        // 3. JOIN с фильтрацией
        Console.WriteLine("\n3. JOIN с фильтрацией - дорогие товары в заказах:");
        //SELECT через LINQ синтаксис
        var expensiveOrders = from oi in db.OrderItems
                             join o in db.Orders on oi.OrderId equals o.Id
                             join p in db.Products on oi.ProductId equals p.Id
                             where p.Price > 10000
                             select new
                             {
                                 OrderId = o.Id,
                                 ProductName = p.Name,
                                 ProductPrice = p.Price,
                                 oi.Quantity,
                                 Total = oi.Quantity * p.Price
                             };
        
        foreach (var item in expensiveOrders)
        {
            Console.WriteLine($"Заказ #{item.OrderId}: {item.ProductName} x{item.Quantity} = {item.Total} руб");
        }
        //SELECT через LINQ синтаксис
        Console.WriteLine("\n3.5 JOIN с фильтрацией - дешевые товары в заказах:");
        var cheapOrders = from oi in db.OrderItems
            join o in db.Orders on oi.OrderId equals o.Id
            join p in db.Products on oi.ProductId equals p.Id
            where p.Price <= 10000
            select new
            {
                OrderId = o.Id,
                ProductName = p.Name,
                ProductPrice = p.Price,
                oi.Quantity,
                Total = oi.Quantity * p.Price
            };

        foreach (var item in cheapOrders)
        {
            Console.WriteLine($"Заказ #{item.OrderId}: {item.ProductName} x{item.Quantity} = {item.Total} руб");
        }
    }
    
    // ГРУППИРОВКА
    static void DemonstrateGrouping()
{
    using var db = new ShopContext();
    
    Console.WriteLine("\nГРУППИРОВКА");
    
    // 1. Группировка заказов по пользователям (оператор group by)
    Console.WriteLine("\n1. Группировка заказов по пользователям (group by):");
    //SELECT через LINQ синтаксис
    var userGroups = from o in db.Orders
                    group o by o.User.Name into g
                    select new
                    {
                        UserName = g.Key,
                        OrderCount = g.Count(),
                        TotalAmount = g.Sum(o => o.TotalAmount),
                        AvgAmount = g.Average(o => o.TotalAmount)
                    };
    
    foreach (var group in userGroups)
    {
        Console.WriteLine($"{group.UserName}: {group.OrderCount} заказов, сумма: {group.TotalAmount} руб, среднее: {group.AvgAmount:F0} руб");
    }
    
    // 2. Группировка товаров в заказах по продуктам (метод GroupBy)
    Console.WriteLine("\n2. Группировка товаров по продуктам (GroupBy):");
    var productGroups = from oi in db.OrderItems
        group oi by oi.Product.Name into g
        select new
        {
            ProductName = g.Key,
            TotalSold = g.Sum(oi => oi.Quantity),
            TotalRevenue = g.Sum(oi => oi.Quantity * oi.UnitPrice),
            AvgPrice = g.Average(oi => oi.UnitPrice)
        };
    foreach (var group in productGroups)
    {
        Console.WriteLine($"{group.ProductName}: продано {group.TotalSold} шт, выручка: {group.TotalRevenue} руб, средняя цена: {group.AvgPrice:F0} руб");
    }
    
    // 3. Группировка заказов по статусу
    Console.WriteLine("\n3. Группировка заказов по статусу:");
    var statusGroups = from o in db.Orders
                      group o by o.Status into g
                      select new
                      {
                          Status = g.Key,
                          Count = g.Count(),
                          Total = g.Sum(o => o.TotalAmount)
                      };
    
    foreach (var group in statusGroups)
    {
        Console.WriteLine($"{group.Status}: {group.Count} заказов, сумма: {group.Total} руб");
    }
    
    // 4. Группировка с множественной агрегацией 
    Console.WriteLine("\n4. Группировка по способу оплаты:");
    var paymentGroups = from o in db.Orders.AsEnumerable()
        group o by o.PaymentMethod into g
        select new
        {
            PaymentMethod = g.Key,
            OrderCount = g.Count(),
            TotalAmount = g.Sum(o => o.TotalAmount),
            MinAmount = g.Min(o => o.TotalAmount),
            MaxAmount = g.Max(o => o.TotalAmount)
        };
    foreach (var group in paymentGroups)
    {
        Console.WriteLine($"{group.PaymentMethod}: {group.OrderCount} заказов, сумма: {group.TotalAmount} руб (мин: {group.MinAmount}, макс: {group.MaxAmount})");
    }
}
    static void DemonstrateFilter()
    {
        using var db = new ShopContext();
        
        Console.WriteLine("\nФИЛЬТРАЦИЯ");
        
        Console.WriteLine("\n1. WHERE - товары дороже 40000 рублей:");
       //SELECT через LINQ синтаксис
       var expensiveProductsLinq = (from p in db.Products  
            where p.Price > 40000
            select p).ToList();
        foreach (var prod in expensiveProductsLinq)
            Console.WriteLine($"{prod.Name} - {prod.Price} руб");
        
        Console.WriteLine("\n2. WHERE - товары дороже 2000 рублей:");
        var expensiveProducts = db.Products.Where(p => p.Price > 2000);
        foreach (var prod in expensiveProducts)
            Console.WriteLine($"{prod.Name} - {prod.Price} руб");
        Console.WriteLine("\n3. EF.Functions.Like - товары с 'ут' в названии:");
        var productsWithUt = db.Products.Where(p => EF.Functions.Like(p.Name, "%ут%"));
        foreach (var prod in productsWithUt)
            Console.WriteLine($"{prod.Name} - {prod.Price} руб");
    
        Console.WriteLine("\n4. Find - товар с ID=2:");
        var product = db.Products.Find(2);
        if (product != null)
            Console.WriteLine($"{product.Name} - {product.Price} руб");
    
        Console.WriteLine("\n5. FirstOrDefault - первый товар дороже 10000:");
        var firstExpensive = db.Products.FirstOrDefault(p => p.Price > 10000);
        if (firstExpensive != null)
            Console.WriteLine($"{firstExpensive.Name} - {firstExpensive.Price} руб");
    }
    
    
    // ПРОЕКЦИЯ - создание новых типов из данных
    static void DemonstrateProjection()
    {
        using var db = new ShopContext();
        
        Console.WriteLine("\nПРОЕКЦИЯ");
        
        // 1. Проекция в анонимный тип
        Console.WriteLine("\n1. Анонимный тип - товары с категорией цены:");
        var productsWithCategory = db.Products.Select(p => new
        {
            p.Name,
            p.Price,
            Category = p.Price > 30000 ? "Дорогой" : p.Price > 10000 ? "Средний" : "Бюджетный"
        });
        
        foreach (var product in productsWithCategory)
        {
            Console.WriteLine($"{product.Name} - {product.Price} руб ({product.Category})");
        }
        
        // 2. Проекция в определенный тип
        Console.WriteLine("\n2. ProductInfo - информация о товарах:");
        var productInfos = db.Products.Select(p => new ProductInfo
        {
            Name = p.Name,
            Price = p.Price,
            PriceCategory = p.Price > 40000 ? "Премиум" : p.Price > 20000 ? "Бизнес" : "Стандарт"
        });
        
        foreach (var info in productInfos)
        {
            Console.WriteLine($"{info.Name} - {info.Price} руб - {info.PriceCategory}");
        }
        
        // 3. Проекция с JOIN (связанные данные)
        Console.WriteLine("\n3. OrderSummary - сводка по заказам:");
        var orderSummaries = db.Orders.Select(o => new OrderSummary
        {
            OrderId = o.Id,
            CustomerName = o.User.Name,
            TotalAmount = o.TotalAmount,
            Status = o.Status,
            ProductCount = o.OrderItems.Count
        });
        
        foreach (var summary in orderSummaries)
        {
            Console.WriteLine($"Заказ #{summary.OrderId}: {summary.CustomerName} - {summary.TotalAmount} руб ({summary.Status}) - {summary.ProductCount} товаров");
        }
        
        // 4. Проекция с вычисляемыми полями
        Console.WriteLine("\n4. Вычисляемые поля - товары со скидкой:");
        var productsWithDiscount = db.Products.Select(p => new
        {
            OriginalName = p.Name,
            OriginalPrice = p.Price,
            DiscountedPrice = p.Price * 0.9m, // 10% скидка
            Savings = p.Price * 0.1m
        });
        
        foreach (var product in productsWithDiscount)
        {
            Console.WriteLine($"{product.OriginalName}: {product.OriginalPrice} → {product.DiscountedPrice:F0} руб (экономия: {product.Savings:F0} руб)");
        }
    }
    
  static void DemonstrateSorting()
{
    using var db = new ShopContext();
    
    Console.WriteLine("\nСОРТИРОВКА");
    
    // 1. Сортировка по возрастанию 
    Console.WriteLine("\n1. OrderBy - товары по цене (возрастание):");
    var productsAsc = from p in db.Products.AsEnumerable()
        orderby p.Price
        select p;
    foreach (var product in productsAsc)
    {
        Console.WriteLine($"{product.Name} - {product.Price} руб");
    }
    
    // 2. Сортировка по убыванию 
    Console.WriteLine("\n2. OrderByDescending - товары по цене (убывание):");
    var productsDesc = from p in db.Products.AsEnumerable()
        orderby p.Price descending
        select p;
    foreach (var product in productsDesc)
    {
        Console.WriteLine($"{product.Name} - {product.Price} руб");
    }
    
    // 3. Множественная сортировка 
    Console.WriteLine("\n3. ThenBy - пользователи по имени и ID:");
    var usersSorted = from u in db.Users
        orderby u.Name, u.Id
        select u;
    
    foreach (var user in usersSorted)
    {
        Console.WriteLine($"{user.Name} (ID: {user.Id})");
    }
    
    // 4. Сортировка в LINQ syntax 
    Console.WriteLine("\n4. LINQ orderby - заказы по дате:");
    //SELECT через LINQ синтаксис
    var ordersByDate = from o in db.Orders
        orderby o.OrderDate descending
        select o;

    foreach (var order in ordersByDate)
    {
        Console.WriteLine($"Заказ #{order.Id} - {order.OrderDate:dd.MM.yyyy HH:mm} - {order.User.Name}");
    }
    
    // 5. Сортировка с проекцией 
    Console.WriteLine("\n5. Сортировка с проекцией - дорогие товары:");
    var expensiveSorted = from p in db.Products
        where p.Price > 10000
        select p;

    foreach (var product in expensiveSorted)
    {
        Console.WriteLine($"{product.Name} - {product.Price} руб");
    }
    
    // 6. Сортировка по имени 
    Console.WriteLine("\n6. Сортировка товаров по имени:");
    var productsByName = from p in db.Products
        orderby p.Name
        select p;

    foreach (var product in productsByName)
    {
        Console.WriteLine($"{product.Name}");
    }
}
    
    static void DisplayOrdersWithDetails()
    {
        using var db = new ShopContext();
        
        Console.WriteLine("\nВСЕ ЗАКАЗЫ С ДЕТАЛЯМИ");
            
        var orders = db.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .ToList();
        
        foreach (var order in orders)
        {
            Console.WriteLine($"\nЗаказ #{order.Id}");
            Console.WriteLine($"Пользователь: {order.User.Name}");
            Console.WriteLine($"Дата: {order.OrderDate:dd.MM.yyyy HH:mm}");
            Console.WriteLine($"Способ оплаты: {order.PaymentMethod}");
            Console.WriteLine($"Адрес доставки: {order.ShippingAddress}");
            Console.WriteLine($"Статус: {order.Status}");
            Console.WriteLine($"Итоговая сумма: {order.TotalAmount} ₽");
            
            Console.WriteLine("Товары в заказе:");
            foreach (var item in order.OrderItems)
            {
                Console.WriteLine($"   {item.Product.Name} x{item.Quantity} = {item.TotalPrice} ₽");
            }
        }
        
        Console.WriteLine("\nДЕМОНСТРАЦИЯ СВЯЗЕЙ");
        
        var users = db.Users.Include(u => u.Orders)
            .ThenInclude(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .ToList();

        foreach (var user in users)
        {
            Console.WriteLine($"\n{user.Name} покупал(а):");
            var userProducts = user.Orders
                .SelectMany(o => o.OrderItems)
                .Select(oi => oi.Product.Name)
                .Distinct();
            var products = userProducts.ToList();
            
            if (products.Any())
            {
                foreach (var product in products)
                {
                    Console.WriteLine($"   {product}");
                }
            }
            else
            {
                Console.WriteLine("   Ничего не покупал(а)! Сжечь!");
            }
        }
    }
}