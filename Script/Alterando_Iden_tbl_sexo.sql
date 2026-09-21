SELECT TOP (1000) [ID_SEXO]
      ,[DESCRICAO]
      ,[SIGLA]
  FROM [SADA_BD].[dbo].[TBL_SEXO]


  -- 1. Adicionar uma nova coluna temporária sem o IDENTITY
ALTER TABLE dbo.TBL_SEXO ADD ID_SEXO_Temp INT NULL;
GO

-- 2. Copiar os dados da coluna antiga para a nova
UPDATE dbo.TBL_SEXO SET ID_SEXO_Temp = ID_SEXO;
GO

-- 3. Remover a restrição de Chave Primária (Primary Key) antiga
-- (O nome da constraint varia; descubra o nome correto se necessário)
ALTER TABLE dbo.TBL_SEXO DROP CONSTRAINT PK_TBL_CIDADE.TBL_SEXO; -- Substitua pelo nome real da sua PK
GO

-- 4. Excluir a coluna antiga com IDENTITY
ALTER TABLE dbo.TBL_SEXO DROP COLUMN ID_SEXO;
GO

-- 5. Renomear a nova coluna para o nome original
EXEC sp_rename 'dbo.TBL_SEXO.ID_SEXO_Temp', 'ID_SEXO', 'COLUMN';
GO

-- 6. Tornar a coluna NOT NULL e recriar a Chave Primária
ALTER TABLE dbo.TBL_SEXO ALTER COLUMN ID_SEXO INT NOT NULL;
GO

ALTER TABLE dbo.TBL_SEXO
ADD CONSTRAINT PK_TBL_SEXO
PRIMARY KEY (ID_SEXO);
GO
