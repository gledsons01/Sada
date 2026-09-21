IF OBJECT_ID(N'dbo.TBL_USUARIO', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TBL_USUARIO
    (
        IdUsuario INT NOT NULL PRIMARY KEY,
        NomeUsuario NVARCHAR(200) NOT NULL,
        Login NVARCHAR(20) NOT NULL,
        Senha NVARCHAR(10) NOT NULL,
        Endereco NVARCHAR(500) NOT NULL,
        NumeroEndereco NVARCHAR(20) NOT NULL,
        Bairro NVARCHAR(100) NOT NULL,
        IdUf INT NOT NULL,
        IdCidade INT NOT NULL,
        NomeSocial NVARCHAR(500) NOT NULL,
        IdSexo INT NOT NULL,
        EMail NVARCHAR(500) NOT NULL
    );
END

