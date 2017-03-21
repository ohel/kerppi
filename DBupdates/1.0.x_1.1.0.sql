SELECT 'Versio', Value,
    CASE WHEN substr(Value,0,5) != '1.0.'
        THEN 'Väärä tietokannan versio.'
    END
FROM kerppi_misc WHERE Key = 'Version';

BEGIN transaction;

ALTER TABLE string_constants ADD COLUMN Footer INTEGER NOT NULL DEFAULT 0;
INSERT INTO string_constants (Text, Footer) SELECT Value, 1 FROM kerppi_misc WHERE Key = 'PrintFooter';
DELETE FROM kerppi_misc WHERE Key = 'PrintFooter';

CREATE TABLE clientsNEW (
Id INTEGER PRIMARY KEY,
IdCode TEXT UNIQUE NOT NULL,
Active INTEGER NOT NULL DEFAULT 1,
Certificate INTEGER NOT NULL DEFAULT 0,
Name TEXT NOT NULL,
PostalAddress TEXT,
PostalCode TEXT,
ContactInfo TEXT,
Information TEXT,
DefaultPayerId INTEGER,
FOREIGN KEY (DefaultPayerId) REFERENCES payers);

INSERT INTO clientsNEW Select Id, IdCode, Active, 0, Name, PostalAddress, PostalCode, ContactInfo, Information, NULL FROM clients;
DROP TABLE clients;
ALTER TABLE clientsNEW RENAME TO clients;

ALTER TABLE payers ADD COLUMN AdditionalInfo TEXT;

UPDATE kerppi_misc SET Value = '1.1.0.0' WHERE Key = 'Versio';

COMMIT transaction;
