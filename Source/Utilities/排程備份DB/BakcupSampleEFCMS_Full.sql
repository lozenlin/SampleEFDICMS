print convert(varchar(20), getdate(), 120)
declare @FileFullName nvarchar(500)
set @FileFullName = N'D:\SqlDataBackup\SQLEXPRESS2012\SampleEFCMS\SampleEFCMS_'+replace(replace(replace(convert(varchar(20), getdate(), 120), '-', ''), ':', ''), ' ', '_')+N'.bak'

BACKUP DATABASE [SampleEFCMS] TO  DISK = @FileFullName WITH NOFORMAT, NOINIT,  NAME = N'SampleEFCMS-完整 資料庫 備份', SKIP, NOREWIND, NOUNLOAD,  STATS = 10
print @FileFullName
GO
