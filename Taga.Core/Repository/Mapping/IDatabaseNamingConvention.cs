namespace Taga.Core.Repository.Mapping
{
    public interface IDatabaseNamingConvention
    {
        string GetTableName(string className);

        string GetColumnName(string propertyName);
    }
}