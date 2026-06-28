namespace Berger.Extensions.Repository;

public static class Values
{
    public const string Deleted = "Deleted";
    public const string IsDeleted = "IsDeleted";
}

public static class Errors
{
    public const string ConfigNotFound = "The application configuration file for the connection was not found.";
    public const string EntityNotFound = "The requested entity was not found.";
}

public static class Patterns
{
    public const string AzureSqlServer = "ConnectionStrings:AzureSqlServer";
}
