SELECT 'Versio', Value,
    CASE WHEN substr(Value,0,5) != '1.1.'
        THEN 'Väärä tietokannan versio.'
    END
FROM kerppi_misc WHERE Key = 'Version';

BEGIN transaction;

ALTER TABLE clients ADD COLUMN ConsentContactInfo INTEGER DEFAULT NULL;
ALTER TABLE clients ADD COLUMN ConsentIdInfo INTEGER DEFAULT NULL;
ALTER TABLE clients ADD COLUMN ConsentContactPerson INTEGER DEFAULT NULL;
ALTER TABLE clients ADD COLUMN Restricted INTEGER NOT NULL DEFAULT 0;

UPDATE kerppi_misc SET Value = '1.2.0.0' WHERE Key = 'Version';

COMMIT transaction;
