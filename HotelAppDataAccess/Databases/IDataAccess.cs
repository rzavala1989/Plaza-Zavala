namespace HotelAppLibrary.Databases;

public interface IDataAccess
{
    List<T> LoadData<T, U>(string sql,
        U parameters,
        string connectionStringName,
        dynamic options = null);

    void SaveData<T>(string sql,
        T parameters,
        string connectionStringName,
        dynamic options = null);
}