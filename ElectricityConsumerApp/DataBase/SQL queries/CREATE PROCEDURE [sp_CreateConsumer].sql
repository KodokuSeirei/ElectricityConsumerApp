CREATE PROCEDURE [sp_CreateConsumer]
    @id int out,
    @lastName nvarchar(50),
    @firstName nvarchar(50),
    @patronymic nvarchar(50)
AS
    INSERT INTO [Consumer]([LastName], [FirstName], [Patronymic])
    VALUES (@lastName, @firstName, @patronymic)

	SELECT SCOPE_IDENTITY()