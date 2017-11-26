/****** Object:  Schema [flat]    Script Date: 10/14/2017 2:34:17 AM ******/
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'flat')
EXEC sys.sp_executesql N'CREATE SCHEMA [flat]'

GO
/****** Object:  Schema [json]    Script Date: 10/14/2017 2:34:17 AM ******/
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'json')
EXEC sys.sp_executesql N'CREATE SCHEMA [json]'

GO
/****** Object:  Table [flat].[BaseballPlayRM]    Script Date: 10/14/2017 2:34:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[flat].[BaseballPlayRM]') AND type in (N'U'))
BEGIN
CREATE TABLE [flat].[BaseballPlayRM](
	[Key] [char](16) NOT NULL,
	[RetrosheetGameId] [char](12) NOT NULL,
	[EventNumber] [smallint] NOT NULL,
	[LineupChangeSequence] [int] NULL,
	[EventText] [varchar](100) NULL,
	[BasicPlay] [varchar](100) NULL,
	[Modifier] [varchar](100) NULL,
	[Inning] [tinyint] NULL,
	[TeamAtBat] [char](1) NULL,
	[BattingOrder] [tinyint] NULL,
	[Batter] [char](8) NULL,
	[Balls] [tinyint] NULL,
	[Strikes] [tinyint] NULL,
	[Pitches] [varchar](50) NULL,
	[Runner1] [varchar](100) NULL,
	[Runner2] [varchar](100) NULL,
	[Runner3] [varchar](100) NULL,
	[RunnerB] [varchar](100) NULL,
	[Runner1Out] [bit] NULL,
	[Runner2Out] [bit] NULL,
	[Runner3Out] [bit] NULL,
	[RunnerBOut] [bit] NULL,
	[OutsOnPlay] [tinyint] NULL,
	[BatterEvent] [bit] NULL,
	[HitValue] [tinyint] NULL,
	[R1Destination] [tinyint] NULL,
	[R2Destination] [tinyint] NULL,
	[R3Destination] [tinyint] NULL,
	[RBDestination] [tinyint] NULL,
	[RunsOnPlay] [tinyint] NULL,
	[AtBat] [bit] NULL,
	[SacHit] [bit] NULL,
	[SacFly] [bit] NULL,
	[WildPitch] [bit] NULL,
	[PassedBall] [bit] NULL,
	[Bunt] [bit] NULL,
	[Foul] [bit] NULL,
	[StolenBaseR1] [bit] NULL,
	[StolenBaseR2] [bit] NULL,
	[StolenBaseR3] [bit] NULL,
	[CaughtStealingR1] [bit] NULL,
	[CaughtStealingR2] [bit] NULL,
	[CaughtStealingR3] [bit] NULL,
	[PickOffR1] [bit] NULL,
	[PickOffR2] [bit] NULL,
	[PickOffR3] [bit] NULL,
	[PlayOnBatter] [varchar](100) NULL,
	[PlayOnR1] [varchar](100) NULL,
	[PlayOnR2] [varchar](100) NULL,
	[PlayOnR3] [varchar](100) NULL,
	[EventType] [varchar](30) NULL,
	[RBIB] [bit] NULL,
	[RBI1] [bit] NULL,
	[RBI2] [bit] NULL,
	[RBI3] [bit] NULL,
	[RBI] [tinyint] NULL,
	[StartOfHalfInning] [bit] NULL,
	[EndOfHalfInning] [bit] NULL,
	[EndOfGame] [bit] NULL,
	[VisitorNextBatter] [tinyint] NULL,
	[HomeNextBatter] [tinyint] NULL,
	[StartOfPlay_Outs] [tinyint] NULL,
	[StartOfPlay_VisitorScore] [tinyint] NULL,
	[StartOfPlay_HomeScore] [tinyint] NULL,
	[StartOfPlay_RunnerOnFirst] [bit] NULL,
	[StartOfPlay_Runner1BO] [tinyint] NULL,
	[StartOfPlay_RunnerOnSecond] [bit] NULL,
	[StartOfPlay_Runner2BO] [tinyint] NULL,
	[StartOfPlay_RunnerOnThird] [bit] NULL,
	[StartOfPlay_Runner3BO] [tinyint] NULL,
	[EndOfPlay_Outs] [tinyint] NULL,
	[EndOfPlay_VisitorScore] [tinyint] NULL,
	[EndOfPlay_HomeScore] [tinyint] NULL,
	[EndOfPlay_RunnerOnFirst] [bit] NULL,
	[EndOfPlay_Runner1BO] [tinyint] NULL,
	[EndOfPlay_RunnerOnSecond] [bit] NULL,
	[EndOfPlay_Runner2BO] [tinyint] NULL,
	[EndOfPlay_RunnerOnThird] [bit] NULL,
	[EndOfPlay_Runner3BO] [tinyint] NULL,
PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [flat].[GameSummaryRM]    Script Date: 10/14/2017 2:34:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[flat].[GameSummaryRM]') AND type in (N'U'))
BEGIN
CREATE TABLE [flat].[GameSummaryRM](
	[Key] [char](12) NOT NULL,
	[RetrosheetGameId] [char](12) NOT NULL,
	[AwayTeam] [char](3) NOT NULL,
	[HomeTeam] [char](3) NOT NULL,
	[UseDH] [bit] NOT NULL,
	[ParkCode] [char](5) NOT NULL,
	[WinningPitcher] [char](8) NULL,
	[LosingPitcher] [char](8) NULL,
	[SavePitcher] [char](8) NULL,
	[HasValidationErrors] [bit] NOT NULL,
	[GameDay] [datetime] NOT NULL,
	[HomeTeamFinalScore] [int] NULL,
	[AwayTeamFinalScore] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [flat].[LineupChangeRM]    Script Date: 10/14/2017 2:34:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[flat].[LineupChangeRM]') AND type in (N'U'))
BEGIN
CREATE TABLE [flat].[LineupChangeRM](
	[Key] [char](20) NOT NULL,
	[RetrosheetGameId] [char](12) NOT NULL,
	[EventNumber] [smallint] NOT NULL,
	[Sequence] [smallint] NOT NULL,
	[IsStarter] [bit] NOT NULL,
	[PlayerId] [char](8) NOT NULL,
	[Name] [varchar](101) NULL,
	[Team] [tinyint] NOT NULL,
	[BattingOrder] [tinyint] NOT NULL,
	[FieldPosition] [tinyint] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [flat].[LineupRM]    Script Date: 10/14/2017 2:34:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[flat].[LineupRM]') AND type in (N'U'))
BEGIN
CREATE TABLE [flat].[LineupRM](
	[Key] [char](16) NOT NULL,
	[RetrosheetGameId] [char](12) NOT NULL,
	[EventNumber] [smallint] NOT NULL,
	[Sequence] [smallint] NOT NULL,
	[Home_BO0_PlayerId] [char](8) NULL,
	[Home_BO0_FieldPosition] [tinyint] NULL,
	[Home_BO1_PlayerId] [char](8) NULL,
	[Home_BO1_FieldPosition] [tinyint] NULL,
	[Home_BO2_PlayerId] [char](8) NULL,
	[Home_BO2_FieldPosition] [tinyint] NULL,
	[Home_BO3_PlayerId] [char](8) NULL,
	[Home_BO3_FieldPosition] [tinyint] NULL,
	[Home_BO4_PlayerId] [char](8) NULL,
	[Home_BO4_FieldPosition] [tinyint] NULL,
	[Home_BO5_PlayerId] [char](8) NULL,
	[Home_BO5_FieldPosition] [tinyint] NULL,
	[Home_BO6_PlayerId] [char](8) NULL,
	[Home_BO6_FieldPosition] [tinyint] NULL,
	[Home_BO7_PlayerId] [char](8) NULL,
	[Home_BO7_FieldPosition] [tinyint] NULL,
	[Home_BO8_PlayerId] [char](8) NULL,
	[Home_BO8_FieldPosition] [tinyint] NULL,
	[Home_BO9_PlayerId] [char](8) NULL,
	[Home_BO9_FieldPosition] [tinyint] NULL,
	[Away_BO0_PlayerId] [char](8) NULL,
	[Away_BO0_FieldPosition] [tinyint] NULL,
	[Away_BO1_PlayerId] [char](8) NULL,
	[Away_BO1_FieldPosition] [tinyint] NULL,
	[Away_BO2_PlayerId] [char](8) NULL,
	[Away_BO2_FieldPosition] [tinyint] NULL,
	[Away_BO3_PlayerId] [char](8) NULL,
	[Away_BO3_FieldPosition] [tinyint] NULL,
	[Away_BO4_PlayerId] [char](8) NULL,
	[Away_BO4_FieldPosition] [tinyint] NULL,
	[Away_BO5_PlayerId] [char](8) NULL,
	[Away_BO5_FieldPosition] [tinyint] NULL,
	[Away_BO6_PlayerId] [char](8) NULL,
	[Away_BO6_FieldPosition] [tinyint] NULL,
	[Away_BO7_PlayerId] [char](8) NULL,
	[Away_BO7_FieldPosition] [tinyint] NULL,
	[Away_BO8_PlayerId] [char](8) NULL,
	[Away_BO8_FieldPosition] [tinyint] NULL,
	[Away_BO9_PlayerId] [char](8) NULL,
	[Away_BO9_FieldPosition] [tinyint] NULL,
PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [flat].[PlayerRM]    Script Date: 10/14/2017 2:34:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[flat].[PlayerRM]') AND type in (N'U'))
BEGIN
CREATE TABLE [flat].[PlayerRM](
	[Key] [char](8) NOT NULL,
	[PlayerId] [char](8) NOT NULL,
	[LastName] [varchar](50) NOT NULL,
	[FirstName] [varchar](50) NOT NULL,
	[Bats] [char](1) NOT NULL,
	[Throws] [char](1) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [flat].[RosterMemberRM]    Script Date: 10/14/2017 2:34:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[flat].[RosterMemberRM]') AND type in (N'U'))
BEGIN
CREATE TABLE [flat].[RosterMemberRM](
	[Key] [char](17) NOT NULL,
	[Year] [int] NOT NULL,
	[TeamCode] [char](3) NOT NULL,
	[PlayerId] [char](8) NOT NULL,
	[LastName] [varchar](50) NOT NULL,
	[FirstName] [varchar](50) NOT NULL,
	[Bats] [char](1) NOT NULL,
	[Throws] [char](1) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [flat].[TeamRM]    Script Date: 10/14/2017 2:34:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[flat].[TeamRM]') AND type in (N'U'))
BEGIN
CREATE TABLE [flat].[TeamRM](
	[Key] [char](8) NOT NULL,
	[Year] [int] NOT NULL,
	[TeamCode] [char](3) NOT NULL,
	[League] [char](1) NOT NULL,
	[Home] [varchar](20) NOT NULL,
	[Name] [varchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [json].[BaseballPlayRM]    Script Date: 10/14/2017 2:34:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[json].[BaseballPlayRM]') AND type in (N'U'))
BEGIN
CREATE TABLE [json].[BaseballPlayRM](
	[Key] [varchar](100) NOT NULL,
	[Json] [nvarchar](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [json].[GameSummaryRM]    Script Date: 10/14/2017 2:34:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[json].[GameSummaryRM]') AND type in (N'U'))
BEGIN
CREATE TABLE [json].[GameSummaryRM](
	[Key] [varchar](100) NOT NULL,
	[Json] [nvarchar](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [json].[LineupChangeRM]    Script Date: 10/14/2017 2:34:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[json].[LineupChangeRM]') AND type in (N'U'))
BEGIN
CREATE TABLE [json].[LineupChangeRM](
	[Key] [varchar](100) NOT NULL,
	[Json] [nvarchar](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [json].[LineupRM]    Script Date: 10/14/2017 2:34:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[json].[LineupRM]') AND type in (N'U'))
BEGIN
CREATE TABLE [json].[LineupRM](
	[Key] [varchar](100) NOT NULL,
	[Json] [nvarchar](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [json].[PlayerRM]    Script Date: 10/14/2017 2:34:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[json].[PlayerRM]') AND type in (N'U'))
BEGIN
CREATE TABLE [json].[PlayerRM](
	[Key] [varchar](100) NOT NULL,
	[Json] [nvarchar](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [json].[RosterMemberRM]    Script Date: 10/14/2017 2:34:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[json].[RosterMemberRM]') AND type in (N'U'))
BEGIN
CREATE TABLE [json].[RosterMemberRM](
	[Key] [varchar](100) NOT NULL,
	[Json] [nvarchar](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [json].[TeamRM]    Script Date: 10/14/2017 2:34:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[json].[TeamRM]') AND type in (N'U'))
BEGIN
CREATE TABLE [json].[TeamRM](
	[Key] [varchar](100) NOT NULL,
	[Json] [nvarchar](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO





/****** Object:  Table [dbo].[Log]    Script Date: 11/12/2017 8:24:19 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Log]') AND type in (N'U'))
BEGIN

CREATE TABLE [dbo].[Log](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Application] [nvarchar](50) NOT NULL,
	[Logged] [datetime] NOT NULL,
	[Level] [nvarchar](50) NOT NULL,
	[Message] [nvarchar](max) NOT NULL,
	[Logger] [nvarchar](250) NULL,
 CONSTRAINT [PK_dbo.Log] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO


