DECLARE @number INT = 10

WHILE @number < 100
    BEGIN
	DECLARE @string nvarchar(300) = 'INSERT INTO [HOUSE] SELECT * FROM OPENROWSET (
               ''MICROSOFT.ACE.OLEDB.12.0'',
               ''dBase 5.0;HDR=YES;IMEX=2;DATABASE=D:\Kolya\fias_dbf'',
               ''SELECT * FROM HOUSE' + convert(varchar(10),@number) + '.dbf'')'
        Exec(@string)
		SET @number = @number + 1
    END;