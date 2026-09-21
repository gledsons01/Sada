/* Tabela definitiva */
IF OBJECT_ID(N'dbo.UF', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.UF
    (
        CodigoUF  tinyint       NOT NULL,
        Sigla     char(2)       NOT NULL,
        Nome      varchar(50)   NOT NULL,
        Latitude  decimal(6,2)  NOT NULL,
        Longitude decimal(6,2)  NOT NULL,
        Regiao    varchar(20)   NOT NULL,

        CONSTRAINT PK_UF PRIMARY KEY (CodigoUF),
        CONSTRAINT UQ_UF_Sigla UNIQUE (Sigla)
    );
END;

/* Recebe o CSV sem fazer conversões durante a leitura */
DROP TABLE IF EXISTS #UF_CSV;

CREATE TABLE #UF_CSV
(
    CodigoUF  varchar(20),
    Sigla     varchar(20),
    Nome      varchar(100),
    Latitude  varchar(30),
    Longitude varchar(30),
    Regiao    varchar(50)
);

BULK INSERT #UF_CSV
FROM 'C:\Teste\Sada\Script\csv\UF.csv'
WITH
(
    DATAFILETYPE = 'char',
    CODEPAGE = '1252',
    FIELDTERMINATOR = ',',
    ROWTERMINATOR = '0x0A',
    FIRSTROW = 1,
    TABLOCK
);

/* Remove espaços e o caractere CR presente em arquivos Windows */
UPDATE #UF_CSV
SET
    CodigoUF  = NULLIF(LTRIM(RTRIM(REPLACE(CodigoUF,  CHAR(13), ''))), ''),
    Sigla     = NULLIF(LTRIM(RTRIM(REPLACE(Sigla,     CHAR(13), ''))), ''),
    Nome      = NULLIF(LTRIM(RTRIM(REPLACE(Nome,      CHAR(13), ''))), ''),
    Latitude  = NULLIF(LTRIM(RTRIM(REPLACE(Latitude,  CHAR(13), ''))), ''),
    Longitude = NULLIF(LTRIM(RTRIM(REPLACE(Longitude, CHAR(13), ''))), ''),
    Regiao    = NULLIF(LTRIM(RTRIM(REPLACE(Regiao,    CHAR(13), ''))), '');

/* Interrompe a carga e mostra linhas inválidas */
IF EXISTS
(
    SELECT 1
    FROM #UF_CSV
    WHERE TRY_CONVERT(tinyint, CodigoUF) IS NULL
       OR LEN(Sigla) <> 2
       OR Nome IS NULL
       OR TRY_CONVERT(decimal(6,2), Latitude) IS NULL
       OR TRY_CONVERT(decimal(6,2), Longitude) IS NULL
       OR Regiao IS NULL
)
BEGIN
    SELECT *
    FROM #UF_CSV
    WHERE TRY_CONVERT(tinyint, CodigoUF) IS NULL
       OR LEN(Sigla) <> 2
       OR Nome IS NULL
       OR TRY_CONVERT(decimal(6,2), Latitude) IS NULL
       OR TRY_CONVERT(decimal(6,2), Longitude) IS NULL
       OR Regiao IS NULL;

    THROW 50001, 'O arquivo UF.csv contém dados inválidos.', 1;
END;

/* Atualiza registros existentes e inclui os novos */
MERGE dbo.UF AS Destino
USING
(
    SELECT
        CONVERT(tinyint, CodigoUF)         AS CodigoUF,
        CONVERT(char(2), Sigla)            AS Sigla,
        CONVERT(varchar(50), Nome)         AS Nome,
        CONVERT(decimal(6,2), Latitude)    AS Latitude,
        CONVERT(decimal(6,2), Longitude)   AS Longitude,
        CONVERT(varchar(20), Regiao)       AS Regiao
    FROM #UF_CSV
) AS Origem
ON Destino.CodigoUF = Origem.CodigoUF

WHEN MATCHED THEN
    UPDATE SET
        Destino.Sigla     = Origem.Sigla,
        Destino.Nome      = Origem.Nome,
        Destino.Latitude  = Origem.Latitude,
        Destino.Longitude = Origem.Longitude,
        Destino.Regiao    = Origem.Regiao

WHEN NOT MATCHED THEN
    INSERT (CodigoUF, Sigla, Nome, Latitude, Longitude, Regiao)
    VALUES
    (
        Origem.CodigoUF,
        Origem.Sigla,
        Origem.Nome,
        Origem.Latitude,
        Origem.Longitude,
        Origem.Regiao
    );

SELECT *
FROM dbo.UF
ORDER BY CodigoUF;

INSERT INTO dbo.TBL_UF  (SELECT * FROM dbo.UF ORDER BY CodigoUF;)