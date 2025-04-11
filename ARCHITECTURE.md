# Architecture et Choix Techniques - Wallet Management API

## Vue d'Ensemble

Le projet "Wallet Management API" a été conçu en suivant les principes de l'architecture propre (Clean Architecture) pour garantir une application modulaire, testable et maintenable. Cette approche favorise la séparation des préoccupations et l'inversion des dépendances, permettant une évolution plus facile du système au fil du temps.

## Architecture Clean

Notre implémentation de l'architecture propre est structurée en plusieurs couches concentriques :

![Clean Architecture Diagram](https://blog.cleancoder.com/uncle-bob/images/2012-08-13-the-clean-architecture/CleanArchitecture.jpg)

### 1. Couche de Domaine (Core)

Au centre de notre architecture se trouve la couche de domaine, qui contient :
- Les entités métier fondamentales (User, Wallet, Transaction)
- Les interfaces des repositories
- La logique de domaine essentielle
- Aucune dépendance vers des frameworks ou bibliothèques externes

Cette couche est totalement indépendante des préoccupations techniques comme la persistance des données, l'interface utilisateur ou les frameworks externes.

### 2. Couche d'Application

Cette couche encapsule la logique d'application et les cas d'utilisation :
- Implémente les services qui orchestrent les opérations sur les entités
- Définit les DTOs (Data Transfer Objects) pour transformer les données entre les couches
- Contient les interfaces des services d'infrastructure
- Dépend uniquement de la couche de domaine

### 3. Couche d'Infrastructure

Cette couche fournit les implémentations concrètes des interfaces définies dans les couches intérieures :
- Persistance des données avec Entity Framework Core
- Implémentation des repositories
- Services d'infrastructure (authentification, journalisation, etc.)
- Communication avec des services externes

### 4. Couche de Présentation / API

La couche la plus externe qui interagit directement avec les utilisateurs :
- API REST avec contrôleurs ASP.NET Core
- Middlewares de sécurité et d'authentification
- Gestion des erreurs et validation des entrées
- Documentation de l'API avec Swagger

## Choix Techniques

### Framework et Environnement

- **.NET 8** : Dernière version du framework .NET offrant des performances optimales et des fonctionnalités modernes
- **ASP.NET Core** : Framework web performant, multiplateforme et open-source
- **Entity Framework Core** : ORM moderne pour l'accès aux données

### Authentification et Sécurité

- **ASP.NET Core Identity** : Framework d'identité complet pour la gestion des utilisateurs
- **JWT (JSON Web Tokens)** : Pour l'authentification sans état (stateless)
- **HTTPS** : Communication sécurisée entre client et serveur
- **Role-Based Authorization** : Contrôle d'accès basé sur les rôles (Admin, User)

### Persistance des Données

- **SQL Server** : SGBD relationnel robuste pour le stockage des données
- **Code First** : Approche de développement où les entités du domaine définissent le schéma de base de données
- **Migrations** : Gestion évolutive du schéma de base de données

### Interface Utilisateur

- **Blazor WebAssembly** : Framework côté client permettant d'écrire des applications web interactives en C#
- **Communication API RESTful** : Interaction entre le client Blazor et le serveur API

### Patterns de Conception

- **Repository Pattern** : Abstraction de la couche d'accès aux données
- **Dependency Injection** : Inversion des dépendances pour un couplage faible


## Avantages de l'Architecture Choisie

1. **Maintenabilité** : Les changements dans une couche n'affectent pas nécessairement les autres couches
2. **Testabilité** : Les composants peuvent être testés individuellement grâce au couplage faible
3. **Évolutivité** : Facilité d'ajout de nouvelles fonctionnalités ou de modification des fonctionnalités existantes
4. **Indépendance technologique** : Possibilité de changer les technologies externes (base de données, UI) sans affecter la logique métier
5. **Clarté** : Structure claire et organisation logique du code

## Défis et Considérations

1. **Complexité initiale** : L'architecture clean introduit une complexité initiale plus élevée
2. **Surcharge de code** : Plus de classes et d'interfaces par rapport à une architecture simple
3. **Courbe d'apprentissage** : Les nouveaux développeurs peuvent avoir besoin de temps pour comprendre la structure

## Améliorations Futures

1. **Microservices** : Évolution potentielle vers une architecture de microservices pour certains composants
2. **API GraphQL** : Alternative/complément à l'API REST pour des requêtes plus flexibles
3. **Event Sourcing** : Pour un historique complet des modifications d'état
4. **Internationalisation** : Support multilingue pour l'interface utilisateur
5. **PWA (Progressive Web App)** : Transformation de l'application Blazor en PWA pour une expérience mobile améliorée

## Diagramme de l'Architecture

```
┌─────────────────────────┐
│  WalletManagement.Blazor │
│   (Interface Utilisateur) │
└───────────┬─────────────┘
            │ HTTP/REST
┌───────────▼─────────────┐
│    WalletManagement.API  │
│  (Contrôleurs, Middleware)│
└───────────┬─────────────┘
            │
┌───────────▼─────────────┐
│ WalletManagement.Application │
│    (Services, DTOs)      │
└───────────┬─────────────┘
            │
┌───────────▼─────────────┐
│  WalletManagement.Domain │
│   (Entités, Interfaces)  │
└───────────┬─────────────┘
            │
┌───────────▼─────────────┐
│WalletManagement.Infrastructure│
│  (Repositories, EF Core)  │
└───────────┬─────────────┘
            │
┌───────────▼─────────────┐
│       SQL Server        │
│    (Base de Données)     │
└─────────────────────────┘
```

## Conclusion

L'architecture propre adoptée pour ce projet offre un équilibre entre la flexibilité, la maintenabilité et les performances. Bien que plus complexe qu'une architecture monolithique simple, elle présente des avantages considérables pour le développement à long terme et l'évolution du système.