DECLARE @number INT = 1

WHILE @number < 10
    BEGIN
	DECLARE @string nvarchar(200) = 'INSERT INTO [ADDROB] SELECT  * FROM OPENROWSET (
               ''MICROSOFT.ACE.OLEDB.12.0'',
               ''dBase 5.0;HDR=YES;IMEX=2;DATABASE=D:\Kolya'',
               ''SELECT * FROM ADDROB0' + convert(varchar(10),@number) + '.dbf'')'
        Exec(@string)
		SET @number = @number + 1
    END;
