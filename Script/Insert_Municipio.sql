USE [SADA_BD]
GO

/* Tabela de destino */
--IF OBJECT_ID(N'dbo.TBL_UF', N'U') IS NULL
--BEGIN
--	CREATE TABLE dbo.TBL_CIDADE(
--		ID_CIDADE				int NOT NULL PRIMARY KEY,
--		DESCRICAO_CIDADE		varchar(300) NULL,
--		ID_UF					int NOT NULL,
--		CAPITAL					bit NULL,
--		DDD						int NULL,
--		FUSOHORARIO				varchar(50) NULL,
--		LATITUDE				numeric(10, 0) NULL,
--		LONGITUDE				numeric(10, 0) NULL,
--		SIAFI_ID				int NULL,
--	 CONSTRAINT PK_TBL_CIDADE PRIMARY KEY CLUSTERED 
--	(ID_CIDADE ASC
--	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON PRIMARY
--	) ON PRIMARY
--	GO
	
--    ALTER TABLE dbo.TBL_CIDADE  WITH CHECK ADD  CONSTRAINT FK_tbl_Cidade__Tbl_Uf FOREIGN KEY(ID_UF) REFERENCES dbo.TBL_UF (ID_UF)
--	GO

--	ALTER TABLE dbo.TBL_CIDADE CHECK CONSTRAINT FK_tbl_Cidade__Tbl_Uf GO

--	ALTER TABLE dbo.TBL_CIDADE  WITH CHECK ADD  CONSTRAINT FK_TBL_CIDADE_TBL_UF FOREIGN KEY(ID_UF) 	REFERENCES dbo.TBL_UF (ID_UF)
--	GO

--	ALTER TABLE dbo.TBL_CIDADE CHECK CONSTRAINT FK_TBL_CIDADE_TBL_UF
--END

/* Carrega cada linha do arquivo sem converter colunas */
DROP TABLE IF EXISTS #LinhasCidade;

CREATE TABLE #LinhasCidade
(
    Linha varchar(1000)
);

BULK INSERT #LinhasCidade
FROM 'C:\Teste\Sada\Script\csv\Municipios.csv'
WITH
(
    DATAFILETYPE = 'char',
    CODEPAGE = '1252',
    FIELDTERMINATOR = '0x0B',
    ROWTERMINATOR = '0x0A',
    TABLOCK
);

/* Separa as nove colunas */
DROP TABLE IF EXISTS #CidadeConvertida;

IF OBJECT_ID('tempdb..#CidadeConvertida') IS NOT NULL
    DROP TABLE #CidadeConvertida;

SELECT
    TRY_CONVERT(int,
        MAX(CASE WHEN CONVERT(int, J.[key]) = 0 THEN J.value END)
    ) AS ID_CIDADE,

    CONVERT(varchar(300),
        MAX(CASE WHEN CONVERT(int, J.[key]) = 1 THEN J.value END)
    ) AS DESCRICAO_CIDADE,

    TRY_CONVERT(decimal(6,2),
        MAX(CASE WHEN CONVERT(int, J.[key]) = 2 THEN J.value END)
    ) AS LATITUDE,

    TRY_CONVERT(decimal(6,2),
        MAX(CASE WHEN CONVERT(int, J.[key]) = 3 THEN J.value END)
    ) AS LONGITUDE,

    CONVERT(BIT,
        MAX(CASE WHEN CONVERT(int, J.[key]) = 4 THEN J.value END)
    ) AS CAPITAL,

    CONVERT(INT,
        MAX(CASE WHEN CONVERT(int, J.[key]) = 5 THEN J.value END)
    ) AS ID_UF,

    CONVERT(INT,
        MAX(CASE WHEN CONVERT(int, J.[key]) = 6 THEN J.value END)
    ) AS SIAFI_ID,

    CONVERT(INT,
        MAX(CASE WHEN CONVERT(int, J.[key]) = 7 THEN J.value END)
    ) AS DDD,

    CONVERT(varchar(50),
        MAX(CASE WHEN CONVERT(int, J.[key]) = 8 THEN J.value END)
    ) AS FUSOHORARIO

    --CONVERT(varchar(20),
    --    MAX(CASE WHEN CONVERT(int, J.[key]) = 5
    --             THEN REPLACE(J.value, CHAR(13), '')
    --        END)
    --) AS REGIAO

INTO #CidadeConvertida
FROM #LinhasCidade AS L
CROSS APPLY OPENJSON
(
    '["' +
    REPLACE(
        REPLACE(
            REPLACE(L.Linha, CHAR(13), ''),
            '"',
            '\"'
        ),
        ',',
        '","'
    ) +
    '"]'
) AS J
GROUP BY L.Linha;


/* Mostra possíveis linhas inválidas */
IF EXISTS
(
    SELECT 1
    FROM #CidadeConvertida
    WHERE
          ID_CIDADE IS NULL
       OR DESCRICAO_CIDADE IS NULL
       OR LATITUDE IS NULL
       OR LONGITUDE IS NULL
       OR LONGITUDE IS NULL
       OR CAPITAL IS NULL
       OR ID_UF IS NULL
       OR SIAFI_ID IS NULL
       OR DDD IS NULL
       OR FUSOHORARIO IS NULL
)
BEGIN
    SELECT *
    FROM #CidadeConvertida
    WHERE
        ID_CIDADE IS NULL
       OR DESCRICAO_CIDADE IS NULL
       OR LATITUDE IS NULL
       OR LONGITUDE IS NULL
       OR LONGITUDE IS NULL
       OR CAPITAL IS NULL
       OR ID_UF IS NULL
       OR SIAFI_ID IS NULL
       OR DDD IS NULL
       OR FUSOHORARIO IS NULL;
       
       THROW 50001, 'Existem registros inválidos no Municipios.csv.', 1;
END;


/* Atualiza registros existentes */
UPDATE Destino
SET
    Destino.ID_CIDADE           = Origem.ID_CIDADE,
    Destino.DESCRICAO_CIDADE    = Origem.DESCRICAO_CIDADE,
    Destino.LATITUDE            = Origem.LATITUDE,
    Destino.LONGITUDE           = Origem.LONGITUDE,
    Destino.CAPITAL             = Origem.CAPITAL,
    Destino.ID_UF               = Origem.ID_UF,
    Destino.SIAFI_ID            = Origem.SIAFI_ID,
    Destino.DDD                 = Origem.DDD,
    Destino.FUSOHORARIO         = Origem.FUSOHORARIO
FROM
    dbo.TBL_CIDADE AS Destino INNER JOIN #CidadeConvertida AS Origem ON Origem.ID_CIDADE = Destino.ID_CIDADE;

/* Insere registros novos */
USE SADA_BD
GO

INSERT INTO dbo.TBL_CIDADE (
        ID_CIDADE				,
		DESCRICAO_CIDADE		,
		ID_UF					,
		CAPITAL					,
		DDD						,
		FUSOHORARIO				,
		LATITUDE				,
		LONGITUDE				,
		SIAFI_ID				)

SELECT
    Origem.ID_CIDADE,
    Origem.DESCRICAO_CIDADE,
    Origem.ID_UF,
    Origem.CAPITAL,
    Origem.DDD,
    Origem.FUSOHORARIO,
    Origem.LATITUDE,
    Origem.LONGITUDE,
    Origem.SIAFI_ID
FROM
    #CidadeConvertida AS Origem
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.TBL_CIDADE AS Destino
    WHERE Destino.ID_CIDADE = Origem.ID_CIDADE
);


/* Confere o resultado */
SELECT
    ID_CIDADE,
    DESCRICAO_CIDADE,
    ID_UF,
    CAPITAL,
    DDD,
    FUSOHORARIO,
    LATITUDE,
    LONGITUDE
FROM
    dbo.TBL_CIDADE
ORDER BY
    ID_CIDADE;

