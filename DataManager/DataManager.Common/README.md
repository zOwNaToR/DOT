## Visual Studio Package manager console
### *Create Migration*
```add-migration <Name> -Project DataManager.SqlServer```

### *Update database*
```update-database````

## Dotnet cli

### *Create Migration*
```dotnet ef migrations add <Name> --project DataManager.Common --startup-project WebApi```

### *Update database*
```dotnet ef database update --project DataManager.Common --startup-project WebApi````