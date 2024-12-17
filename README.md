# Travel Agency Website  

**A project by Group 1: Neha, Stephen, Jalil, and Taiwo**  

---

## Table of Contents  
1. [Project Overview](#project-overview)  
2. [Features](#features)  
3. [Technologies Used](#technologies-used)  
4. [Database Schema](#database-schema)  
5. [Setup Instructions](#setup-instructions)  
6. [Branches](#branches)  
7. [Usage](#usage)  
8. [Contributors](#contributors)  
9. [Additional Notes](#additional-notes)  

---

## Project Overview  
The **Travel Agency Website** is an ASP.NET Core-based web application that allows users to browse, purchase, and manage travel packages. It includes functionalities for **user management**, **virtual wallets**, **travel preferences**, and **comprehensive bookings**.  

---

## Features  
- **User Authentication and Role Management** (via ASP.NET Identity)  
- **Travel Package Browsing and Booking**  
- **Virtual Wallet System with Transactions**  
- **Customer Travel Preferences Input**  
- **Detailed Price Breakdown for Purchases**  
- **Supplier and Product Management**  

---

## Technologies Used  
- **ASP.NET Core MVC**  
- **C#**  
- **SQL Server**  
- **Entity Framework Core**  
- **Bootstrap** (for UI)  
- **GitHub for Version Control**  

---

## Database Schema  
The database consists of **33 tables** that manage users, bookings, purchases, and associated travel data:  

- **_EFMigrationsHistory** – Tracks Entity Framework migrations.  
- **Affiliations** – Manages partner or affiliation details.  
- **Agencies** – Stores travel agency details.  
- **AgentPasswords** – Handles agent authentication.  
- **Agents** – Contains information about agents.  
- **AspNetRoleClaims** – ASP.NET Identity role claims.  
- **AspNetRoles** – Manages application roles.  
- **AspNetUserClaims** – User claims for ASP.NET Identity.  
- **AspNetUserLogins** – External login providers.  
- **AspNetUserRoles** – Relationship between users and roles.  
- **AspNetUsers** – User information (customers, agents).  
- **AspNetUserTokens** – User tokens for authentication.  
- **BookingDetails** – Detailed information about bookings.  
- **Bookings** – Manages customer bookings.  
- **Classes** – Class types for travel or packages.  
- **CreditCards** – Stores customer payment details.  
- **Customers** – Stores registered customer data.  
- **Customers_Rewards** – Tracks reward points for customers.  
- **Employees** – Information about employees of the travel agency.  
- **Fees** – Stores additional fee details for packages.  
- **Packages** – List of travel packages.  
- **Packages_Products_Suppliers** – Relationships between packages, products, and suppliers.  
- **Products** – Travel-related products or add-ons.  
- **Products_Suppliers** – Manages product-supplier relationships.  
- **Purchases** – Tracks purchase transactions.  
- **Regions** – Travel regions or destinations.  
- **Rewards** – Reward details for customers.  
- **SupplierContacts** – Contact details for suppliers.  
- **Suppliers** – Stores supplier information.  
- **TravelPreferences** – Customers' travel preferences.  
- **TripTypes** – Different types of travel trips.  
- **Wallets** – Virtual wallet system for payments.  
- **WalletTransactions** – Tracks wallet-based transactions.  

---

## Setup Instructions  

### Prerequisites  
- **Visual Studio 2022** or higher  
- **.NET Core SDK 6.0** or higher  
- **SQL Server** (Express or Full version)  

### Steps to Set Up the Project  

1. **Clone the Repository**  
   ```bash  
   git clone https://github.com/nehauttwani/TravelTales-Asp.Net.git  
---
Switch to the Correct Branch

bash
Copy code
git checkout test-merge  
Restore NuGet Packages

Open the project in Visual Studio.
Right-click on the Solution and select Restore NuGet Packages.
Set Up the Database

Attach the provided SQL Server database file.
Update the connection string in appsettings.json to match your SQL Server configuration:
json
Copy code
"ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=TravelExperts;Trusted_Connection=True;"
}
Run Database Migrations
Open the Package Manager Console and run:

bash
Copy code
Update-Database  
Run the Application

Build and run the application by pressing F5 or clicking Start.
Branches
test-merge: Latest version of the Travel Tales ASP.NET website.
Usage
Access the homepage at http://localhost:{port}.
Register or log in to explore travel packages.
Use the virtual wallet system to simulate purchases.
View and manage travel preferences.
Contributors
Neha Uttwani
Stephen Garo
Jalil Mohseni
Taiwo Adejoro
Additional Notes
If you encounter any issues, please contact the contributors or raise an issue in the repository.
