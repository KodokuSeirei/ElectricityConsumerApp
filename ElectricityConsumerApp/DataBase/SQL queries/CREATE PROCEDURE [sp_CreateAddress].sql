CREATE PROCEDURE [sp_CreateAddress]
    @id int out,
    @cityID int,
    @street nvarchar(100),
    @home nvarchar(40),
    @fiat nvarchar(40)
AS
    INSERT INTO [AddressesDirectory]([CityID], [Street], [Home], [Fiat])
    VALUES (@cityID, @street, @home, @fiat)

	SELECT SCOPE_IDENTITY()
