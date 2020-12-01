using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MainApplication;

namespace SecondApplication
{
    public partial class MainWindow : Window
    {
        private Dictionary<Product, int> products = new Dictionary<Product, int>();
        private List<DiscountCode> discountList = new List<DiscountCode>();
        private List<string> productImages;
        private StackPanel discountTextBoxPanel, productButtonPanel, productTextBoxPanel, discountButtonPanel, motivationPanel, productListPanel, discountListPanel;
        private WrapPanel productImagePanel;
        private TextBox imageBox, nameBox, priceBox, descriptionBox, percentageBox, codeBox;
        private ListBox availableProductsListBox, discountListBox;
        private RadioButton checkBox;

        public MainWindow()
        {
            InitializeComponent();
            Start();
        }

        private void Start()
        {
            // To make sure we always can use periods as decimal point.
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            // Read the product csv file using the method in MainApplication
            products = MainApplication.MainWindow.FileDeserialisation(MainApplication.MainWindow.tempAddress + "Products.csv");
            // Create a discount List and call the method for Dezerialisation in the MainApplication.
            discountList = MainApplication.MainWindow.DiscountFileDeserialization();
            // Read the images from image csv file and add the filepaths to a list.
            productImages = ImageListDeserialization();

            // Window options
            Title = "Food Store - STAFF";
            Width = 1420;
            Height = 850;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Scrolling
            ScrollViewer root = new ScrollViewer();
            root.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            Content = root;

            // Main grid
            Grid grid = new Grid { Background = new SolidColorBrush(Color.FromRgb(224, 240, 224)) };
            root.Content = grid;
            grid.Margin = new Thickness(5);
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(300) }); ;

            // Create a textblock with Main Title.
            TextBlock mainTitle = MainApplication.MainWindow.CreateTextBlock("Food Store - STAFF", 30, 0, 0, 0, 0);
            mainTitle.HorizontalAlignment = HorizontalAlignment.Center;
            grid.Children.Add(mainTitle);
            Grid.SetRow(mainTitle, 0);
            Grid.SetColumn(mainTitle, 0);
            Grid.SetColumnSpan(mainTitle, 2);

            // Creates the main menu under the main title.
            StackPanel mainMenu = CreateMainMenu();
            grid.Children.Add(mainMenu);
            Grid.SetRow(mainMenu, 1);
            Grid.SetColumn(mainMenu, 0);
            Grid.SetColumnSpan(mainMenu, 2);

            // Creates the StackPanel that shows the content to add a new product to the store. 
            productTextBoxPanel = CreateProduct_TextBoxPanel();
            grid.Children.Add(productTextBoxPanel);
            Grid.SetRow(productTextBoxPanel, 2);
            Grid.SetColumn(productTextBoxPanel, 0);

            // Creates the buttonpanel for "Add Product" section.
            productButtonPanel = CreateProduct_ButtonPanel();
            grid.Children.Add(productButtonPanel);
            Grid.SetRow(productButtonPanel, 2);
            Grid.SetColumn(productButtonPanel, 1);

            // Creates the WrapPanel to display all product images.
            productImagePanel = CreateProduct_ImageGrid();
            grid.Children.Add(productImagePanel);
            Grid.SetRow(productImagePanel, 3);
            Grid.SetColumn(productImagePanel, 0);
            // Grid.SetColumnSpan(images, 3);

            // Creates the StackPanel for current stock with buttons.
            productListPanel = CreateProduct_ListPanel();
            grid.Children.Add(productListPanel);
            Grid.SetRow(productListPanel, 3);
            Grid.SetColumn(productListPanel, 2);

            // Creates the StackPanel that shows the content to add a new Discount code. 
            discountTextBoxPanel = CreateDiscount_TextBoxPanel();
            grid.Children.Add(discountTextBoxPanel);
            Grid.SetRow(discountTextBoxPanel, 2);
            Grid.SetColumn(discountTextBoxPanel, 0);
            // Visibility is set to hidden at launch.
            discountTextBoxPanel.Visibility = Visibility.Hidden;

