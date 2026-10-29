-- Customer-wise tariff: NULL CustomerId = tariff applies to ALL customers (all existing rows stay NULL)
IF COL_LENGTH('dbo.TariffRate', 'CustomerId') IS NULL
    ALTER TABLE dbo.TariffRate ADD CustomerId INT NULL;
