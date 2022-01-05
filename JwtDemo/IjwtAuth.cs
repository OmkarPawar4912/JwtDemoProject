namespace JwtDemo
{
    public interface IjwtAuth
    {
       string Authentication(string username, string password);
    }
}