# Workleap.Extensions.Configuration.Substitution

Workleap.Extensions.Configuration.Substitution is a .NET library that provides variable substitution for Microsoft.Extensions.Configuration. It allows referencing configuration values within other configuration values using the syntax `${Key}` or `${Section:Key}`.

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Working Effectively

### Prerequisites and Setup
- **CRITICAL**: Install .NET 9.0.304 SDK (specified in global.json): `curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --version 9.0.304`
- Add to PATH: `export PATH="$HOME/.dotnet:$PATH"`
- Navigate to repository: `cd /home/runner/work/wl-extensions-configuration-substitution/wl-extensions-configuration-substitution`

### Bootstrap, Build, and Test Commands
**CRITICAL TIMING**: All operations are fast in this repository.
- Navigate to src directory: `cd src`
- Clean: `dotnet clean`
- Debug build: `dotnet build --configuration Debug`
- **Release build limitation**: `dotnet build --configuration Release` fails due to GitVersion requirements in sandbox environment. Use Debug builds for development.
- Test: `dotnet test --configuration Debug --no-build --logger "console;verbosity=detailed"`

### Validation and Quality Checks
- **ALWAYS format code**: `dotnet format`
- **ALWAYS verify formatting**: `dotnet format --verify-no-changes`
- **ALWAYS build and test** after making any code changes to ensure functionality is preserved.

### Manual Functionality Testing
After making changes to the library code, ALWAYS manually validate functionality:

**Option 1: Create test console app in /tmp**
```bash
mkdir -p /tmp/test-app
cd /tmp/test-app
cat > Program.cs << 'EOF'
using Microsoft.Extensions.Configuration;
using Workleap.Extensions.Configuration.Substitution;

var builder = new ConfigurationBuilder()
    .AddInMemoryCollection(new Dictionary<string, string?> 
    {
        {"Database:Username", "alice"},
        {"Database:Password", "secret123"},
        {"ConnectionString", "User=${Database:Username};Password=${Database:Password};Server=localhost"}
    })
    .AddSubstitution();

var config = builder.Build();
Console.WriteLine($"ConnectionString = {config["ConnectionString"]}");
EOF

cat > TestApp.csproj << 'EOF'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="/home/runner/work/wl-extensions-configuration-substitution/wl-extensions-configuration-substitution/src/Workleap.Extensions.Configuration.Substitution/Workleap.Extensions.Configuration.Substitution.csproj" />
  </ItemGroup>
</Project>
EOF

dotnet run
```
Expected output: `ConnectionString = User=alice;Password=secret123;Server=localhost`

**Option 2: Run integration test web application**
```bash
cd src/WebAPI.Substitution.IntegrationTests
dotnet run
# App starts on http://localhost:5076 (404 response is expected - no endpoints defined)
# Press Ctrl+C to stop
```

## Project Structure and Key Files

### Repository Layout
```
/
├── README.md               # Package documentation
├── Build.ps1              # PowerShell build script (has GitVersion limitations in sandbox)
├── global.json            # .NET SDK version specification (9.0.304)
├── src/
│   ├── Workleap.Extensions.Configuration.Substitution/           # Main library
│   │   ├── ConfigurationSubstitutorBuilderExtensions.cs         # Main API entry point
│   │   ├── ConfigurationSubstitutor.cs                          # Core substitution logic
│   │   └── *.cs                                                 # Supporting classes
│   ├── Workleap.Extensions.Configuration.Substitution.Tests/    # Unit tests (20 tests)
│   └── WebAPI.Substitution.IntegrationTests/                   # Integration tests (2 tests)
```

### Key Code Files
- **ConfigurationSubstitutorBuilderExtensions.cs**: Main public API with `AddSubstitution()` extension method
- **ConfigurationSubstitutorTests.cs**: Comprehensive unit tests covering all substitution scenarios
- **Program.cs** (integration tests): Minimal web app for testing configuration scenarios

## Development Guidelines

### Code Standards
- Uses **Workleap.DotNet.CodingStandards** package for consistent formatting
- **ALWAYS run `dotnet format`** before committing
- Target frameworks: net8.0 and netstandard2.0 for main library
- Tests use xUnit framework
- Integration tests use ASP.NET Core testing framework

### Testing Strategy
- **Unit tests**: Cover all substitution scenarios (circular references, escaping, missing keys, etc.)
- **Integration tests**: Validate configuration reloading and multiple AddSubstitution calls
- **Manual testing**: Always test actual substitution functionality with real scenarios

### Common Development Tasks
- **Adding new substitution features**: Modify `ConfigurationSubstitutor.cs` and add corresponding tests
- **API changes**: Update `ConfigurationSubstitutorBuilderExtensions.cs` and `PublicAPI.Unshipped.txt`
- **Bug fixes**: Add failing test first, then implement fix
- **Always check handler.ts after making changes to apiContracts.ts** - N/A (this is a .NET project, not TypeScript)

## Build Limitations and Workarounds

### Known Issues
- **GitVersion fails in sandbox environment**: Release builds fail due to git repository requirements
- **Workaround**: Use Debug builds for all development work
- **NuGet packaging**: Use `dotnet pack --configuration Debug` for testing package creation

### CI/CD Information
- GitHub Actions workflows in `.github/workflows/`
- Main CI: Uses PowerShell `Build.ps1` script
- Runs on custom runners with `[idp]` tag
- Publishes to NuGet on releases

## Functionality Overview

The library provides configuration value substitution using `${Key}` syntax:

### Basic Usage
```csharp
builder.Configuration.AddSubstitution(); // Add after other providers
```

### Substitution Examples
- `${Username}` - Simple key reference
- `${Database:ConnectionString}` - Section:Key reference  
- `${Array:0}` - Array index reference
- `${{NotSubstituted}}` - Escaped (outputs `${NotSubstituted}`)

### Exception Handling
- `UnresolvedConfigurationKeyException`: Referenced key doesn't exist
- `RecursiveConfigurationKeyException`: Circular reference detected

### Validation Options
- `AddSubstitution(eagerValidation: true)` - Validates all keys immediately
- `AddSubstitution(eagerValidation: false)` - Validates on-demand (default)

Always test configuration substitution scenarios manually after making changes to ensure the library works correctly in real-world usage.