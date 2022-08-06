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

Main difference between "Idividual" and "Company" user is that individual is redirected to stripe immediately after placing the order and is required of them
to pay for the product right after placing the order, while "company" user can first place the order and pay later. The limit for paying is one month after 
placing the order.

<p align="right">(<a href="#top">back to top</a>)</p>

### Screens
![scr1](https://user-images.githubusercontent.com/22219433/183261040-9d36c922-9890-4602-8455-a233dccc5577.png)

![scr2](https://user-images.githubusercontent.com/22219433/183261047-643b4ee7-1b76-4774-82f1-91ec9c2def7a.png)

![scr3](https://user-images.githubusercontent.com/22219433/183261050-509d81a2-f5ca-42be-828b-23a00b305ce6.png)

![scr4](https://user-images.githubusercontent.com/22219433/183261055-e195a555-e6a6-4eae-b183-81fc32d17b96.png)

![scr5](https://user-images.githubusercontent.com/22219433/183261060-4a4e2fe6-1729-4fe4-85b4-bb7a5aef03ee.png)

![scr6](https://user-images.githubusercontent.com/22219433/183261063-1b03187c-70f0-4296-ada7-256170bfa43b.png)

![scr7](https://user-images.githubusercontent.com/22219433/183261067-84fe81c8-a492-4288-a597-1ea8f605dbae.png)

![scr8_a](https://user-images.githubusercontent.com/22219433/183261405-c726d7dd-f939-4f04-b8aa-082fe124c07a.png)

![scr8_b](https://user-images.githubusercontent.com/22219433/183261406-97d91127-fa04-4ad4-91a7-13c68c1f1a62.png)

![scr9](https://user-images.githubusercontent.com/22219433/183261082-0160bd39-eebb-42da-afca-629e530dc01b.png)

![scr10](https://user-images.githubusercontent.com/22219433/183261086-e47d8e28-b34a-4318-b1ac-7aa072ff9a1a.png)

![scr11](https://user-images.githubusercontent.com/22219433/183261091-673b696d-8e4c-44d0-a42c-4cd218f141da.png)


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


