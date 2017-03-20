SELECT 'Versio', Value,
    CASE WHEN substr(Value,0,5) != '1.0.'
        THEN 'Väärä tietokannan versio.'
    END
FROM kerppi_misc WHERE Key = 'Version';

BEGIN transaction;

ALTER TABLE string_constants ADD COLUMN Footer INTEGER NOT NULL DEFAULT 0;
INSERT INTO string_constants (Text, Footer) SELECT Value, 1 FROM kerppi_misc WHERE Key = 'PrintFooter';
DELETE FROM kerppi_misc WHERE Key = 'PrintFooter';

CREATE TABLE ClientsNEW (
Id INTEGER PRIMARY KEY,
IdCode TEXT UNIQUE NOT NULL,
Active INTEGER NOT NULL DEFAULT 1,
Certificate INTEGER NOT NULL DEFAULT 0,
Name TEXT NOT NULL,
PostalAddress TEXT,
PostalCode TEXT,
ContactInfo TEXT,
Information TEXT);

INSERT INTO ClientsNEW Select Id, IdCode, Active, 0, Name, PostalAddress, PostalCode, ContactInfo, Information FROM Clients;
DROP TABLE Clients;
ALTER TABLE ClientsNEW RENAME TO Clients;

UPDATE kerppi_misc SET Value = '1.1.0.0' WHERE Key = 'Versio';

COMMIT transaction;
