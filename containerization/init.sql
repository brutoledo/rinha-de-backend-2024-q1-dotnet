-- noinspection SqlNoDataSourceInspectionForFile
-- noinspection SqlDialectInspectionForFile

CREATE UNLOGGED TABLE CLIENTS
(
    ID           SMALLSERIAL PRIMARY KEY,
    NAME         VARCHAR(50) NOT NULL,
    CREDIT_LIMIT INT         NOT NULL,
    BALANCE      INT         NOT NULL DEFAULT (0)
);

CREATE UNLOGGED TABLE CLIENT_TRANSACTIONS
(
    ID           SERIAL PRIMARY KEY,
    CLIENT_ID    INT         NOT NULL,
    VALUE        INT         NOT NULL,
    TYPE         CHAR(1),
    DESCRIPTION  VARCHAR(50) NOT NULL,
    CREATED_DATE timestamp(0) NOT NULL DEFAULT timezone('utc', now()),
    CONSTRAINT FK_clients FOREIGN KEY (CLIENT_ID) REFERENCES CLIENTS (ID)
);


DO
$$
    BEGIN
        INSERT INTO CLIENTS (NAME, CREDIT_LIMIT)
        VALUES ('o barato sai caro', 1000 * 100),
               ('zan corp ltda', 800 * 100),
               ('les cruders', 10000 * 100),
               ('padaria joia de cocaia', 100000 * 100),
               ('kid mais', 5000 * 100);
    END;
$$;