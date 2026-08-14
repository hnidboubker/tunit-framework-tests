
# TUnit Framework Tests

[![Build & Tests](https://https://github.com/hnidboubker/tunit-framework-tests/actions/workflows/build-tests.yml/badge.svg)](https://[github.com/mon-compte/tunit-framework-tests](https://github.com/hnidboubker/tunit-framework-tests)/actions/workflows/build-tests.yml)

Unit tests for the **IntCore** library, built with [TUnit](https://github.com/thomhurst/TUnit) and [Moq](https://github.com/moq/moq).

## Solution Structure

```
tunit-framework-tests/
├── src/
│   └── IntCore/                    # Main library (net10.0)
│       ├── Services/
│       │   └── UserService.cs
│       ├── DTOs/
│       │   ├── UserDto.cs
│       │   └── CreateUserDto.cs
│       └── Models/
│           ├── Identity/
│           │   └── User.cs
│           └── MultiTenancy/
│               └── Tenant.cs
├── tests/
│   └── IntCore.UnitTests/          # Unit test project (net10.0)
│       └── User/
│           └── UserServiceUnitTests.cs
└── tunit-framework-tests.slnx
```

## Projects

| Project | Target Framework | Purpose |
|---------|-----------------|---------|
| `IntCore` | net10.0 | Core library providing user management with ASP.NET Core Identity and EF Core |
| `IntCore.UnitTests` | net10.0 | Unit tests for `UserService` |

## Dependencies

### IntCore
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` 10.0.11

### IntCore.UnitTests
- `TUnit` 1.65.0 — Test framework
- `Moq` 4.20.72 — Mocking framework
- Project reference: `IntCore`

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
