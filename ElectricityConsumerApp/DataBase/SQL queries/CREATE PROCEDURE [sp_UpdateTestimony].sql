CREATE PROCEDURE [sp_UpdateTestimony]
	@id int,
    @value float
AS
	UPDATE [TestimonyHistory] 
	SET
	[Value] = @value
	WHERE [TestimonyHistory].[ID] = @id;
