# .NET-Core-6-MVC-CRUD

BulkyBook is web book store application built in **Asp.Net Core 6 (C#)**. For backend data storage and retrieving was used **MSSQL**.

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

### Built With

* [C#](https://nextjs.org/)
* [.NET 6 (Core)](https://reactjs.org/)
* [Bootstrap](https://vuejs.org/)
* [Html](https://angular.io/)
* [CSS](https://svelte.dev/)
* [JQuery](https://laravel.com)
* [SignalR](https://getbootstrap.com)
* [.ML NET - Matrix Factorization](https://jquery.com)

<p align="right">(<a href="#top">back to top</a>)</p>

