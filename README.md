# TechXpress

## Overview
TechXpress is a robust e-commerce platform specifically designed for technology products. Built with ASP.NET Core, it provides a comprehensive solution for managing tech product sales, user accounts, and order processing.

## Architecture

### Tech Stack
- **Backend**: C# with ASP.NET Core 6.0
- **ORM**: Entity Framework Core
- **Database**: SQL Server
- **Authentication**: ASP.NET Identity with JWT support
- **Frontend**: ASP.NET MVC with JavaScript/Bootstrap
- **Payment Processing**: Stripe API

### Project Structure
The solution follows clean architecture principles with clear separation of concerns:

- **TechXpress.Models**: Domain entities and DTOs
- **TechXpress.DataAccess**: Database context, repositories, and migrations
- **TechXpress.Services**: Business logic and service implementations
- **TechXpress.Web**: MVC controllers, views, and API endpoints

## Key Features

- **Product Management**: Comprehensive catalog with categories, reviews, and inventory tracking
- **User Authentication**: Secure account management with role-based access control
- **Shopping Experience**: Cart functionality with persistent storage
- **Checkout Process**: Multi-step checkout with address validation
- **Payment Integration**: Secure payment processing via Stripe
- **Order Management**: Complete order lifecycle from creation to fulfillment
- **Admin Dashboard**: Tools for inventory, user, and order management

## Getting Started

### Prerequisites
- .NET 6.0 SDK
- SQL Server (2019+ recommended)
- Visual Studio 2022 or equivalent IDE
- Stripe account for payment processing

### Installation

1. Clone the repository:
```
git clone https://github.com/yourusername/TechXpress.git
```

2. Update the connection string in `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=TechXpress;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

3. Configure Stripe settings in `appsettings.json`:
```json
"Stripe": {
  "SecretKey": "YOUR_STRIPE_SECRET_KEY",
  "PublishableKey": "YOUR_STRIPE_PUBLISHABLE_KEY"
}
```

4. Run database migrations:
```
dotnet ef database update --project TechXpress.DataAccess --startup-project TechXpress.Web
```

5. Launch the application:
```
dotnet run --project TechXpress.Web
```

## Database Schema

The application uses a relational database with the following core entities:
- `User`: Customer accounts and authentication
- `Product`: Tech product details and inventory
- `Category`: Product categorization
- `Order`/`OrderItem`: Customer purchases
- `ShoppingCart`/`CartItem`: In-progress selections
- `Review`: Product ratings and feedback
- `Payment`: Transaction records

## API Documentation

The RESTful API endpoints are available at `/api/` with JWT authentication:

- `GET /api/products`: List all products
- `GET /api/products/{id}`: Get product details
- `GET /api/categories`: List all categories
- `POST /api/cart`: Add item to cart
- `POST /api/orders`: Create new order
- `GET /api/orders/{id}`: Get order details

## Deployment

The application is configured for CI/CD deployment with Azure:

1. Build the solution:
```
dotnet build --configuration Release
```

2. Publish the web project:
```
dotnet publish TechXpress.Web --configuration Release
```

3. Deploy using Azure Web App Service or your preferred hosting platform

## Code Standards

This project follows:
- Microsoft's C# coding conventions
- RESTful API design principles
- Repository pattern for data access
- Dependency injection for service resolution
- Comprehensive exception handling

## Performance Considerations

- Database queries are optimized with appropriate indexes
- EF Core is configured with query tracking optimization
- Static resources use appropriate caching headers
- Pagination is implemented for large data sets

## Security Implementation

- HTTPS is enforced across the application
- SQL parameters prevent injection attacks
- Cross-site scripting (XSS) protection is enabled
- CSRF tokens are required for state-changing operations
- Password requirements enforce strong credentials
- Role-based authorization controls access to sensitive operations

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details. 