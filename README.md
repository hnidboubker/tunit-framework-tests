# TUnit Framework Tests

[![Build & Tests](https://github.com/hnidboubker/tunit-framework-tests/actions/workflows/build-tests.yml/badge.svg)](https://github.com/hnidboubker/tunit-framework-tests)

Unit tests for the **IntCore** library, built with [TUnit](https://github.com/thomhurst/TUnit) and [Moq](https://github.com/moq/moq).

## Solution Structure

```
tunit-framework-tests/
├── src/
│   ├── IntCore/                        # Core library (net10.0)
│   │   ├── DTOs/
│   │   │   ├── UserDto.cs
│   │   │   └── CreateUserDto.cs
│   │   ├── Models/
│   │   │   ├── Identity/
│   │   │   │   ├── User.cs
│   │   │   │   └── Role.cs
│   │   │   └── MultiTenancy/
│   │   │       └── Tenant.cs
│   │   └── Services/
│   │       └── UserService.cs
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
│   │   └── User/
│   │       └── UserUnitTests.cs
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
| `IntCore` | net10.0 | Core library providing domain models, DTOs, and user management primitives with ASP.NET Core Identity |
| `IntEntityFrameworkCore` | net10.0 | EF Core persistence layer, provides `DefaultContext` (IdentityDbContext) |
| `IntApplication` | net10.0 | Application layer with business services (`UserService`) orchestrating Identity operations |
| `IntCore.UnitTests` | net10.0 | Unit tests for `IntCore` |
| `IntApplication.UnitTests` | net10.0 | Unit tests for `IntApplication.UserService` |

## Dependencies

### IntCore
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` 10.0.11

### IntEntityFrameworkCore
- Project reference: `IntCore`

### IntApplication
- Project reference: `IntEntityFrameworkCore`

### IntCore.UnitTests
- `TUnit` 1.65.0 — Test framework
- `Moq` 4.20.72 — Mocking framework
- Project reference: `IntCore`

### IntApplication.UnitTests
- `TUnit` 1.65.0 — Test framework
- `Moq` 4.20.72 — Mocking framework
- Project reference: `IntApplication`

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
