/* Tabela de destino */
IF OBJECT_ID(N'dbo.TBL_UF', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TBL_UF
    (
        ID_UF           tinyint       NOT NULL PRIMARY KEY,
        SIGLA_UF        char(2)       NOT NULL,
        DESCRICAO_UF    varchar(50)   NOT NULL,
        LATITUDE        decimal(6,2)  NOT NULL,
        LONGITUDE       decimal(6,2)  NOT NULL,
        REGIAO          varchar(20)   NOT NULL
    );
END;


/* Carrega cada linha do arquivo sem converter colunas */
DROP TABLE IF EXISTS #LinhasUF;

CREATE TABLE #LinhasUF
(
    Linha varchar(1000)
);

BULK INSERT #LinhasUF
FROM 'C:\Teste\Sada\Script\csv\UF.csv'
WITH
(
    DATAFILETYPE = 'char',
    CODEPAGE = '1252',
    FIELDTERMINATOR = '0x0B',
    ROWTERMINATOR = '0x0A',
    TABLOCK
);


/* Separa as seis colunas */
DROP TABLE IF EXISTS #UFConvertida;

SELECT
    TRY_CONVERT(tinyint,
        MAX(CASE WHEN CONVERT(int, J.[key]) = 0 THEN J.value END)
    ) AS ID_UF,

    CONVERT(char(2),
        MAX(CASE WHEN CONVERT(int, J.[key]) = 1 THEN J.value END)
    ) AS SIGLA_UF,

    CONVERT(varchar(50),
        MAX(CASE WHEN CONVERT(int, J.[key]) = 2 THEN J.value END)
    ) AS DESCRICAO_UF,

    TRY_CONVERT(decimal(6,2),
        MAX(CASE WHEN CONVERT(int, J.[key]) = 3 THEN J.value END)
    ) AS LATITUDE,

    TRY_CONVERT(decimal(6,2),
        MAX(CASE WHEN CONVERT(int, J.[key]) = 4 THEN J.value END)
    ) AS LONGITUDE,

    CONVERT(varchar(20),
        MAX(CASE WHEN CONVERT(int, J.[key]) = 5
                 THEN REPLACE(J.value, CHAR(13), '')
            END)
    ) AS REGIAO
INTO #UFConvertida
FROM #LinhasUF AS L
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
    FROM #UFConvertida
    WHERE ID_UF IS NULL
       OR SIGLA_UF IS NULL
       OR DESCRICAO_UF IS NULL
       OR LATITUDE IS NULL
       OR LONGITUDE IS NULL
       OR REGIAO IS NULL
)
BEGIN
    SELECT *
    FROM #UFConvertida
    WHERE ID_UF IS NULL
       OR SIGLA_UF IS NULL
       OR DESCRICAO_UF IS NULL
       OR LATITUDE IS NULL
       OR LONGITUDE IS NULL
       OR REGIAO IS NULL;

    THROW 50001, 'Existem registros inválidos no UF.csv.', 1;
END;


/* Atualiza registros existentes */
UPDATE Destino
SET
    Destino.SIGLA_UF            = Origem.SIGLA_UF,
    Destino.DESCRICAO_UF        = Origem.DESCRICAO_UF,
    Destino.LATITUDE            = Origem.LATITUDE,
    Destino.LONGITUDE           = Origem.LONGITUDE,
    Destino.REGIAO              = Origem.REGIAO
FROM dbo.TBL_UF AS Destino
INNER JOIN #UFConvertida AS Origem
    ON Origem.ID_UF = Destino.ID_UF;


/* Insere registros novos */
USE [SADA_BD]
GO

INSERT INTO dbo.TBL_UF
           (ID_UF
           ,SIGLA_UF
           ,DESCRICAO_UF
           ,LATITUDE
           ,LONGITUDE
           ,REGIAO)



SELECT
    Origem.ID_UF,
    Origem.SIGLA_UF,
    Origem.DESCRICAO_UF,
    Origem.LATITUDE,
    Origem.LONGITUDE,
    Origem.REGIAO
FROM #UFConvertida AS Origem
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.TBL_UF AS Destino
    WHERE Destino.ID_UF = Origem.ID_UF
);


/* Confere o resultado */
SELECT
    ID_UF,
    SIGLA_UF,
    DESCRICAO_UF,
    LATITUDE,
    LONGITUDE,
    REGIAO
FROM dbo.TBL_UF
ORDER BY ID_UF;