            // Creates the StackPanel that contains buttons for the discount section.
            discountButtonPanel = CreateDiscount_ButtonPanel();
            grid.Children.Add(discountButtonPanel);
            Grid.SetRow(discountButtonPanel, 2);
            Grid.SetColumn(discountButtonPanel, 2);
            // Visibility is set to hidden at launch.
            discountButtonPanel.Visibility = Visibility.Hidden;

            // Creates the StackPanel to display all discountcodes. 
            motivationPanel = CreateDiscount_MotivationPanel();
            grid.Children.Add(motivationPanel);
            Grid.SetRow(motivationPanel, 3);
            Grid.SetColumn(motivationPanel, 0);
            // Visibility is set to hidden at launch.
            motivationPanel.Visibility = Visibility.Hidden;

            // Creates the StackPanel for current discounts with buttons.
            discountListPanel = CreateDiscount_ListPanel();
            grid.Children.Add(discountListPanel);
            Grid.SetRow(discountListPanel, 3);
            Grid.SetColumn(discountListPanel, 1);
            // Visibility is set to hidden at launch.
            discountListPanel.Visibility = Visibility.Hidden;
        }

        private StackPanel CreateMainMenu()
        {
            StackPanel mainMenu = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            // Button to get to the Product section of the app.
            Button addProduct = new Button
            {
                Foreground = new SolidColorBrush(Colors.DarkGreen),
                Background = new SolidColorBrush(Colors.White) { Opacity = 0 },
                BorderBrush = new SolidColorBrush(Colors.White) { Opacity = 0 },
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Content = "Products",
            };
            mainMenu.Children.Add(addProduct);
            addProduct.Click += ProductSection_ButtonClick;

            // Button to get to the Discount sections of the app.
            Button addDiscount = new Button
            {
                Margin = new Thickness(10, 0, 0, 0),
                Foreground = new SolidColorBrush(Colors.DarkGreen),
                Background = new SolidColorBrush(Colors.White) { Opacity = 0 },
                BorderBrush = new SolidColorBrush(Colors.White) { Opacity = 0 },
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Content = "Discounts",
            };
            mainMenu.Children.Add(addDiscount);
            addDiscount.Click += DiscountSection_ButtonClick;

            return mainMenu;
        }
        private StackPanel CreateProduct_TextBoxPanel()
        {
            // Create the StackPanel
            StackPanel addProdcut = new StackPanel
            {
                Margin = new Thickness(50, 10, 50, 5),
                Orientation = Orientation.Vertical,
                Background = new SolidColorBrush(Color.FromRgb(148, 182, 148)),
            };

            // Create a grid inside the StackPanel to be able to have the textblocks and textboxes side by side.
            Grid addProductGrid = new Grid();
            addProductGrid.RowDefinitions.Add(new RowDefinition());
            addProductGrid.RowDefinitions.Add(new RowDefinition());
            addProductGrid.RowDefinitions.Add(new RowDefinition());
            addProductGrid.RowDefinitions.Add(new RowDefinition());
            addProductGrid.ColumnDefinitions.Add(new ColumnDefinition());
            addProductGrid.ColumnDefinitions.Add(new ColumnDefinition());
            // Add the grid to the StackPanel
            addProdcut.Children.Add(addProductGrid);

            // Create a TextBlock for the Name text.
            TextBlock name = new TextBlock
            {
                Text = "Name: ",
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 10, 2, 2),
            };
            addProductGrid.Children.Add(name);
            Grid.SetRow(name, 0);
            Grid.SetColumn(name, 0);

            // Create the TextBox next to the Name text.
            nameBox = new TextBox
            {
                Margin = new Thickness(0, 10, 10, 2),
            };
            addProductGrid.Children.Add(nameBox);
            Grid.SetRow(nameBox, 0);
            Grid.SetColumn(nameBox, 1);

            // Create a TextBlock for the Price text.
            TextBlock price = new TextBlock
            {
                Text = "Price: ",
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 2, 2, 2),
            };
            addProductGrid.Children.Add(price);
            Grid.SetRow(price, 1);
            Grid.SetColumn(price, 0);

            // Create the TextBox next to the Price text.
            priceBox = new TextBox
            {
                Margin = new Thickness(0, 2, 10, 2),
            };
            addProductGrid.Children.Add(priceBox);
            Grid.SetRow(priceBox, 1);
            Grid.SetColumn(priceBox, 1);

            // Create the TextBlock for the Description Text.
            TextBlock description = new TextBlock
            {
                Text = "Description: ",
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 2, 2, 2),
            };
            addProductGrid.Children.Add(description);
            Grid.SetRow(description, 2);
            Grid.SetColumn(description, 0);

            // Create the TextBox next to the Description text.
            descriptionBox = new TextBox
            {
                Margin = new Thickness(0, 2, 10, 2),
            };
            addProductGrid.Children.Add(descriptionBox);
            Grid.SetRow(descriptionBox, 2);
            Grid.SetColumn(descriptionBox, 1);

            // Create the TextBlock for the Image text.
            TextBlock image = new TextBlock
            {
                Text = "Image: ",
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 2, 2, 10),
            };
            addProductGrid.Children.Add(image);
            Grid.SetRow(image, 3);
            Grid.SetColumn(image, 0);

            // Create the TextBox next to the Image text.
            imageBox = new TextBox
            {
                Margin = new Thickness(0, 2, 10, 10),
            };
            addProductGrid.Children.Add(imageBox);
            Grid.SetRow(imageBox, 3);
            Grid.SetColumn(imageBox, 1);

            return addProdcut;
        }
        private StackPanel CreateProduct_ButtonPanel()
        {
            // Create the StackPanel for the buttons inside the Product section.
            StackPanel menuPanel = new StackPanel
            {
                Margin = new Thickness(5, 10, 50, 5),
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 300,
            };

            // Create Add Product Button.
            Button addProductButton = MainApplication.MainWindow.CreateButton("Add Product", 200, 0, 2, 2, 2, 3);
            menuPanel.Children.Add(addProductButton);
            addProductButton.Click += AddProduct_ButtonClick;

            // Create Change Product Button.
            Button changeProductButton = MainApplication.MainWindow.CreateButton("Change Product", 200, 0, 2, 2, 2, 3);
            menuPanel.Children.Add(changeProductButton);
            changeProductButton.Click += ChangeProduct_ButtonClick;
            return menuPanel;
        }
        private WrapPanel CreateProduct_ImageGrid()
        {
            // Create a WrapPanel for the product images.
            WrapPanel imageGrid = new WrapPanel
            {
                Margin = new Thickness(50, 10, 0, 5),
                Orientation = Orientation.Horizontal,
                Background = new SolidColorBrush(Color.FromRgb(148, 182, 148)),
            };

            // Loop trough the productImage list which contains all the image filepaths from the image csv file.
            foreach (string line in productImages)
            {
                Image productImage = MainApplication.MainWindow.CreateImage(line);
                productImage.Width = 75;
                productImage.Height = 75;
                productImage.Margin = new Thickness(2);
                productImage.Stretch = Stretch.Uniform;

                // Create a RadioButton and set it´s content to a image. To get the right image set the Tag to "line" which is the image title name.jpg
                checkBox = new RadioButton
                {
                    Content = productImage,
                    Tag = line,
                    Margin = new Thickness(0, 10, 0, 0),
                };
                imageGrid.Children.Add(checkBox);
                checkBox.Checked += ProductImageBox_Checked;
            }
            return imageGrid;
        }
        private StackPanel CreateProduct_ListPanel()
        {
            // Create the StackPanel for the listBox and buttons.
            StackPanel productListPanel = new StackPanel
            {
                Margin = new Thickness(10, 10, 50, 5),
                Orientation = Orientation.Vertical,
                Background = new SolidColorBrush(Color.FromRgb(148, 182, 148)),
            };

            // Create the ListBox containing all the current store products.
            availableProductsListBox = new ListBox
            {
                Background = new SolidColorBrush(Color.FromRgb(249, 251, 249)),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(20, 10, 20, 0),
                Width = 200,
                Height = 500,
            };
            productListPanel.Children.Add(availableProductsListBox);

            // Create the button to load a selected product to the TextBoxes.
            Button loadProductButton = MainApplication.MainWindow.CreateButton("Load Product", 200, 0, 10, 0, 0, 3);
            productListPanel.Children.Add(loadProductButton);
            loadProductButton.Click += LoadProduct_ButtonClick;

            // Create the button to remove a single product from the store products.
            Button removeProductButton = MainApplication.MainWindow.CreateButton("Remove Product", 200, 0, 2, 0, 0, 3);
            productListPanel.Children.Add(removeProductButton);
            removeProductButton.Click += RemoveProduct_ButtonClick;
            UpdateProductListBox();

            return productListPanel;
        }
        private StackPanel CreateDiscount_TextBoxPanel()
        {
            // Create the StackPanel for the TextBoxes in the Discount section.
            StackPanel addDiscount = new StackPanel
            {
                Margin = new Thickness(50, 10, 50, 5),
                Orientation = Orientation.Vertical,
                Background = new SolidColorBrush(Color.FromRgb(148, 182, 148)),
            };

            // Create a grid inside the StackPanel to be able to have the textblocks and textboxes side by side.
            Grid addDiscountGrid = new Grid();
            addDiscountGrid.RowDefinitions.Add(new RowDefinition());
            addDiscountGrid.RowDefinitions.Add(new RowDefinition());
            addDiscountGrid.RowDefinitions.Add(new RowDefinition());
            addDiscountGrid.RowDefinitions.Add(new RowDefinition());
            addDiscountGrid.ColumnDefinitions.Add(new ColumnDefinition());
            addDiscountGrid.ColumnDefinitions.Add(new ColumnDefinition());
            // Add the grid to the StackPanel
            addDiscount.Children.Add(addDiscountGrid);

            // Create a TextBlock for the Code text.
            TextBlock codeText = new TextBlock
            {
                Text = "Code: ",
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 25, 2, 2),
            };
            addDiscountGrid.Children.Add(codeText);
            Grid.SetRow(codeText, 0);
            Grid.SetColumn(codeText, 0);

            // Create the TextBox next to the Code Text.
            codeBox = new TextBox
            {
                Margin = new Thickness(0, 25, 10, 2),
            };
            addDiscountGrid.Children.Add(codeBox);
            Grid.SetRow(codeBox, 0);
            Grid.SetColumn(codeBox, 1);

            // Create a TextBlock for the Percent text.
            TextBlock percentageText = new TextBlock
            {
                Text = "Percent: ",
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 10, 2, 2),
            };
            addDiscountGrid.Children.Add(percentageText);
            Grid.SetRow(percentageText, 1);
            Grid.SetColumn(percentageText, 0);

            // Create the TextBox next to the Percent text.
            percentageBox = new TextBox
            {
                Margin = new Thickness(0, 10, 10, 2),
            };
            addDiscountGrid.Children.Add(percentageBox);
            Grid.SetRow(percentageBox, 1);
            Grid.SetColumn(percentageBox, 1);

            return addDiscount;
        }
        private StackPanel CreateDiscount_ButtonPanel()
        {
            // Create the StackPanel for the buttons inside the Discount section.
            StackPanel discountButtonPanel = new StackPanel
            {
                Margin = new Thickness(5, 10, 50, 5),
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 300,
            };

            // Create the Add Discount button.
            Button addDiscountButton = MainApplication.MainWindow.CreateButton("Add Discount", 200, 0, 2, 2, 2, 3);
            discountButtonPanel.Children.Add(addDiscountButton);
            addDiscountButton.Click += AddDiscount_ButtonClick;

            // Create the Change Discount button.
            Button changeDiscountButton = MainApplication.MainWindow.CreateButton("Change Discount", 200, 0, 2, 2, 2, 3);
            discountButtonPanel.Children.Add(changeDiscountButton);
            changeDiscountButton.Click += ChangeDiscount_ButtonClick;

            return discountButtonPanel;
        }
        private StackPanel CreateDiscount_MotivationPanel()
        {

            // Create the StackPanel for the ListBox.
            StackPanel motivationPanel = new StackPanel
            {
                Margin = new Thickness(50, 10, 0, 5),
                Orientation = Orientation.Vertical,
                Background = new SolidColorBrush(Color.FromRgb(148, 182, 148)),
            };

            TextBlock motivationTitle = new TextBlock
            {
                Text = "General advantages of offering discounts",
                FontSize = 30,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 10, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            motivationPanel.Children.Add(motivationTitle);

            TextBlock partOne = new TextBlock
            {
                Text = "1. Attracts Customers. Discounts are very attractive to customers and may " +
                "not only bring new clients but can also bring back previous customers. Discounting products and " +
                "services, particularly in-demand ones, is a good way to get attention. Especially in these days " +
                "of social media, word-of-mouth traffic can increase results on promotions exponentially quickly. " +
                "Your business is likely to experience increased traffic online or in-store (or both), and a boost in sales",
                FontSize = 15,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(15, 5, 15, 5),
            };
            motivationPanel.Children.Add(partOne);

            TextBlock partTwo = new TextBlock
            {
                Text = "2. Increases Sales. While the discounted items and services are generally the ones that will " +
                "garner the greatest sales, the increased traffic to your store or site means that other products and " +
                "services also enter customers’ awareness and become potential purchases. The increased traffic for " +
                "one item may lead to purchases of other items while they’re there.",
                FontSize = 15,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(15, 20, 15, 5),
            };
            motivationPanel.Children.Add(partTwo);

            TextBlock partThree = new TextBlock
            {
                Text = "3. Improves Image. There are plenty of circumstances in which a business can offer a discount " +
                "in order to improve its image. Targeted discounting (like seasonal or locational discounts, or for " +
                "certain subset of people) can greatly bolster a business’s reputation. For example, if a business was " +
                "to offer discounts to senior citizens or military personnel, cancer survivors, that business demonstrates " +
                "compassion and aligns itself with specific audiences (which in turns draws yet more attention).",
                FontSize = 15,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(15, 20, 15, 5),
            };
            motivationPanel.Children.Add(partThree);

            return motivationPanel;
        }
        private StackPanel CreateDiscount_ListPanel()
        {

            // Create the StackPanel for the listBox and buttons.
            StackPanel discountListPanel = new StackPanel
            {
                Margin = new Thickness(10, 10, 50, 5),
                Orientation = Orientation.Vertical,
                Background = new SolidColorBrush(Color.FromRgb(148, 182, 148)),
            };

            // Create the ListBox containing all the current discount codes.
            discountListBox = new ListBox
            {
                Background = new SolidColorBrush(Color.FromRgb(249, 251, 249)),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(20, 10, 20, 0),
                Width = 200,
                Height = 500,
            };
            discountListPanel.Children.Add(discountListBox);

            // Create the button to load selected discount code to the TextBoxes.
            Button loadDiscountButton = MainApplication.MainWindow.CreateButton("Load Code", 200, 0, 10, 0, 0, 3);
            discountListPanel.Children.Add(loadDiscountButton);
            loadDiscountButton.Click += LoadDiscount_ButtonClick;

            // Create the button to remove a single discound code..
            Button removeDiscountButton = MainApplication.MainWindow.CreateButton("Remove Code", 200, 0, 2, 0, 0, 3);
            discountListPanel.Children.Add(removeDiscountButton);
            removeDiscountButton.Click += RemoveDiscount_ButtonClick;

            UpdateDiscountListBox();
            return discountListPanel;
        }
        private List<string> ImageListDeserialization()
        {
            // Create a temporary list and load it with all image filepaths from the csv file for productimages.
            List<string> tempList = new List<string>();

            // Create an stringarray and fill it with imagefilepaths.
            string[] fromFile = File.ReadAllLines(MainApplication.MainWindow.tempAddress + "ProductImages.csv");

            // Iterate through everyline in the stringarray and split into parts on every \, then only add the fourth parts to the tempList (the part containing the filename).
            foreach (string line in fromFile)
            {
                string[] parts = line.Split(@"\");
                tempList.Add(parts[5]);
            }

            return tempList;
        }
        private void UpdateDiscountListBox()
        {
            // Update the available product ListBox.
            discountListBox.Items.Clear();
            discountList = MainApplication.MainWindow.DiscountFileDeserialization();
            // Iterate through the discountList and add every code with percentage to the ListBox above.
            foreach (DiscountCode code in discountList)
            {
                discountListBox.Items.Add($"{code.Name} ({Math.Round((1 - code.Code) * 100, 0)}%)");
            }
        }
        private void UpdateProductListBox()
        {
            // Update the available product ListBox.
            availableProductsListBox.Items.Clear();
            products = MainApplication.MainWindow.FileDeserialisation(MainApplication.MainWindow.tempAddress + "Products.csv");
            foreach (KeyValuePair<Product, int> p in products)
            {
                availableProductsListBox.Items.Add($"{p.Key.Name} ({p.Key.Price}kr)");
            }
        }
        private void AddDiscount_ButtonClick(object sender, RoutedEventArgs e)
        {
            // Linq to check if the title of the new discount code already exists in the discountlist, if it does give a message.
            if (discountList.Where(x => x.Name == codeBox.Text).Any())
            {
                MessageBox.Show($"{codeBox.Text} is already in list");
                return;
            }
            // check if the percentage is filled with correct format then check to see if all fields are filled
            decimal d;
            if (decimal.TryParse(percentageBox.Text, out d))
            {
                // if percentageBox are a valid input, check if all fields are filled
                if (codeBox.Text == string.Empty || percentageBox.Text == string.Empty)
                {
                    // if one ore more fields are missing, give a error message
                    MessageBox.Show("You have to fill in all the boxes!");
                }
                else
                {
                    string toFile = $"{codeBox.Text.ToUpper()},{1 - (decimal.Parse(percentageBox.Text) / 100)}";

                    // Create a new StreamWriter and set it to AppendText to Discounts.csv
                    StreamWriter sw = File.AppendText(MainApplication.MainWindow.tempAddress + "Discounts.csv");
                    // Use StreamWriter to write toFile to discounts.csv
                    sw.WriteLine(toFile);
                    // Close the stream.
                    sw.Close();

                    // Show a message that the code was added, then clear the fields for code and percentage.
                    MessageBox.Show("Discount code added!");
                    codeBox.Clear();
                    percentageBox.Clear();
                }
            }
            // If percentageBox don´t contain a valid decimal, give a message.
            else
            {
                MessageBox.Show("Please enter a valid decimal");
                return;
            }
            // Update the discount ListBox.
            UpdateDiscountListBox();
        }
        private void LoadDiscount_ButtonClick(object sender, RoutedEventArgs e)
        {
            // If a discount code is selected in the ListBox
            try
            {
                // Get the index from the selected discount in the listbox.
                int selectedIndex = discountListBox.SelectedIndex;
                // Modify the object from selected index
                DiscountCode d = discountList.ElementAt(selectedIndex);
                // Fill in the textboxes with the information from the discount.
                decimal showPercent = (1 - d.Code) * 100;
                codeBox.Text = d.Name;
                percentageBox.Text = Math.Round(showPercent).ToString();
            }
            // If no discount are selected in the ListBox throw a mesasge.
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("No discount selected!");
            }
        }
        private void RemoveDiscount_ButtonClick(object sender, RoutedEventArgs e)
        {
            // If a discount is selected in the ListBox.
            try
            {
                // Initializing a int and assign the value from the selexted index in the listbox.
                int selectedIndex = discountListBox.SelectedIndex;
                // Remove selected discount from the list.
                discountList.RemoveAt(selectedIndex);
                // Remove selexted discount from the ListBox.
                discountListBox.Items.RemoveAt(selectedIndex);


                string toFile = string.Empty;

                // Iterate through the discountList and for every item, att it toFile.
                foreach (DiscountCode code in discountList)
                {
                    string currentItem = $"{code.Name},{code.Code}{Environment.NewLine}";
                    toFile += currentItem;
                }
                // Write All Text to the discount csv file.
                File.WriteAllText(MainApplication.MainWindow.tempAddress + "Discounts.csv", toFile);

                MessageBox.Show("Discount removed!");
            }
            // If no discount are selected in the ListBox, throw a message.
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Your discountlist is either empty or no discount is selected");
            }
        }
        private void ChangeDiscount_ButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the index from the selected product in the list and assign it to the int variable.
                int selectedIndex = discountListBox.SelectedIndex;
                // Modify the object from selected index
                DiscountCode d = discountList.ElementAt(selectedIndex);
                d.Name = codeBox.Text;
                d.Code = 1 - (decimal.Parse(percentageBox.Text) / 100);

                string toFile = string.Empty;

                // iterate through the discountlist and add every object with , between each field and add it toFile.
                foreach (DiscountCode code in discountList)
                {
                    string currentItem = $"{code.Name},{code.Code}{Environment.NewLine}";
                    toFile += currentItem;
                }

                // Write toFile to csv and give message and update the ListBox.
                File.WriteAllText(MainApplication.MainWindow.tempAddress + "Discounts.csv", toFile);
                UpdateDiscountListBox();
                MessageBox.Show("Discount updated!");
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("No discount loaded!");
            }

        }
        private void AddProduct_ButtonClick(object sender, RoutedEventArgs e)
        {
            // Linq to check if the title of the new product already exists in the productList, if it does give a message.
            if (products.Where(x => x.Key.Name == nameBox.Text).Any())
            {
                MessageBox.Show($"{nameBox.Text} is already in list");
                return;
            }
            // if the priceBox is filled with correct format, check to see if all fields are filled
            decimal d;
            if (decimal.TryParse(priceBox.Text, out d))
            {
                // if priceBox are a valid input, check if all fields are filled
                if (nameBox.Text == string.Empty || priceBox.Text == string.Empty || imageBox.Text == string.Empty || descriptionBox.Text == string.Empty)
                {
                    // if one ore more fields are missing, give a error message
                    MessageBox.Show("You have to fill in all the boxes!");
                }
                else
                {

                    // otherwise add the product to the Productlist.csv file
                    string toFile = $"{nameBox.Text},{priceBox.Text},{imageBox.Text},{descriptionBox.Text}";
                    // Create a new StreamWriter and set it to AppendText to Discounts.csv
                    StreamWriter sw = File.AppendText(MainApplication.MainWindow.tempAddress + "Products.csv");
                    // Use StreamWriter to write toFile to discounts.csv
                    sw.WriteLine(toFile);
                    // Close the stream.
                    sw.Close();

                    // Show a message that the Product was added, then clear the fields. 
                    MessageBox.Show("Your product was added!");
                    nameBox.Clear();
                    priceBox.Clear();
                    imageBox.Clear();
                    descriptionBox.Clear();
                }
            }
            // If the priceBox don´t contains a valid decimal, give a message.
            else
            {
                // if priceBox use a invalid input, give a message
                MessageBox.Show("Please enter a valid decimal");
                return;
            }
            UpdateProductListBox();
        }
        private void LoadProduct_ButtonClick(object sender, RoutedEventArgs e)
        {
            // If a Product is selected in the ListBox
            try
            {
                // Get the index from the selected product in the listbox.
                int selectedIndex = availableProductsListBox.SelectedIndex;
                // Create a new product from the selected index in the list.
                var x = products.ElementAt(selectedIndex);
                // Fill in the textboxes with the information from the product.
                nameBox.Text = x.Key.Name;
                priceBox.Text = x.Key.Price.ToString();
                descriptionBox.Text = x.Key.Description;
                imageBox.Text = x.Key.Image;

            }
            // If no Product are selected, throw a mesasge.
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("No product selected!");
            }
        }
        private void RemoveProduct_ButtonClick(object sender, RoutedEventArgs e)
        {
            // If a product is selected in the ListBox.
            try
            {
                // Initializing a int with the index from the selexted product in the ListBox.
                int selectedIndexone = availableProductsListBox.SelectedIndex;
                // Remove selected index from the ListBox.
                availableProductsListBox.Items.RemoveAt(selectedIndexone);
                // Remove the selexted index from the product dictionary.
                products.Remove(products.ElementAt(selectedIndexone).Key);

                string toFile = string.Empty;
                // Iterate through the ProductDictionary and add every keyvaluepair toFile
                foreach (KeyValuePair<Product, int> kvp in products)
                {
                    string currentItem = $"{kvp.Key.Name},{kvp.Key.Price},{kvp.Key.Image},{kvp.Key.Description}{Environment.NewLine}";
                    toFile += currentItem;
                }
                // Write all text to product csv file.
                File.WriteAllText(MainApplication.MainWindow.tempAddress + "Products.csv", toFile);
                MessageBox.Show("Product removed!");

            }
            // If no Product are selected in the ListBox, throw a message.
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("No item selected!");
            }
        }
        private void ChangeProduct_ButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                int selectedIndex = availableProductsListBox.SelectedIndex;
                // Modify the object from selected index
                Product p = products.ElementAt(selectedIndex).Key;
                p.Name = nameBox.Text;
                p.Price = decimal.Parse(priceBox.Text);
                p.Image = imageBox.Text;
                p.Description = descriptionBox.Text;

                string toFile = string.Empty;

                // iterate through the productlist and add every object with , between each field and add it toFile.
                foreach (KeyValuePair<Product, int> kvp in products)
                {
                    string currentItem = $"{kvp.Key.Name},{kvp.Key.Price},{kvp.Key.Image},{kvp.Key.Description}{Environment.NewLine}";
                    toFile += currentItem;
                }

                // Write toFile to csv.
                File.WriteAllText(MainApplication.MainWindow.tempAddress + "Products.csv", toFile);
                UpdateProductListBox();
                MessageBox.Show("Product updated!");
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("No Product loaded!");
            }
        }
        private void ProductImageBox_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton checkBoxOne = (RadioButton)sender;
            string productName = (string)checkBoxOne.Tag;
            imageBox.Text = productName;
        }
        private void DiscountSection_ButtonClick(object sender, RoutedEventArgs e)
        {
            // Hide panels.
            productTextBoxPanel.Visibility = Visibility.Hidden;
            productImagePanel.Visibility = Visibility.Hidden;
            productButtonPanel.Visibility = Visibility.Hidden;
            productListPanel.Visibility = Visibility.Hidden;

            //Show panels.
            discountTextBoxPanel.Visibility = Visibility.Visible;
            discountButtonPanel.Visibility = Visibility.Visible;
            motivationPanel.Visibility = Visibility.Visible;
            discountListPanel.Visibility = Visibility.Visible;
        }
        private void ProductSection_ButtonClick(object sender, RoutedEventArgs e)
        {
            //Show Panels.
            discountTextBoxPanel.Visibility = Visibility.Hidden;
            discountButtonPanel.Visibility = Visibility.Hidden;
            motivationPanel.Visibility = Visibility.Hidden;
            discountListPanel.Visibility = Visibility.Hidden;

            // Hide Panels.
            productTextBoxPanel.Visibility = Visibility.Visible;
            productImagePanel.Visibility = Visibility.Visible;
            productButtonPanel.Visibility = Visibility.Visible;
            productListPanel.Visibility = Visibility.Visible;
        }
    }
}


