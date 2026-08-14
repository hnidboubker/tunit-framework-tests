# TUnit Framework Tests

[![Build & Tests](https://github.com/hnidboubker/tunit-framework-tests/actions/workflows/build-tests.yml/badge.svg)](https://github.com/hnidboubker/tunit-framework-tests)

Unit tests for the **IntCore** and **IntApplication** libraries, built with [![NuGet Version](https://img.shields.io/nuget/v/TUnit.svg)](https://www.nuget.org/packages/TUnit) **TUnit** and [![NuGet Version](https://img.shields.io/nuget/v/Moq.svg)](https://www.nuget.org/packages/Moq) **Moq**.

## Solution Overview

| Project | Target Framework | NuGet Packages |
|---------|-----------------|----------------|
| `IntCore` | net10.0 | `Microsoft.AspNetCore.Identity.EntityFrameworkCore` 10.0.11 |
| `IntEntityFrameworkCore` | net10.0 | — |
| `IntApplication` | net10.0 | — |
| `IntCore.UnitTests` | net10.0 | `TUnit` 1.65.0 · `Moq` 4.20.72 |
| `IntApplication.UnitTests` | net10.0 | `TUnit` 1.65.0 · `Moq` 4.20.72 |

## Solution Structure

```
tunit-framework-tests/
├── src/
│   ├── IntCore/                        # Core library (net10.0)
│   │   └── Models/
│   │       ├── Identity/
│   │       │   ├── User.cs
│   │       │   └── Role.cs
│   │       └── MultiTenancy/
│   │           └── Tenant.cs
│   ├── IntEntityFrameworkCore/         # EF Core persistence (net10.0)
│   │   └── Persistence/
│   │       └── DefaultContext.cs
│   └── IntApplication/                 # Application layer (net10.0)
│       ├── DTOs/
│       │   ├── UserDto.cs
│       │   ├── CreateUserDto.cs
│       │   └── EditUserDto.cs
│       └── Services/
│           └── UserService.cs
├── tests/
│   ├── IntCore.UnitTests/              # Unit tests for IntCore (net10.0)
│   │   ├── User/
│   │   │   └── UserUnitTests.cs
│   │   ├── Role/
│   │   │   └── RoleUnitTests.cs
│   │   └── Tenant/
│   │       └── TenantTests.cs
│   └── IntApplication.UnitTests/       # Unit tests for IntApplication (net10.0)
│       ├── Services/
│       │   └── UserServiceTests.cs
│       ├── Managers/
│       │   └── MockManager.cs
│       ├── Builders/
│       │   └── UserServiceBuilder.cs
│       ├── Collections/
│       │   ├── TestAsyncEnumerable.cs
│       │   ├── TestAsyncEnumerator.cs
│       │   └── TestAsyncQueryProvider.cs
│       └── Helpers/
│           └── QuerableHelper.cs
└── tunit-framework-tests.slnx
```

## Projects

| Project | Target Framework | Purpose |
|---------|-----------------|---------|
| `IntCore` | net10.0 | Core library providing domain models (`User`, `Role`, `Tenant`) |
| `IntEntityFrameworkCore` | net10.0 | EF Core persistence layer, provides `DefaultContext` (IdentityDbContext) |
| `IntApplication` | net10.0 | Application layer with DTOs and business services (`UserService`) |
| `IntCore.UnitTests` | net10.0 | Unit tests for `IntCore` |
| `IntApplication.UnitTests` | net10.0 | Unit tests for `IntApplication.UserService` |

## Dependencies

| Project | NuGet Packages |
|---------|---------------|
| `IntCore` | `Microsoft.AspNetCore.Identity.EntityFrameworkCore` 10.0.11 |
| `IntEntityFrameworkCore` | — |
| `IntApplication` | — |
| `IntCore.UnitTests` | `TUnit` 1.65.0 · `Moq` 4.20.72 |
| `IntApplication.UnitTests` | `TUnit` 1.65.0 · `Moq` 4.20.72 |

## Building

```bash
dotnet build
```

## Running Tests

```bash
dotnet test
```

## License

MIT License