namespace biscuit.Data.Crypt;

public static class CryptFactory
{
    public static ICrypt Create(string key, byte[]? IV)
    {
        return new GK(key, IV);
    }
}