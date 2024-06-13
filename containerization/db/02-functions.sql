CREATE OR REPLACE FUNCTION createTransaction(
    IN clientId integer,
    IN transactionValue integer,
    IN transactionType varchar(10),
    IN description varchar(10)
) RETURNS RECORD AS $$

DECLARE
    foundClient clients%rowtype;
    ret RECORD;
BEGIN

    --raise notice 'ClientId: %', clientId;
    --raise notice 'TransactionValue: %', transactionValue;
    --raise notice 'TransactionType: %', transactionType;
    --raise notice 'Description: %', description;
    
    SELECT * FROM clients
        WHERE id = clientId
    INTO foundClient;    

    IF not found THEN
        --raise notice 'Client not found.';
        SELECT -1 INTO ret;
        RETURN ret;
    END IF;

    UPDATE CLIENTS 
        SET BALANCE = BALANCE + transactionValue
        WHERE ID = clientId 
          AND (transactionValue > 0 OR BALANCE + transactionValue >= CREDIT_LIMIT * -1)
        RETURNING BALANCE, CREDIT_LIMIT INTO ret;   

    raise notice 'Ret: %', ret;
    
    IF ret.CREDIT_LIMIT is NULL THEN
        SELECT -2 INTO ret; -- no balance available
        RETURN ret;
    END IF;

    INSERT INTO CLIENT_TRANSACTIONS (
        CLIENT_ID, VALUE, TYPE, DESCRIPTION)
    VALUES (clientId, transactionValue, transactionType, description);
    
    return ret;
    
END;$$ LANGUAGE plpgsql;