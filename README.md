## About The Project

BulkyBook is web book store application built in **Asp.Net Core 6 (C#)** using **repository pattern**. For backend data storage and retrieving was used **MSSQL**.

Web application enables different roles on the system: Admin, Individual, Company and Employee.
Responsabilities for admin:
  - Managing categories
  - Managing cover types
  - Managing companies
  - Managing users
  - Managing/processing orders
  - Managing authors
  - Managing products/books
  - Managing profile info.

Admin is also provided with notifications done with SignalR. It works in the way that he is instantly notified when user makes a purchase.

Individual user is provided with following funcitionalities:
  - Login/Register + Login with Facebook or Gmail account
  - Display of books and their details
  - Search based on author name, title and category
  - Add to cart
  - Purchase/order a product
  - Stripe payment
  - Seacrh through made orders by their status (completed, processing, etc..)
  - Recommended products based on their preferences/likings
  - Rating/commenting products
  - Profile info edit
  - Adding book to favourite (bookmarking)

Display of recommended products is done with ML .NET applying MatrixFactorization. It functions on the principle of what other people with similar
preferences have liked and based off of that, products are shown to specific user. The goal is to offer a buyer products that he would most likely
purchase.

<p align="right">(<a href="#top">back to top</a>)</p>

### Built With

* [C#](https://docs.microsoft.com/en-us/dotnet/csharp/)
* [.NET 6 (Core)](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
* [MVC](https://developer.mozilla.org/en-US/docs/Glossary/MVC)
* [Bootstrap](https://getbootstrap.com)
* [Html](https://html.com/)
* [CSS](https://developer.mozilla.org/en-US/docs/Web/CSS)
* [JQuery](https://jquery.com)
* [SignalR](https://dotnet.microsoft.com/en-us/apps/aspnet/signalr))
* [.ML NET - Matrix Factorization](https://jquery.com)

<p align="right">(<a href="#top">back to top</a>)</p>

<!-- GETTING STARTED -->
## Getting Started

This is an example of how you may give instructions on setting up your project locally.
To get a local copy up and running follow these simple example steps.

### Prerequisites

This is an example of how to list things you need to use the software and how to install them.
* Visual Studio 2022
* SQL Management Studio


### Installation

1. Clone the repo
   ```sh
   git clone https://github.com/Lejla1502/.NET-Core-6-MVC-CRUD.git
   ```
2. Change appsettings sql connection string if needed
   
3. Run the app and register

<p align="right">(<a href="#top">back to top</a>)</p>


