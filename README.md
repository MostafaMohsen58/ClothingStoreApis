# Clothing Store API

A RESTful API backend service for an e-commerce clothing store built with ASP.NET Core. This API provides comprehensive endpoints for managing products, orders, user accounts, and more.

## Features

- **User Authentication & Authorization**
  - User registration and login
  - External login support
  - Password reset functionality
  - JWT-based authentication

- **Product Management**
  - Product categories and subcategories
  - Multiple product images support
  - Product reviews and ratings
  - Size variations

- **Shopping Features**
  - Shopping cart functionality
  - Wishlist management
  - Order processing
  - Stripe payment integration

- **Order Management**
  - Order tracking
  - Billing address management
  - Order history
  - Additional notes support

## Project Structure

- `Controllers/` - API endpoints implementation
  - Account management
  - Product operations
  - Order processing
  - Shopping cart
  - Wishlist
  - Reviews

- `Models/` - Data models and entities
- `DTO/` - Data Transfer Objects for API requests/responses
- `Helpers/` - Utility classes including email and image handling
- `RepoServices/` - Repository pattern implementation
- `IRepoServices/` - Service interfaces

## Technologies Used

- ASP.NET Core
- Entity Framework Core
- SQL Server
- Stripe Payment Integration
- JWT Authentication

## Getting Started

1. **Prerequisites**
   - .NET 6.0 SDK or later
   - SQL Server
   - Visual Studio 2022 or preferred IDE

2. **Configuration**
   - Update the connection string in `appsettings.json`
   - Set up Stripe API keys for payment processing

3. **Database Setup**
   ```bash
   dotnet ef database update```