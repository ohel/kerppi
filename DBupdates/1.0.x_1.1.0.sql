SELECT 'Versio', Value,
    CASE WHEN substr(Value,0,5) != '1.0.'
        THEN 'Väärä tietokannan versio.'
    END
FROM kerppi_misc WHERE Key = 'Version';

BEGIN transaction;

ALTER TABLE string_constants ADD COLUMN Footer INTEGER NOT NULL DEFAULT 0;
INSERT INTO string_constants (Text, Footer) SELECT Value, 1 FROM kerppi_misc WHERE Key = 'PrintFooter';
DELETE FROM kerppi_misc WHERE Key = 'PrintFooter';

CREATE TABLE contacts (
    Id INTEGER PRIMARY KEY,
    Name TEXT NOT NULL,
    PostalAddress TEXT,
    PostalCode TEXT,
    DefaultInfo TEXT,
    AdditionalInfo TEXT,
    Payer INTEGER NOT NULL DEFAULT 0
);
INSERT INTO contacts Select Id, Name, PostalAddress, PostalCode, DefaultContact, NULL, 1 FROM payers;
DROP TABLE payers;

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
    DefaultPayerContactId INTEGER,
    ContactPersonName TEXT,
    ContactPersonPostalAddress TEXT,
    ContactPersonPostalCode TEXT,
    ContactPersonContactInfo TEXT,
    FOREIGN KEY (DefaultPayerContactId) REFERENCES contacts
);
INSERT INTO clientsNEW Select Id, IdCode, Active, 0, Name, PostalAddress, PostalCode, ContactInfo, Information, NULL, NULL, NULL, NULL, NULL FROM clients;
DROP TABLE clients;
ALTER TABLE clientsNEW RENAME TO clients;

UPDATE kerppi_misc SET Value = '1.1.0.0' WHERE Key = 'Version';

COMMIT transaction;
