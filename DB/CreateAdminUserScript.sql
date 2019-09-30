INSERT INTO [dbo].[User]
           ([CargoWiseId]
           ,[CargoWiseBardCode]
           ,[CompanyName]
           ,[ClientId]
           ,[IsClient]
           ,[CountryOfOperation]
           ,[ContactName]
           ,[Email]
           ,[TelephoneNo]
           ,[MobileNo]
           ,[FaxNumber]
           ,[WorkingStartTime]
           ,[WorkingEndTime]
           ,[TimezoneId]
           ,[VATGST]
           ,[ShortName]
           ,[Position]
           ,[Skype]
           ,[IsActive]
           ,[CreatedOn]
           ,[CreatedBy]
           ,[UpdatedOn]
           ,[UpdatedBy])
     VALUES
           (null
		   ,null
		   ,'Frayte Admin Pvt Ltd'
           ,0
           ,0
           ,null
           ,'Frayte Admin'
           ,'admin@frayte.com'
           ,'1234567890'
           ,'9457374801'
           ,'1234567890'
           ,null
           ,null
           ,null
           ,null
           ,'Frayte Admin'
           ,'Admin'
           ,'admin.frayte'
           ,1
           ,getdate()
           ,1
           ,null
           ,0)
GO

INSERT INTO [dbo].[UserLogin]
           ([UserId]
           ,[UserName]
           ,[Password]
           ,[PasswordSalt]
           ,[IsActive]
           ,[IsLocked]
           ,[LastLoginDate]
           ,[LastPasswordChangeDate]
           ,[FailedPasswordAttmemptCount]
           ,[IsRecoveryMailSent]
           ,[ProfileImage])
     VALUES
           (1
           ,'admin@frayte.com'
           ,'1234'
           ,'1234'
           ,1
           ,0
           ,getdate()
           ,GETDATE()
           ,0
           ,0
           ,null)
GO

INSERT INTO [dbo].[UserRole]
           ([UserId]
           ,[RoleId])
     VALUES
           (1
           ,1)