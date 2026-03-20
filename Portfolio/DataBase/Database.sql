USE master;
GO

IF DB_ID('PortfolioDb') IS NOT NULL
BEGIN
    ALTER DATABASE PortfolioDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE PortfolioDb;
END
GO

CREATE DATABASE PortfolioDb;
GO

USE PortfolioDb;
GO

/* =========================================================
   ADMINS
========================================================= */
CREATE TABLE Admins
(
    AdminId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE()
);
GO

/* =========================================================
   ABOUTS
========================================================= */
CREATE TABLE Abouts
(
    AboutId INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(150) NULL,
    Description NVARCHAR(MAX) NULL,
    ProfileImageUrl NVARCHAR(255) NULL,
    CvUrl NVARCHAR(255) NULL,
    UpdatedDate DATETIME NULL
);
GO

/* =========================================================
   SKILLCATEGORIES
========================================================= */
CREATE TABLE SkillCategories
(
    SkillCategoryId INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL,
    DisplayName NVARCHAR(150) NOT NULL,
    ColorClass NVARCHAR(100) NULL,
    HexColor NVARCHAR(20) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    DisplayOrder INT NOT NULL DEFAULT 0
);
GO

/* =========================================================
   SKILLS
========================================================= */
CREATE TABLE Skills
(
    SkillId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Percentage INT NOT NULL DEFAULT 0,
    Category NVARCHAR(100) NULL,
    Icon NVARCHAR(100) NULL,
    DisplayOrder INT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1
);
GO

/* =========================================================
   SOCIALMEDIAS
========================================================= */
CREATE TABLE SocialMedias
(
    SocialMediaId INT IDENTITY(1,1) PRIMARY KEY,
    PlatformName NVARCHAR(100) NOT NULL,
    Url NVARCHAR(255) NOT NULL,
    IconClass NVARCHAR(100) NULL,
    DisplayOrder INT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1
);
GO

/* =========================================================
   SITESETTINGS
========================================================= */
CREATE TABLE SiteSettings
(
    SiteSettingId INT IDENTITY(1,1) PRIMARY KEY,
    SiteTitle NVARCHAR(150) NULL,
    HeroTitle NVARCHAR(150) NULL,
    HeroSubtitle NVARCHAR(250) NULL,
    HeroBackgroundImageUrl NVARCHAR(255) NULL,
    FooterText NVARCHAR(200) NULL
);
GO

/* =========================================================
   CONTACTSECTIONSETTINGS
========================================================= */
CREATE TABLE ContactSectionSettings
(
    ContactSectionId INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(150) NULL,
    Description NVARCHAR(MAX) NULL,
    BadgeText NVARCHAR(150) NULL,

    Box1Icon NVARCHAR(100) NULL,
    Box1Title NVARCHAR(100) NULL,
    Box1Text NVARCHAR(200) NULL,
    Box1Url NVARCHAR(255) NULL,

    Box2Icon NVARCHAR(100) NULL,
    Box2Title NVARCHAR(100) NULL,
    Box2Text NVARCHAR(200) NULL,
    Box2Url NVARCHAR(255) NULL,

    Box3Icon NVARCHAR(100) NULL,
    Box3Title NVARCHAR(100) NULL,
    Box3Text NVARCHAR(200) NULL,
    Box3Url NVARCHAR(255) NULL,

    UpdatedDate DATETIME NULL
);
GO

/* =========================================================
   MESSAGES
========================================================= */
CREATE TABLE Messages
(
    MessageId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(150) NOT NULL,
    Subject NVARCHAR(200) NULL,
    MessageContent NVARCHAR(MAX) NOT NULL,
    SendDate DATETIME NOT NULL DEFAULT GETDATE(),
    IsRead BIT NOT NULL DEFAULT 0
);
GO

/* =========================================================
   LOGINLOGS
========================================================= */
CREATE TABLE LoginLogs
(
    LoginLogId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NULL,
    IsSuccess BIT NOT NULL,
    IpAddress NVARCHAR(50) NULL,
    Description NVARCHAR(255) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);
GO

/* =========================================================
   PROJECTS
========================================================= */
CREATE TABLE Projects
(
    ProjectId INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(150) NOT NULL,
    ShortDescription NVARCHAR(300) NULL,
    Description NVARCHAR(MAX) NULL,
    Technologies NVARCHAR(300) NULL,
    GithubUrl NVARCHAR(255) NULL,
    LiveDemoUrl NVARCHAR(255) NULL,
    ThumbnailUrl NVARCHAR(255) NULL,
    IsFeatured BIT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    DisplayOrder INT NOT NULL DEFAULT 0,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedDate DATETIME NULL
);
GO

/* =========================================================
   PROJECTIMAGES
========================================================= */
CREATE TABLE ProjectImages
(
    ProjectImageId INT IDENTITY(1,1) PRIMARY KEY,
    ProjectId INT NOT NULL,
    ImageUrl NVARCHAR(255) NOT NULL,
    IsMain BIT NOT NULL DEFAULT 0,
    DisplayOrder INT NOT NULL DEFAULT 0,

    CONSTRAINT FK_ProjectImages_Projects
        FOREIGN KEY (ProjectId) REFERENCES Projects(ProjectId)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);
GO

/* =========================================================
   PROJECTSECTIONS
========================================================= */
CREATE TABLE ProjectSections
(
    ProjectSectionId INT IDENTITY(1,1) PRIMARY KEY,
    ProjectId INT NOT NULL,
    Title NVARCHAR(150) NULL,
    Description NVARCHAR(MAX) NULL,
    ImageUrl NVARCHAR(255) NULL,
    BulletList NVARCHAR(MAX) NULL,
    ButtonText NVARCHAR(100) NULL,
    ButtonUrl NVARCHAR(255) NULL,
    ImagePosition NVARCHAR(20) NULL,
    DisplayOrder INT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedDate DATETIME NULL,

    CONSTRAINT FK_ProjectSections_Projects
        FOREIGN KEY (ProjectId) REFERENCES Projects(ProjectId)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);
GO

/* =========================================================
   INDEXES
========================================================= */
CREATE INDEX IX_Projects_DisplayOrder ON Projects(DisplayOrder);
CREATE INDEX IX_ProjectSections_ProjectId ON ProjectSections(ProjectId);
CREATE INDEX IX_ProjectSections_DisplayOrder ON ProjectSections(DisplayOrder);
CREATE INDEX IX_ProjectImages_ProjectId ON ProjectImages(ProjectId);
CREATE INDEX IX_Skills_DisplayOrder ON Skills(DisplayOrder);
CREATE INDEX IX_SkillCategories_DisplayOrder ON SkillCategories(DisplayOrder);
CREATE INDEX IX_Messages_SendDate ON Messages(SendDate);
CREATE INDEX IX_LoginLogs_CreatedAt ON LoginLogs(CreatedAt);
GO

/* =========================================================
   DEFAULT SEED DATA
========================================================= */

-- Admin account
INSERT INTO Admins (Username, PasswordHash, FullName, IsActive, CreatedDate)
VALUES 
('admin', 'TEMP_HASH_VALUE', 'Admin Name', 1, GETDATE());
GO

-- About section
INSERT INTO Abouts (Title, Description, ProfileImageUrl, CvUrl, UpdatedDate)
VALUES
('Merhaba, Ben Admin',
 N'Hakkımda açıklama alanı.',
 '~/Content/img/default-profile.jpg',
 NULL,
 GETDATE());
GO

-- Site settings
INSERT INTO SiteSettings (SiteTitle, HeroTitle, HeroSubtitle, HeroBackgroundImageUrl, FooterText)
VALUES
('User Portfolio',
 'Merhaba, Ben User',
 'Backend Developer & Frontend Enthusiast',
 '~/Content/img/default-hero.jpg',
 '© 2026 User');
GO

-- Contact section settings
INSERT INTO ContactSectionSettings
(
    Title, Description, BadgeText,
    Box1Icon, Box1Title, Box1Text, Box1Url,
    Box2Icon, Box2Title, Box2Text, Box2Url,
    Box3Icon, Box3Title, Box3Text, Box3Url,
    UpdatedDate
)
VALUES
(
    N'Birlikte bir şeyler üretelim',
    N'Yazılım, portfolyo, proje geliştirme ya da iş fırsatları hakkında benimle iletişime geçebilirsin. Mesajını gördüğümde en kısa sürede dönüş yaparım.',
    N'Açık, net ve hızlı iletişim',

    'bi-envelope', 'E-posta', 'a@mail.com', 'mailto:a@mail.com',
    'bi-github', 'GitHub', 'github.com', 'https://github.com',
    'bi-linkedin', 'LinkedIn', 'linkedin.com/in', 'https://linkedin.com/in',

    GETDATE()
);
GO

-- Skill categories
INSERT INTO SkillCategories (CategoryName, DisplayName, ColorClass, HexColor, IsActive, DisplayOrder)
VALUES
('Backend', 'Backend Development', 'bg-primary', '#2563eb', 1, 1),
('Frontend', 'Frontend Development', 'bg-success', '#10b981', 1, 2),
('Database', 'Database Management', 'bg-warning', '#f59e0b', 1, 3),
('Tools', 'Tools & Others', 'bg-dark', '#111827', 1, 4);
GO

-- Skills
INSERT INTO Skills (Name, Percentage, Category, Icon, DisplayOrder, IsActive)
VALUES
('ASP.NET MVC', 85, 'Backend', NULL, 1, 1),
('C#', 85, 'Backend', NULL, 2, 1),
('Entity Framework', 80, 'Backend', NULL, 3, 1),
('HTML', 80, 'Frontend', NULL, 4, 1),
('CSS', 75, 'Frontend', NULL, 5, 1),
('Bootstrap', 80, 'Frontend', NULL, 6, 1),
('SQL Server', 80, 'Database', NULL, 7, 1),
('MySQL', 75, 'Database', NULL, 8, 1),
('Git', 70, 'Tools', NULL, 9, 1);
GO

-- Social media
INSERT INTO SocialMedias (PlatformName, Url, IconClass, DisplayOrder, IsActive)
VALUES
('GitHub', 'https://github.com', 'bi bi-github', 1, 1),
('LinkedIn', 'https://linkedin.com/in', 'bi bi-linkedin', 2, 1),
('Mail', 'mailto:a@mail.com', 'bi bi-envelope', 3, 1);
GO