SELECT 'Versio', Value,
    CASE WHEN substr(Value,0,5) != '1.0.'
        THEN 'Väärä tietokannan versio.'
    END
FROM kerppi_misc WHERE Key = 'Version';

ALTER TABLE string_constants ADD COLUMN Footer INTEGER NOT NULL DEFAULT 0;
INSERT INTO string_constants (Text, Footer) SELECT Value, 1 FROM kerppi_misc WHERE Key = 'PrintFooter';
DELETE FROM kerppi_misc WHERE Key = 'PrintFooter';

UPDATE kerppi_misc SET Value = '1.1.0.0' WHERE Key = 'Versio';
