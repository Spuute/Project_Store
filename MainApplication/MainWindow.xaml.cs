using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Data;
using System.Text.RegularExpressions;

namespace MainApplication
{
    public class DiscountCode
    {
        public string Name;
        public decimal Code;

        public DiscountCode(string name, decimal code)
        {
            Name = name;
            Code = code;
        }
    }
    public class Product
    {
        public string Name;
        public decimal Price;
        public string Image;
        public string Description;

        public Product(string name, decimal price, string image, string description)
        {
            Name = name;
            Price = price;
            Image = image;
            Description = description;
        }
    }
    public partial class MainWindow : Window
    {
        private Grid grid;
        private Dictionary<Product, int> cart;
        private Dictionary<Product, int> loadCart;
        private List<StackPanel> productStack;
        private Button checkout;
        private ListBox cartSide;
        private Label totalSum;
        private TextBox discount;
        private StackPanel menuPanel, cartSidePanel, orderPanel;
        private WrapPanel products;
        private TextBlock menuTitle, productTitle, cartTitle, mainTitle;
        public static Dictionary<Product, int> productList = new Dictionary<Product, int>();
        public static List<DiscountCode> discountList;
        private int discountCount = 0;
        private decimal sum = 0;
        public static string tempAddress = @"C:\Windows\Temp\Utilities\";

        public MainWindow()
        {
            // Check if C:\Win\Temp have a folder named Utilities, if not copy it from solution folder, then start the program.
            DirectoryCopy(Directory.GetCurrentDirectory() + @"\Utilities", @"C:\Windows\Temp\Utilities", true);
            InitializeComponent();
            Start();
        }

