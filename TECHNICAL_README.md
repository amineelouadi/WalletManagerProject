# Documentation Technique - Wallet Management API

## Architecture du Projet

Ce projet suit les principes de l'architecture propre (Clean Architecture) pour assurer la séparation des préoccupations et faciliter la maintenabilité du code.

### Couches Architecturales

1. **Couche de Domaine** (`WalletManagement.Domain`)
   - Contient les entités du domaine métier
   - Définit les interfaces des repositories (pattern Repository)
   - Ne dépend d'aucune autre couche

2. **Couche d'Application** (`WalletManagement.Application`)
   - Contient la logique métier et les cas d'utilisation
   - Implémente les services qui orchestrent les opérations
   - Définit les DTOs (Data Transfer Objects) pour les communications
   - Dépend uniquement de la couche de domaine

3. **Couche d'Infrastructure** (`WalletManagement.Infrastructure`)
   - Implémente les repositories définis dans la couche de domaine
   - Gère l'accès aux données via Entity Framework Core
   - Configure la persistance et les migrations de base de données
   - Dépend des couches de domaine et d'application

4. **Couche de Présentation/API** (`WalletManagement.API`)
   - Expose les fonctionnalités via des endpoints REST
   - Gère l'authentification et l'autorisation
   - Configure les middlewares et les services ASP.NET Core
   - Dépend des couches d'application et d'infrastructure

5. **Couche Interface Utilisateur** (`WalletManagement.Blazor`)
   - Implémente l'interface utilisateur avec Blazor WebAssembly
   - Consomme l'API REST de la couche de présentation
   - Gère les états de l'application côté client
   - Dépend indirectement des DTOs de la couche d'application

## Modèles de Données

### Entités de Domaine

1. **User**
   - Dérive de `IdentityUser` pour l'authentification
   - Propriétés supplémentaires : FirstName, LastName, Role, etc.
   - Relations : One-to-Many avec Wallet et Transaction

2. **Wallet**
   - Propriétés : Id, Name, Description, Balance, Currency, etc.
   - Relations : Many-to-One avec User, One-to-Many avec Transaction

3. **Transaction**
   - Propriétés : Id, Amount, Type (Deposit, Withdrawal, Transfer), Description, etc.
   - Relations : Many-to-One avec User et Wallet(s)

## Flux d'Authentication

1. **Inscription (Register)**
   - La requête est reçue par `AuthController`
   - Validation des données d'entrée via le DTO `RegisterUserDTO`
   - Traitement par `UserService` qui utilise `UserRepository`
   - Création de l'utilisateur avec ASP.NET Identity
   - Génération d'un token JWT pour l'utilisateur

2. **Connexion (Login)**
   - La requête est reçue par `AuthController`
   - Validation des identifiants via le DTO `LoginUserDTO`
   - Traitement par `UserService` qui vérifie les identifiants
   - Génération d'un token JWT pour l'utilisateur authentifié

## Flux des Transactions

1. **Création d'une Transaction**
   - La requête est reçue par `TransactionsController`
   - Validation des données d'entrée via le DTO `CreateTransactionDTO`
   - Traitement par `TransactionService` qui détermine le type de transaction
   - Selon le type (dépôt, retrait, transfert), différentes logiques sont appliquées
   - Mise à jour des soldes des portefeuilles concernés via `WalletService`
   - Persistance de la transaction via `TransactionRepository`

## Sécurité

### Configuration JWT

```csharp
services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = Configuration["JwtSettings:Issuer"],
        ValidAudience = Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(Configuration["JwtSettings:Secret"]))
    };
});
```

### Autorisation Basée sur les Rôles

```csharp
services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => 
        policy.RequireRole("Admin"));
    
    options.AddPolicy("RequireUserRole", policy => 
        policy.RequireRole("User", "Admin"));
});
```

## Tests Unitaires et d'Intégration

Les tests sont organisés dans des projets séparés :

- `WalletManagement.UnitTests` : Tests unitaires des services et repositories
- `WalletManagement.IntegrationTests` : Tests d'intégration des contrôleurs API

## Configuration de la Base de Données

### Migration avec Entity Framework Core

```bash
# Créer une nouvelle migration
dotnet ef migrations add InitialCreate -p src/WalletManagement.Infrastructure -s src/WalletManagement.API

# Appliquer les migrations à la base de données
dotnet ef database update -p src/WalletManagement.Infrastructure -s src/WalletManagement.API
```

### Structure de la Base de Données

- Tables ASP.NET Identity : AspNetUsers, AspNetRoles, AspNetUserRoles, etc.
- Tables métier : Wallets, Transactions

## Interfaces REST API

### Authentication

```
POST /api/auth/register
{
  "username": "user1",
  "email": "user1@example.com",
  "password": "Password123!",
  "confirmPassword": "Password123!",
  "firstName": "John",
  "lastName": "Doe"
}

POST /api/auth/login
{
  "username": "user1",
  "password": "Password123!"
}

POST /api/auth/logout
```

### Wallets

```
GET /api/wallets
Authorization: Bearer {token}

GET /api/wallets/{id}
Authorization: Bearer {token}

POST /api/wallets
Authorization: Bearer {token}
{
  "name": "Mon Portefeuille Principal",
  "description": "Portefeuille pour les dépenses quotidiennes",
  "currency": "EUR"
}

PUT /api/wallets/{id}
Authorization: Bearer {token}
{
  "name": "Portefeuille Modifié",
  "description": "Nouvelle description",
  "isActive": true
}

DELETE /api/wallets/{id}
Authorization: Bearer {token}
```

### Transactions

```
GET /api/transactions
Authorization: Bearer {token}

GET /api/transactions/{id}
Authorization: Bearer {token}

GET /api/transactions/wallet/{walletId}
Authorization: Bearer {token}

POST /api/transactions
Authorization: Bearer {token}
{
  "amount": 100.00,
  "type": "Deposit",
  "description": "Dépôt initial",
  "walletId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

## Dépendances Principales

- ASP.NET Core 8.0
- Entity Framework Core 8.0
- ASP.NET Core Identity
- Microsoft.AspNetCore.Authentication.JwtBearer
- Microsoft.EntityFrameworkCore.SqlServer
- Swashbuckle.AspNetCore (Swagger)