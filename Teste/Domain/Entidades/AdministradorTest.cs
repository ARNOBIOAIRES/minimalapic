namespace minimalApi.Domain.Entities;

[TestClass]
public sealed class AdministradorTest
{
    [TestMethod]
    public void TestarGetSetPropriedades()
    {

        // Arrange
        var administrador = new Administrador();
    

        // act
        administrador.Id = 1;
        administrador.Perfil = "Adm";
        administrador.Email = "joao@gmail.com";
        administrador.Senha = "teste123";

        // assert
        Assert.AreEqual(1, administrador.Id);
        Assert.AreEqual("joao@gmail.com", administrador.Email);
        Assert.AreEqual("Adm", administrador.Perfil);
        Assert.AreEqual("teste123", administrador.Senha);
    }
}

