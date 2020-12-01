# Preface

Our final assignment for the first course was to create a GUI Application to simulate a Store in WPF programmatically (not using XAML). 
The **Main application** should simulate the store with some basefunction. 
The **second application** _(optional for higher grade)_ should simulate the store from a staff perspective to add and change the stock and discountcodes. 

# Documentation

I started by drawing a rough sketch in Photoshop for the design. 
I felt that I wanted to work with StackPanels and WrapPanels instead of building the GUI with nestled grids, so I don´t have to keep track of rows and columns to  
place things where I want them. But on some places I use grids inside StackPanels for obvious reasons. 
I choose WrapPanel for the products because it adapts to the size of the window. 

When we launch the MainApplication the folder Utilities will be copied from the project folder to `C:\Windows\Temp.` I am using the Temp folder to read all 
the csv files and images. I found this part to be quite tricky. After some time spent searching and reading **MSDN** I finally got this to work. I only added
this function to the **main application** so in order to get this to work you need to start the main application first to copy all the necessary files to Temp. 

In the startmethod I began by placeing all the StackPanels and WrapPanel in the correct position in the main grid. When you press the place order button 
the visibillity will change from visible to hidden for everything except for the menuPanel that I change to collapsed so it won´t take any space in the                             
background. If I use hidden instead the content in checkout won´t be centered. I am creating a StackPanel for the receipt when the place order button is used. 

When the csv files are deserialized, every product is going through a temporary List that the method are returning. I call this method in the beginning of the program to fill my productList which is a Dictionary. In the method to create the WrapPanel i start by creating an empty `List<StackPanel>` then I am looping through my Dictionary for the products and every product is going through the method to create a StackPanel for the product. After this the StackPanel with the product is going to be added to the `List<StackPanel>` and then to the WrapPanel that displays all the products. By doing this every product in the csv file will dynamically be created to a StackPanel that is added to the WrapPanel that displays all the current products. 
I think this was the most difficult part of this project. After discussing this problem with a friend of mine he told me that I can create a List of _**anything**_. That´s when I realised that if I created a List with StackPanels I would be able to solve this problem. Big thanks to Sebastian!

Another problem I had was when I had to connect the + and - buttons on the products to the same eventhandler, one to add the product and one to remove the product from the cart. I solved this problem by using Tag:s. I used the title (The name of the product) as the Tag for every product and by doing this, the correct price will be added / removed from the cart. And in the **second application** I made sure that you can´t add a product with the same name as a product that already exists in the current stock. 

When I was half way through the main application I realised that I didn´t want the products to stack on top of each other in the cart like this:
* Milk
* Milk
* Milk

I want the cart to display each product once, and with the amount like this:  **Product x3** instead. 
And to fix this I had at least two options
* Add a counter to the product class
* Change the productList from `List<Product>` to `Dictionary<Product, int>`

I wanted to learn Dictionaries so I went with that option. It would had saved me a lot of time going with the first option above, but hey - I want to learn as much as possible so **don´t always go the easy way** :) 
I changed the deserialization to use Dict instead of List and for the Key I add Product, and for the value I add the amount, so when the user press + or minus button I increase / decrease the value by one. 

When the application grew I got tired of writing ++ (increase), -- (decrease) so I changed it to linq
```csharp 
Sum = cart.Sum(x => x.Key.Price * x.Value)
```
And by doing this I no longer have to think about ++ or -- since it always sum´s the entire cart. 

I added a detail to the `Place Order` button so that it will display the current amount of products in cart, and I did this by making the button global and for every product added or removed from cart I update the button with linq 
```csharp
 checkout.Content = $"Place Order ({cart.Sum(x => x.Value)})";
```

# Conclusion

When I started this education I had little to none experience with programming. I hade never seen c# syntax and I had no idea what Visual Studio was. 
Hard work have really paid out for me, this is a product that I have created after 13 weeks of education. From 0 knowledge to this application in the short time I have been programming is something that I am really proud of! I have learned a lot in these 13 weeks and I am super motivated to keep learning and becoming better at programming.  


# Screenshots

![](Projektarbete%20Screenshots/Main.png)

![](Projektarbete%20Screenshots/OrderPlaced.png)

![](Projektarbete%20Screenshots/ChangeProduct.png)

![](Projektarbete%20Screenshots/Discount.png)

# Video Documentation (Swedish)

Comming soon.