        private void Start()
        {
            // To make sure we always can use periods as decimal point.
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            // Deserialize productlist and discountlist.
            productList = FileDeserialisation(tempAddress + "Products.csv");
            discountList = DiscountFileDeserialization();
            cart = new Dictionary<Product, int>();

            // Window options
            Title = "Food Store";
            Width = 1424;
            Height = 800;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Scrolling
            ScrollViewer root = new ScrollViewer();
            root.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            Content = root;

            // Main grid
            grid = new Grid { Background = new SolidColorBrush(Color.FromRgb(224, 240, 224)) };
            root.Content = grid;
            grid.Margin = new Thickness(5);
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });//());
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(300) });

            // Our main title
            mainTitle = CreateTextBlock("Food Store", 30, 0, 0, 0, 0);
            grid.Children.Add(mainTitle);
            mainTitle.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetRow(mainTitle, 0);
            Grid.SetColumn(mainTitle, 0);
            Grid.SetColumnSpan(mainTitle, 3);

            // Subtitle for "Menu"
            menuTitle = CreateTextBlock("Menu", 20, 0, 14, 0, 0);
            grid.Children.Add(menuTitle);
            menuTitle.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetRow(menuTitle, 1);
            Grid.SetColumn(menuTitle, 0);

            // Subtitle for "Products"
            productTitle = CreateTextBlock("Products", 20, 0, 14, 0, 0);
            grid.Children.Add(productTitle);
            productTitle.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetRow(productTitle, 1);
            Grid.SetColumn(productTitle, 1);

            // Subtitle for "Cart"
            cartTitle = CreateTextBlock("Cart", 20, 0, 14, 0, 0);
            grid.Children.Add(cartTitle);
            cartTitle.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetRow(cartTitle, 1);
            Grid.SetColumn(cartTitle, 2);

            // Create the StackPanel for the manu.
            menuPanel = CreateMenuPanel();
            grid.Children.Add(menuPanel);
            Grid.SetRow(menuPanel, 2);
            Grid.SetColumn(menuPanel, 0);

            // Create the StackPanel for the right side with cart and buttons.
            cartSidePanel = CreateCartSidePanel();
            grid.Children.Add(cartSidePanel);
            Grid.SetRow(cartSidePanel, 2);
            Grid.SetColumn(cartSidePanel, 2);

            // Create the WrapPanel for the products.
            products = CreateProductPanel();
            grid.Children.Add(products);
            Grid.SetRow(products, 2);
            Grid.SetColumn(products, 1);

            // Check if the file SavedCart exist. If it does Load it to dictionary loadCart.
            if (File.Exists(tempAddress + "SavedCart.csv"))
            {
                loadCart = CartFileDeserialisation();

                foreach (KeyValuePair<Product, int> p in loadCart)
                {
                    // Point fromSaved to the object in productList where the key names match.
                    Product fromSaved = productList.First(x => x.Key.Name == p.Key.Name).Key;
                    // if cart don´t contains the product, add it with it´s saved value.
                    if (!cart.ContainsKey(fromSaved))
                    {
                        cart.Add(fromSaved, p.Value);
                    }
                    // Add the name, amount and price to ListBox
                    cartSide.Items.Add($"{fromSaved.Name} x{p.Value} ({fromSaved.Price})");
                }
                sum = cart.Sum(x => x.Key.Price * x.Value);
                checkout.Content = $"Place Order ({cart.Sum(x => x.Value)})";
                totalSum.Content = $"Total: {Math.Round(sum, 2)} :-";
            }
        }

        private StackPanel CreateMenuPanel()
        {
            StackPanel menuPanel = new StackPanel { Orientation = Orientation.Vertical };

            checkout = CreateButton("Place Order", 200, 0, 10, 0, 0, 5);
            menuPanel.Children.Add(checkout);
            checkout.Click += PlaceOrder_ButtonClick;

            Button saveCart = CreateButton("Save Cart", 200, 0, 10, 0, 0, 5);
            menuPanel.Children.Add(saveCart);
            saveCart.Click += SaveCart_ButtonClick;

            return menuPanel;
        }
        private StackPanel CreateCartSidePanel()
        {
            // Create the StackPanel and set it´s orientation to vertical.
            StackPanel cartSidePanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(0, 0, 10, 0)
            };

            // Create the grid used inside this StackPanel.
            Grid cartGrid = new Grid();
            cartSidePanel.Children.Add(cartGrid);
            cartGrid.RowDefinitions.Add(new RowDefinition());
            cartGrid.RowDefinitions.Add(new RowDefinition());
            cartGrid.RowDefinitions.Add(new RowDefinition());
            cartGrid.RowDefinitions.Add(new RowDefinition());
            cartGrid.RowDefinitions.Add(new RowDefinition());
            cartGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            cartGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // Create the ListBox used to print current items in cart. 
            cartSide = new ListBox
            {
                Background = new SolidColorBrush(Color.FromRgb(249, 251, 249)),
                Margin = new Thickness(0, 10, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = 300,
                Height = 500,
            };
            cartGrid.Children.Add(cartSide);
            Grid.SetRow(cartSide, 0);
            Grid.SetColumn(cartSide, 0);
            Grid.SetColumnSpan(cartSide, 2);

            // Create the Label to show the total amount of cart.
            totalSum = new Label
            {
                Content = $"Total: 0 :-"
            };
            cartGrid.Children.Add(totalSum);
            Grid.SetRow(totalSum, 1);
            Grid.SetColumn(totalSum, 0);

            // Create the discount TextBox.
            discount = new TextBox
            {
                Foreground = Brushes.Gray,
                Background = Brushes.LightGoldenrodYellow,
                Text = "Rabattkod",
                Margin = new Thickness(0, 4, 2, 0)
            };
            cartGrid.Children.Add(discount);
            Grid.SetRow(discount, 2);
            Grid.SetColumn(discount, 0);
            Grid.SetColumnSpan(discount, 1);
            discount.GotFocus += DiscountBox_Click;

            // Create the Button for adding the discount code.
            Button discountButton = CreateButton("Add", 146, 4, 4, 0, 0, 2);
            cartGrid.Children.Add(discountButton);
            Grid.SetRow(discountButton, 2);
            Grid.SetColumn(discountButton, 1);
            discountButton.Click += DiscountButton_Click;

            // Create the Button to remove single product from cart.
            Button remove = CreateButton("Remove Product", 146, 0, 4, 2, 0, 2);
            remove.HorizontalAlignment = HorizontalAlignment.Left;
            cartGrid.Children.Add(remove);
            Grid.SetRow(remove, 3);
            Grid.SetColumn(remove, 0);
            remove.Click += RemoveSingleItem_ButtonClick;

            // Create the button to clear cart.
            Button clear = CreateButton("Clear Cart", 146, 2, 4, 0, 0, 2);
            clear.HorizontalAlignment = HorizontalAlignment.Right;
            cartGrid.Children.Add(clear);
            Grid.SetRow(clear, 3);
            Grid.SetColumn(clear, 1);
            clear.Click += ClearCart_ButtonClick;

            return cartSidePanel;
        }
        public StackPanel CreateProducts(string title, string image, string info, decimal price)
        {
            // Method for creating the products. Every line from the csv file goes through this method. 
            StackPanel newProduct = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Width = 100,
                Margin = new Thickness(5, 5, 5, 5),
            };

            Grid productGrid = new Grid { Background = new SolidColorBrush(Color.FromRgb(249, 251, 249)) };
            newProduct.Children.Add(productGrid);
            productGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(60) }); // We have a fixed height so long titles won´t mess up our formatting.
            productGrid.RowDefinitions.Add(new RowDefinition());
            productGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(100) }); // We have a fixed height so long descriptions won´t mess up our formatting. (We can skip this if we split our desciption to show only the 3 first words)
            productGrid.RowDefinitions.Add(new RowDefinition());
            productGrid.RowDefinitions.Add(new RowDefinition());
            productGrid.RowDefinitions.Add(new RowDefinition());
            productGrid.ColumnDefinitions.Add(new ColumnDefinition());
            productGrid.ColumnDefinitions.Add(new ColumnDefinition());
            productGrid.ColumnDefinitions.Add(new ColumnDefinition());

            // The name of the product.
            TextBlock productName = new TextBlock
            {
                Text = title,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontFamily = new FontFamily("Constantia"),
                FontSize = 16,
                TextAlignment = TextAlignment.Center
            };

            productGrid.Children.Add(productName);
            Grid.SetRow(productName, 0);
            Grid.SetColumn(productName, 0);
            Grid.SetColumnSpan(productName, 3);

            // The product image.
            Image productImage = CreateImage(image);
            productImage.Stretch = Stretch.Uniform;
            productGrid.Children.Add(productImage);
            Grid.SetRow(productImage, 1);
            Grid.SetColumn(productImage, 0);
            Grid.SetColumnSpan(productImage, 3);

            // The product description.
            TextBlock productDescription = new TextBlock
            {
                Text = info,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontFamily = new FontFamily("Constantia"),
                FontSize = 12,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            productGrid.Children.Add(productDescription);
            Grid.SetRow(productDescription, 2);
            Grid.SetColumn(productDescription, 0);
            Grid.SetColumnSpan(productDescription, 3);

            // The product price.
            Label productPrice = new Label
            {
                Content = price + ":-",
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Center,
                FontFamily = new FontFamily("Constantia"),
                FontSize = 20
            };
            productGrid.Children.Add(productPrice);
            Grid.SetRow(productPrice, 3);
            Grid.SetColumn(productPrice, 0);
            Grid.SetColumnSpan(productPrice, 3);

            // Add a label to colorize the last row in our product StackPanel
            Label colorize = new Label
            {
                Background = new SolidColorBrush(Color.FromRgb(148, 182, 148)),
            };
            productGrid.Children.Add(colorize);
            Grid.SetRow(colorize, 5);
            Grid.SetColumn(colorize, 0);
            Grid.SetColumnSpan(colorize, 3);

            // Remove product from cart button
            Button removeOne = new Button
            {
                Background = new SolidColorBrush(Color.FromRgb(188, 223, 188)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(96, 170, 96)),
                Foreground = new SolidColorBrush(Color.FromRgb(79, 131, 79)),
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Left,
                Content = "-",
                Width = 25,
                Margin = new Thickness(5, 5, 0, 5),
                Tag = title, // Tag with title which is the product name
            };
            productGrid.Children.Add(removeOne);
            Grid.SetRow(removeOne, 5);
            Grid.SetColumn(removeOne, 0);
            removeOne.Click += RemoveFromCart_ButtonClick;

            // Add product to cart button. 
            Button addOne = new Button
            {
                Background = new SolidColorBrush(Color.FromRgb(188, 223, 188)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(96, 170, 96)),
                Foreground = new SolidColorBrush(Color.FromRgb(79, 131, 79)),
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Right,
                Content = "+",
                Width = 25,
                Margin = new Thickness(0, 5, 5, 5),
                Tag = title, // Tag with title which is the product name
            };
            productGrid.Children.Add(addOne);
            Grid.SetRow(addOne, 5);
            Grid.SetColumn(addOne, 2);
            addOne.Click += AddToCart_ButtonClick;

            return newProduct;
        }
        private WrapPanel CreateProductPanel()
        {
            WrapPanel products = new WrapPanel
            {
                Orientation = Orientation.Horizontal,
            };

            productStack = new List<StackPanel>(); // Create an empty StackPanel list.

            foreach (KeyValuePair<Product, int> p in productList) // Iterate through our productList.
            {
                productStack.Add(CreateProducts(p.Key.Name, p.Key.Image, p.Key.Description, p.Key.Price)); // For every product in our list, send it to our CreateProducts method which creates a StackPanel of it, then add it to our StackPanel list.
            }

            foreach (StackPanel stack in productStack) // Iterate through the StackPanel list
            {
                products.Children.Add(stack); // For every stack in our StackPanel List, add it to our WrapPanel.
            }
            return products;
        }
        private StackPanel CreateCheckoutPanel(Dictionary<Product, int> inputList)
        {
            StackPanel checkout = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(5, 5, 5, 5),
            };

            Grid checkoutGrid = new Grid();
            checkout.Children.Add(checkoutGrid);
            checkoutGrid.RowDefinitions.Add(new RowDefinition());
            checkoutGrid.RowDefinitions.Add(new RowDefinition());
            checkoutGrid.RowDefinitions.Add(new RowDefinition());
            checkoutGrid.RowDefinitions.Add(new RowDefinition());
            checkoutGrid.RowDefinitions.Add(new RowDefinition());
            checkoutGrid.RowDefinitions.Add(new RowDefinition());
            checkoutGrid.RowDefinitions.Add(new RowDefinition());
            checkoutGrid.ColumnDefinitions.Add(new ColumnDefinition());
            checkoutGrid.ColumnDefinitions.Add(new ColumnDefinition());

            // Create TextBlock for thank you message
            TextBlock orderTitle = CreateTextBlock("Thanks for your order!", 30, 0, 0, 0, 0);
            orderTitle.HorizontalAlignment = HorizontalAlignment.Center;
            checkoutGrid.Children.Add(orderTitle);
            Grid.SetRow(orderTitle, 0);
            Grid.SetColumn(orderTitle, 0);
            Grid.SetColumnSpan(orderTitle, 2);

            // Create TextBlock for dateTime
            TextBlock dateTime = CreateTextBlock($"Order placed: {DateTime.Now}", 12, 0, 0, 0, 0);
            dateTime.HorizontalAlignment = HorizontalAlignment.Center;
            checkoutGrid.Children.Add(dateTime);
            Grid.SetRow(dateTime, 1);
            Grid.SetColumn(dateTime, 0);
            Grid.SetColumnSpan(dateTime, 2);

            // Create TextBlock for Summary text.
            TextBlock summary = CreateTextBlock("Summary:", 22, 0, 10, 0, 10);
            summary.HorizontalAlignment = HorizontalAlignment.Center;
            checkoutGrid.Children.Add(summary);
            Grid.SetRow(summary, 2);
            Grid.SetColumn(summary, 0);
            Grid.SetColumnSpan(summary, 3);

            // Create a DataGrid for the cart receipt.
            DataGrid cartListReceipt = new DataGrid();
            cartListReceipt.ColumnWidth = 200;
            cartListReceipt.IsReadOnly = true;
            cartListReceipt.Background = new SolidColorBrush(Color.FromRgb(249, 251, 249));
            checkoutGrid.Children.Add(cartListReceipt);
            Grid.SetRow(cartListReceipt, 3);
            Grid.SetColumnSpan(cartListReceipt, 3);

            // Create a DataTable for receipt.
            DataTable cartListReceiptTable = new DataTable();
            cartListReceiptTable.Columns.Add(new DataColumn("Amount", typeof(string)));
            cartListReceiptTable.Columns.Add(new DataColumn("Product", typeof(string)));
            cartListReceiptTable.Columns.Add(new DataColumn("Unit price", typeof(string)));
            cartListReceiptTable.Columns.Add(new DataColumn("Total", typeof(string)));

            // Create temp dictionary 
            Dictionary<Product, int> tempDict = new Dictionary<Product, int>();

            // Iterate through the inputList (cart) and add a new row to the datatable for every item, then add the item to tempDict.
            foreach (KeyValuePair<Product, int> p in inputList)
            {
                cartListReceiptTable.Rows.Add(new object[] { p.Value, p.Key.Name, Math.Round(p.Key.Price, 2), Math.Round(p.Key.Price * p.Value, 2) });
                tempDict.Add(p.Key, p.Value);
            }

            cartListReceipt.ItemsSource = cartListReceiptTable.DefaultView;
            
            // reset the prices if discount code has been used
            productList = FileDeserialisation(tempAddress + "Products.csv");

            // Iterate throug tempDict and change the prices from discountprice to normal price 
            foreach (KeyValuePair<Product, int> kvp in tempDict)
            {
                kvp.Key.Price = productList.Where(x => x.Key.Name == kvp.Key.Name).Select(x => x.Key.Price).First();
            }

            // calculations
            decimal sumBeforeDiscount = tempDict.Sum(x => x.Key.Price * x.Value);
            decimal totalDiscount = sumBeforeDiscount - sum;
            decimal showPercent = (totalDiscount / sumBeforeDiscount) * 100;

            // Create TextBlock to show total amount before discount.
            TextBlock discountUsed = CreateTextBlock($"Total before discount: {Math.Round(sumBeforeDiscount, 2)}", 12, 0, 10, 0, 0);
            checkoutGrid.Children.Add(discountUsed);
            Grid.SetRow(discountUsed, 4);
            Grid.SetColumn(discountUsed, 0);

            // Create TextBlock to show money saved on discount and discount percentage used.
            TextBlock calculateDiscount = CreateTextBlock($"Total discount: -{Math.Round(totalDiscount, 2)} | ({Math.Round(showPercent, 0)}%)", 12, 0, 0, 0, 0);
            checkoutGrid.Children.Add(calculateDiscount);
            Grid.SetRow(calculateDiscount, 5);
            Grid.SetColumn(calculateDiscount, 0);

            // Create TextBlock to show final sum after discounts.
            TextBlock cartTotal = CreateTextBlock($"Total: {Math.Round(sum, 2)}:-", 12, 0, 0, 0, 0);
            cartTotal.FontWeight = FontWeights.Bold;
            checkoutGrid.Children.Add(cartTotal);
            Grid.SetRow(cartTotal, 6);
            Grid.SetColumn(cartTotal, 0);
            UpdateListBox();

            return checkout;
        }
        public static Dictionary<Product, int> FileDeserialisation(string filePath)
        {
            Dictionary<Product, int> tempDict = new Dictionary<Product, int>();

            string[] fromFile = File.ReadAllLines(filePath);

            foreach (string lines in fromFile)
            {
                string[] parts = lines.Split(',');
                Product p = new Product(parts[0], decimal.Parse(parts[1]), parts[2], parts[3]);
                tempDict.Add(p, 1); // Add the product to dictionary as key and 1 as value.
            }
            return tempDict;
        }
        public static List<DiscountCode> DiscountFileDeserialization()
        {
            // Create a temporary List to store the codes in.
            List<DiscountCode> tempList = new List<DiscountCode>();

            string[] fromFile = File.ReadAllLines(tempAddress + "Discounts.csv");

            foreach (string lines in fromFile)
            {
                //if (lines != string.Empty)
                //{
                string[] parts = lines.Split(',');
                DiscountCode d = new DiscountCode(parts[0], decimal.Parse(parts[1]));
                tempList.Add(d);
                //}
            }
            return tempList;
        }
        private Dictionary<Product, int> CartFileDeserialisation()
        {
            Dictionary<Product, int> tempDict = new Dictionary<Product, int>();
            string[] fromFile = File.ReadAllLines(tempAddress + "SavedCart.csv");

            foreach (string s in fromFile)
            {
                string[] parts = s.Split(',');
                int amount = int.Parse(parts[4]);
                Product p = new Product(parts[0], decimal.Parse(parts[1]), parts[2], parts[3]);
                tempDict.Add(p, amount);
            }
            return tempDict;
        }
        public static Image CreateImage(string filePath)
        {
            ImageSource source = new BitmapImage(new Uri(tempAddress + @"Images\" + filePath, UriKind.RelativeOrAbsolute));
            Image image = new Image
            {
                Source = source,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5),
                Stretch = Stretch.Uniform,
            };
            return image;
        }
        public static TextBlock CreateTextBlock(string text, int fontSize, int left, int up, int right, int down)
        {
            TextBlock newTextBlock = new TextBlock
            {
                Text = text,
                FontSize = fontSize,
                Margin = new Thickness(left, up, right, down),
            };

            return newTextBlock;
        }
        public static Button CreateButton(string content, int width, int left, int up, int right, int down, int padding)
        {
            Button test = new Button
            {
                Background = new SolidColorBrush(Color.FromRgb(87, 209, 128)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(96, 170, 96)),
                Content = content,
                Width = width,
                Margin = new Thickness(left, up, right, down),
                Padding = new Thickness(padding),
            };
            return test;
        }
        private void SaveCart()
        {
            string toFile = String.Empty;

            // Iterate through the cart dictionary and write all products with its value (amount) to file.
            foreach (KeyValuePair<Product, int> p in cart)
            {
                string line = $"{p.Key.Name},{p.Key.Price},{p.Key.Image},{p.Key.Description},{p.Value} {Environment.NewLine}";
                toFile += line;
            }
            if (!File.Exists(tempAddress + "SavedCart.csv"))
            {
                File.WriteAllText(tempAddress + "SavedCart.csv", toFile);
            }
            else
            {
                File.WriteAllText(tempAddress + "SavedCart.csv", toFile);
            }
        }
        private void UpdateListBox()
        {
            // Clear the cart ListBox and iterate through the cart dictionary and add every key (object) with it´s value (amount).
            cartSide.Items.Clear();
            foreach (KeyValuePair<Product, int> p in cart)
            {
                cartSide.Items.Add($"{p.Key.Name} x{p.Value} ({Math.Round(p.Key.Price, 2)})");
            }
            sum = cart.Sum(x => x.Key.Price * x.Value);
        }
        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            // If directory don´t exists throw message.
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                try
                {
                    string tempPath = Path.Combine(destDirName, file.Name);
                    file.CopyTo(tempPath, false);
                }
                catch
                {

                }
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }
        private void ClearCart_ButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to clear the shopping cart?", "Warning", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                // Clear the ListBox.
                cartSide.Items.Clear();
                // Clear the Dictionary for our cart.
                cart.Clear();

                sum = 0;
                // Reset the content for the totalSum label and checkout button counter.
                totalSum.Content = $"Total:  {Math.Round(sum, 2)} :-";
                checkout.Content = $"Place Order ({cart.Sum(x => x.Value)})";
                // ReRead the productlist (if any discount was added, the prices will now be stockprices).
                productList = FileDeserialisation(tempAddress + "Products.csv");
            }
            // Reset the discount counter.
            discountCount = 0;
            // Save the empty cart.
            SaveCart();
        }
        private void SaveCart_ButtonClick(object sender, RoutedEventArgs e)
        {
            SaveCart();
            MessageBox.Show("The shopping cart is saved");
        }
        private void RemoveSingleItem_ButtonClick(object sender, RoutedEventArgs e)
        {
            // Save the selected index to a variable and use it to remove selected item form ListBox and cart dictionary, then update totalSum label and checkout button content.
            try
            {
                int selectedIndex = cartSide.SelectedIndex;
                cartSide.Items.RemoveAt(selectedIndex);
                cart.Remove(cart.ElementAt(selectedIndex).Key);
                sum = cart.Sum(x => x.Key.Price * x.Value);
                totalSum.Content = $"Total: {Math.Round(sum, 2)}:-";
                checkout.Content = $"Place Order ({cart.Sum(x => x.Value)})";
            }
            // Throw a message if no items in the ListBox are selected.
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Your cart is either empty or no product is selected");
            }
        }
        private void PlaceOrder_ButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to place the order?", "Warning", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                // Create the WrapPanel for the checkout.
                orderPanel = CreateCheckoutPanel(cart);
                grid.Children.Add(orderPanel);
                Grid.SetRow(orderPanel, 2);
                Grid.SetColumn(orderPanel, 1);
                Grid.SetColumnSpan(orderPanel, 3);

                // Hide all the other panels.
                menuPanel.Visibility = Visibility.Collapsed;
                products.Visibility = Visibility.Hidden;
                cartSidePanel.Visibility = Visibility.Hidden;

                // Instead of hiding the title and subtitles just change them to empty
                menuTitle.Text = "";
                productTitle.Text = "";
                cartTitle.Text = "";
                mainTitle.Text = "";
            }
        }
        private void DiscountBox_Click(object sender, RoutedEventArgs e)
        {
            // To change the colors on the discount TextBox when user click it.
            bool hasBeenClicked = false;
            if (!hasBeenClicked)
            {
                discount = sender as TextBox;
                discount.Foreground = Brushes.Black;
                discount.Background = Brushes.White;
                discount.Text = String.Empty;
            }
        }
        private void DiscountButton_Click(object sender, RoutedEventArgs e)
        {
            // Makes lowercase letter to uppercase.
            string discountCode = discount.Text.ToUpper();
            // Prevents the user from using special characters in discount code.
            var regex = new Regex("^[a-zA-Z0-9]*$");

            bool inList = discountList.Any(x => x.Name == discountCode);
            if (inList == true)
            {
                decimal disCode = discountList.Where(x => x.Name == discountCode).Select(x => x.Code).First();
                if (discountCount >= 1)
                {
                    MessageBox.Show("You can only use one discount code");
                    return;
                }
                else if (discountCode.Length < 3 || discountCode.Length > 20)
                {
                    MessageBox.Show("The discount code can not be shorter than 3 letters or longer than 20");
                    return;
                }
                else if (!regex.IsMatch(discountCode))
                {
                    MessageBox.Show("The discount code can't contain special characters");
                    return;
                }
                discountCount += 1;
                MessageBox.Show("Discount code added");
                foreach (KeyValuePair<Product, int> p in productList)
                {
                    p.Key.Price = disCode * p.Key.Price;
                }
                sum = disCode * sum;
            }
            else
            {
                MessageBox.Show("Invalid discount code!");
            }

            UpdateListBox();
            totalSum.Content = $"Total: {Math.Round(sum, 2)} :-";
        }
        public static string TotalDiscounts(int discountCount)
        {
            // Created this method only for testing purposes. Check if a discount code have been used or not.
            string messageText = "";
            if (discountCount == 0)
            {
                messageText = "Discount code added";
                return messageText;
            }
            else
            {
                messageText = "You can only use one discount code";
                return messageText;
            }

        }
        private void RemoveFromCart_ButtonClick(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;
            // Use the tag in the - button to get the productname
            string productName = (string)clickedButton.Tag;
            // Point fromCart to the productname in productList
            Product fromCart = productList.First(x => x.Key.Name == productName).Key;

            // If cart dictionary contains the object, remove one and change the sum label
            if (cart.ContainsKey(fromCart))
            {
                cart[fromCart]--;
                sum = cart.Sum(x => x.Key.Price * x.Value);
            }
            // else throw a message that the current product do not exist in the cart.
            else
            {
                MessageBoxResult warning = MessageBox.Show($"{fromCart.Name} does not exist in you cart");
            }

            // When we press - with the product amount 1, remove it from the cart dictionary
            foreach (KeyValuePair<Product, int> pair in cart)
            {
                if (pair.Value == 0)
                {
                    cart.Remove(pair.Key);
                }
            }
            // update the cart ListBox
            UpdateListBox();
            // If the cart dictionary is empty, change the sum to 0
            if (cart.Count == 0)
            {
                sum = 0;
            }
            // Update the totalSum label and checkout button.
            checkout.Content = $"Place Order ({cart.Sum(x => x.Value)})";
            totalSum.Content = $"Total: {Math.Round(sum, 2)} :-";
        }
        public void AddToCart_ButtonClick(object sender, RoutedEventArgs e)
        {
            // Same as for the RemoveFromCartClick
            Button clickedButton = (Button)sender;
            string productName = (string)clickedButton.Tag;
            Product toCart = productList.First(x => x.Key.Name == productName).Key;

            // If the cart Dictionary contains the object, increase it´s value by one
            if (cart.ContainsKey(toCart))
            {
                cart[toCart]++;
            }
            // If the cart Dictionary do not contain the object, add it with a value of one.
            else
            {
                cart.Add(toCart, 1);
            }
            // update cart ListBox
            UpdateListBox();
            // Update the totalSum label and checkout button.
            sum = cart.Sum(x => x.Key.Price * x.Value);
            checkout.Content = $"Place Order ({cart.Sum(x => x.Value)})";
            totalSum.Content = $"Total: {Math.Round(sum, 2)} :-";
        }
        
    }
}