namespace WebApi.Tests.Resources
{
    public class UserIdentityManager
    {
        private readonly MyRecipeBook.Domain.Entities.User _user;
        private readonly MyRecipeBook.Domain.Entities.Recipe _recipe;
        private readonly string _password;
        private readonly string _acessToken;

        public UserIdentityManager(MyRecipeBook.Domain.Entities.User user, MyRecipeBook.Domain.Entities.Recipe recipe, string password, string acessToken)
        {
            _user = user;
            _password = password;
            _acessToken = acessToken;
            _recipe = recipe;
        }

        public Guid GetId() => _user.Id;
        public string GetName() => _user.Name;
        public string GetEmail() => _user.Email;
        public string GetPassword() => _password;
        public string GetAcessToken() => _acessToken;
        public MyRecipeBook.Domain.Entities.Recipe GetRecipe() => _recipe;

    }
}
