CREATE PROCEDURE [sp_UpdateConsumer]
	@id int,
    @lastName nvarchar(50),
    @firstName nvarchar(50),
    @patronymic nvarchar(50)
AS
	UPDATE [Consumer] 
	SET
	[LastName] = @lastName,
    [FirstName] = @firstName,
	[Patronymic] = @patronymic
	WHERE [Consumer].[ID] = @id;
