CREATE TRIGGER city_delete
ON [fs_city]
INSTEAD OF DELETE
AS
UPDATE [AddressesDirectory]
SET [IsDeleted] = 1
WHERE [CityID] =(SELECT id FROM deleted)
DELETE FROM [fs_city] 
WHERE [id] =(SELECT id FROM deleted)