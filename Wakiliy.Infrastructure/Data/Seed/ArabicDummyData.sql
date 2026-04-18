SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

DECLARE @AdminId nvarchar(450) = N'seed-admin-001';
DECLARE @ClientId nvarchar(450) = N'seed-client-001';
DECLARE @Lawyer1Id nvarchar(450) = N'seed-lawyer-001';
DECLARE @Lawyer2Id nvarchar(450) = N'seed-lawyer-002';

DECLARE @AdminProfileImageId uniqueidentifier = '11111111-1111-1111-1111-111111111111';
DECLARE @ClientProfileImageId uniqueidentifier = '22222222-2222-2222-2222-222222222222';
DECLARE @Lawyer1ProfileImageId uniqueidentifier = '33333333-3333-3333-3333-333333333333';
DECLARE @Lawyer2ProfileImageId uniqueidentifier = '44444444-4444-4444-4444-444444444444';
DECLARE @Lawyer1NationalFrontFileId uniqueidentifier = '55555555-5555-5555-5555-555555555551';
DECLARE @Lawyer1NationalBackFileId uniqueidentifier = '55555555-5555-5555-5555-555555555552';
DECLARE @Lawyer1LicenseFileId uniqueidentifier = '55555555-5555-5555-5555-555555555553';
DECLARE @Lawyer1CertificateFileId uniqueidentifier = '55555555-5555-5555-5555-555555555554';
DECLARE @Lawyer1DegreeFileId uniqueidentifier = '55555555-5555-5555-5555-555555555555';
DECLARE @Lawyer2LicenseFileId uniqueidentifier = '66666666-6666-6666-6666-666666666661';
DECLARE @Lawyer2CertificateFileId uniqueidentifier = '66666666-6666-6666-6666-666666666662';
DECLARE @Lawyer2DegreeFileId uniqueidentifier = '66666666-6666-6666-6666-666666666663';

DECLARE @Lawyer1QualificationId uniqueidentifier = '77777777-7777-7777-7777-777777777771';
DECLARE @Lawyer2QualificationId uniqueidentifier = '77777777-7777-7777-7777-777777777772';
DECLARE @Lawyer1CertificationId uniqueidentifier = '88888888-8888-8888-8888-888888888881';
DECLARE @Lawyer2CertificationId uniqueidentifier = '88888888-8888-8888-8888-888888888882';
DECLARE @Lawyer1WorkExperienceId uniqueidentifier = '99999999-9999-9999-9999-999999999991';
DECLARE @Lawyer2WorkExperienceId uniqueidentifier = '99999999-9999-9999-9999-999999999992';
DECLARE @Lawyer1VerificationNationalFrontId uniqueidentifier = 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1';
DECLARE @Lawyer1VerificationNationalBackId uniqueidentifier = 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2';
DECLARE @Lawyer1VerificationLicenseId uniqueidentifier = 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3';
DECLARE @Lawyer1VerificationCertificateId uniqueidentifier = 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4';
DECLARE @Lawyer2VerificationLicenseId uniqueidentifier = 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1';
DECLARE @Lawyer2VerificationCertificateId uniqueidentifier = 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2';
DECLARE @Appointment1Id uniqueidentifier = 'cccccccc-cccc-cccc-cccc-ccccccccccc1';
DECLARE @Appointment2Id uniqueidentifier = 'cccccccc-cccc-cccc-cccc-ccccccccccc2';
DECLARE @Review1Id uniqueidentifier = 'dddddddd-dddd-dddd-dddd-ddddddddddd1';
DECLARE @Review2Id uniqueidentifier = 'dddddddd-dddd-dddd-dddd-ddddddddddd2';
DECLARE @SystemReviewId uniqueidentifier = 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee1';

DECLARE @SpecializationCorporateId int;
DECLARE @SpecializationFamilyId int;
DECLARE @SpecializationLaborId int;
DECLARE @Slot1Id int;
DECLARE @Slot2Id int;

IF NOT EXISTS (SELECT 1 FROM [UploadedFiles] WHERE [Id] = @AdminProfileImageId)
BEGIN
    INSERT INTO [UploadedFiles] ([Id], [OwnerId], [FileName], [PublicId], [SystemFileUrl], [Size], [ContentType], [Category], [Purpose], [UploadedAt])
    VALUES (@AdminProfileImageId, @AdminId, N'صورة-المشرف.jpg', N'admin-profile-ar', N'https://example.com/files/admin-profile-ar.jpg', 51200, N'image/jpeg', N'ProfilePicture', N'Profile', '2026-04-15T09:00:00');
END;

IF NOT EXISTS (SELECT 1 FROM [UploadedFiles] WHERE [Id] = @ClientProfileImageId)
BEGIN
    INSERT INTO [UploadedFiles] ([Id], [OwnerId], [FileName], [PublicId], [SystemFileUrl], [Size], [ContentType], [Category], [Purpose], [UploadedAt])
    VALUES (@ClientProfileImageId, @ClientId, N'صورة-العميل.jpg', N'client-profile-ar', N'https://example.com/files/client-profile-ar.jpg', 63488, N'image/jpeg', N'ProfilePicture', N'Profile', '2026-04-15T09:05:00');
END;

IF NOT EXISTS (SELECT 1 FROM [UploadedFiles] WHERE [Id] = @Lawyer1ProfileImageId)
BEGIN
    INSERT INTO [UploadedFiles] ([Id], [OwnerId], [FileName], [PublicId], [SystemFileUrl], [Size], [ContentType], [Category], [Purpose], [UploadedAt])
    VALUES (@Lawyer1ProfileImageId, @Lawyer1Id, N'صورة-المحامي-أحمد.jpg', N'lawyer-ahmed-profile-ar', N'https://example.com/files/lawyer-ahmed-profile-ar.jpg', 71680, N'image/jpeg', N'ProfilePicture', N'Profile', '2026-04-15T09:10:00');
END;

IF NOT EXISTS (SELECT 1 FROM [UploadedFiles] WHERE [Id] = @Lawyer2ProfileImageId)
BEGIN
    INSERT INTO [UploadedFiles] ([Id], [OwnerId], [FileName], [PublicId], [SystemFileUrl], [Size], [ContentType], [Category], [Purpose], [UploadedAt])
    VALUES (@Lawyer2ProfileImageId, @Lawyer2Id, N'صورة-المحامية-سارة.jpg', N'lawyer-sara-profile-ar', N'https://example.com/files/lawyer-sara-profile-ar.jpg', 74240, N'image/jpeg', N'ProfilePicture', N'Profile', '2026-04-15T09:15:00');
END;

IF NOT EXISTS (SELECT 1 FROM [UploadedFiles] WHERE [Id] = @Lawyer1NationalFrontFileId)
BEGIN
    INSERT INTO [UploadedFiles] ([Id], [OwnerId], [FileName], [PublicId], [SystemFileUrl], [Size], [ContentType], [Category], [Purpose], [UploadedAt])
    VALUES (@Lawyer1NationalFrontFileId, @Lawyer1Id, N'الهوية-الأمامية-أحمد.pdf', N'lawyer-ahmed-national-front-ar', N'https://example.com/files/lawyer-ahmed-national-front-ar.pdf', 128000, N'application/pdf', N'NationalIdFront', N'Verification', '2026-04-15T09:20:00');
END;

IF NOT EXISTS (SELECT 1 FROM [UploadedFiles] WHERE [Id] = @Lawyer1NationalBackFileId)
BEGIN
    INSERT INTO [UploadedFiles] ([Id], [OwnerId], [FileName], [PublicId], [SystemFileUrl], [Size], [ContentType], [Category], [Purpose], [UploadedAt])
    VALUES (@Lawyer1NationalBackFileId, @Lawyer1Id, N'الهوية-الخلفية-أحمد.pdf', N'lawyer-ahmed-national-back-ar', N'https://example.com/files/lawyer-ahmed-national-back-ar.pdf', 126000, N'application/pdf', N'NationalIdBack', N'Verification', '2026-04-15T09:21:00');
END;

IF NOT EXISTS (SELECT 1 FROM [UploadedFiles] WHERE [Id] = @Lawyer1LicenseFileId)
BEGIN
    INSERT INTO [UploadedFiles] ([Id], [OwnerId], [FileName], [PublicId], [SystemFileUrl], [Size], [ContentType], [Category], [Purpose], [UploadedAt])
    VALUES (@Lawyer1LicenseFileId, @Lawyer1Id, N'رخصة-المحاماة-أحمد.pdf', N'lawyer-ahmed-license-ar', N'https://example.com/files/lawyer-ahmed-license-ar.pdf', 145500, N'application/pdf', N'LawyerLicense', N'Verification', '2026-04-15T09:22:00');
END;

IF NOT EXISTS (SELECT 1 FROM [UploadedFiles] WHERE [Id] = @Lawyer1CertificateFileId)
BEGIN
    INSERT INTO [UploadedFiles] ([Id], [OwnerId], [FileName], [PublicId], [SystemFileUrl], [Size], [ContentType], [Category], [Purpose], [UploadedAt])
    VALUES (@Lawyer1CertificateFileId, @Lawyer1Id, N'شهادة-مهنية-أحمد.pdf', N'lawyer-ahmed-prof-cert-ar', N'https://example.com/files/lawyer-ahmed-prof-cert-ar.pdf', 118000, N'application/pdf', N'ProfessionalCertificate', N'Verification', '2026-04-15T09:23:00');
END;

IF NOT EXISTS (SELECT 1 FROM [UploadedFiles] WHERE [Id] = @Lawyer1DegreeFileId)
BEGIN
    INSERT INTO [UploadedFiles] ([Id], [OwnerId], [FileName], [PublicId], [SystemFileUrl], [Size], [ContentType], [Category], [Purpose], [UploadedAt])
    VALUES (@Lawyer1DegreeFileId, @Lawyer1Id, N'شهادة-البكالوريوس-أحمد.pdf', N'lawyer-ahmed-degree-ar', N'https://example.com/files/lawyer-ahmed-degree-ar.pdf', 152000, N'application/pdf', N'EducationalCertificate', N'Verification', '2026-04-15T09:24:00');
END;

IF NOT EXISTS (SELECT 1 FROM [UploadedFiles] WHERE [Id] = @Lawyer2LicenseFileId)
BEGIN
    INSERT INTO [UploadedFiles] ([Id], [OwnerId], [FileName], [PublicId], [SystemFileUrl], [Size], [ContentType], [Category], [Purpose], [UploadedAt])
    VALUES (@Lawyer2LicenseFileId, @Lawyer2Id, N'رخصة-المحاماة-سارة.pdf', N'lawyer-sara-license-ar', N'https://example.com/files/lawyer-sara-license-ar.pdf', 149000, N'application/pdf', N'LawyerLicense', N'Verification', '2026-04-15T09:25:00');
END;

IF NOT EXISTS (SELECT 1 FROM [UploadedFiles] WHERE [Id] = @Lawyer2CertificateFileId)
BEGIN
    INSERT INTO [UploadedFiles] ([Id], [OwnerId], [FileName], [PublicId], [SystemFileUrl], [Size], [ContentType], [Category], [Purpose], [UploadedAt])
    VALUES (@Lawyer2CertificateFileId, @Lawyer2Id, N'شهادة-مهنية-سارة.pdf', N'lawyer-sara-prof-cert-ar', N'https://example.com/files/lawyer-sara-prof-cert-ar.pdf', 121000, N'application/pdf', N'ProfessionalCertificate', N'Verification', '2026-04-15T09:26:00');
END;

IF NOT EXISTS (SELECT 1 FROM [UploadedFiles] WHERE [Id] = @Lawyer2DegreeFileId)
BEGIN
    INSERT INTO [UploadedFiles] ([Id], [OwnerId], [FileName], [PublicId], [SystemFileUrl], [Size], [ContentType], [Category], [Purpose], [UploadedAt])
    VALUES (@Lawyer2DegreeFileId, @Lawyer2Id, N'ماجستير-القانون-سارة.pdf', N'lawyer-sara-degree-ar', N'https://example.com/files/lawyer-sara-degree-ar.pdf', 154500, N'application/pdf', N'EducationalCertificate', N'Verification', '2026-04-15T09:27:00');
END;

IF NOT EXISTS (SELECT 1 FROM [AspNetUsers] WHERE [Id] = @AdminId)
BEGIN
    INSERT INTO [AspNetUsers] ([Id], [FirstName], [LastName], [ProfileImageId], [AcceptTerms], [Gender], [City], [Country], [Status], [CreatedAt], [UpdatedAt], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount])
    VALUES (@AdminId, N'مشرف', N'النظام', @AdminProfileImageId, 1, N'ذكر', N'القاهرة', N'مصر', N'Active', '2026-04-15T10:00:00', NULL, N'admin.ar', N'ADMIN.AR', N'admin.ar@wakiliy.test', N'ADMIN.AR@WAKILIY.TEST', 1, NULL, CONVERT(nvarchar(36), NEWID()), CONVERT(nvarchar(36), NEWID()), N'01000000001', 1, 0, NULL, 0, 0);
END;

IF NOT EXISTS (SELECT 1 FROM [AspNetUsers] WHERE [Id] = @ClientId)
BEGIN
    INSERT INTO [AspNetUsers] ([Id], [FirstName], [LastName], [ProfileImageId], [AcceptTerms], [Gender], [City], [Country], [Status], [CreatedAt], [UpdatedAt], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount])
    VALUES (@ClientId, N'ليلى', N'حسن', @ClientProfileImageId, 1, N'أنثى', N'الإسكندرية', N'مصر', N'Active', '2026-04-15T10:05:00', NULL, N'leila.hassan', N'LEILA.HASSAN', N'leila.hassan@wakiliy.test', N'LEILA.HASSAN@WAKILIY.TEST', 1, NULL, CONVERT(nvarchar(36), NEWID()), CONVERT(nvarchar(36), NEWID()), N'01000000002', 1, 0, NULL, 0, 0);
END;

IF NOT EXISTS (SELECT 1 FROM [AspNetUsers] WHERE [Id] = @Lawyer1Id)
BEGIN
    INSERT INTO [AspNetUsers] ([Id], [FirstName], [LastName], [ProfileImageId], [AcceptTerms], [Gender], [City], [Country], [Status], [CreatedAt], [UpdatedAt], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount])
    VALUES (@Lawyer1Id, N'أحمد', N'الشافعي', @Lawyer1ProfileImageId, 1, N'ذكر', N'القاهرة', N'مصر', N'Active', '2026-04-15T10:10:00', NULL, N'ahmed.elshafie', N'AHMED.ELSHAFIE', N'ahmed.elshafie@wakiliy.test', N'AHMED.ELSHAFIE@WAKILIY.TEST', 1, NULL, CONVERT(nvarchar(36), NEWID()), CONVERT(nvarchar(36), NEWID()), N'01000000003', 1, 0, NULL, 0, 0);
END;

IF NOT EXISTS (SELECT 1 FROM [AspNetUsers] WHERE [Id] = @Lawyer2Id)
BEGIN
    INSERT INTO [AspNetUsers] ([Id], [FirstName], [LastName], [ProfileImageId], [AcceptTerms], [Gender], [City], [Country], [Status], [CreatedAt], [UpdatedAt], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount])
    VALUES (@Lawyer2Id, N'سارة', N'عبدالرحمن', @Lawyer2ProfileImageId, 1, N'أنثى', N'المنصورة', N'مصر', N'Active', '2026-04-15T10:15:00', NULL, N'sara.abdelrahman', N'SARA.ABDELRAHMAN', N'sara.abdelrahman@wakiliy.test', N'SARA.ABDELRAHMAN@WAKILIY.TEST', 1, NULL, CONVERT(nvarchar(36), NEWID()), CONVERT(nvarchar(36), NEWID()), N'01000000004', 1, 0, NULL, 0, 0);
END;

IF NOT EXISTS (SELECT 1 FROM [Clients] WHERE [Id] = @ClientId)
BEGIN
    INSERT INTO [Clients] ([Id], [NationalId], [Bio])
    VALUES (@ClientId, N'29801011234567', N'عميلة تبحث عن استشارة قانونية بخصوص نزاع عائلي وإجراءات النفقة.');
END;

IF NOT EXISTS (SELECT 1 FROM [Lawyers] WHERE [Id] = @Lawyer1Id)
BEGIN
    INSERT INTO [Lawyers] ([Id], [JoinedDate], [Bio], [YearsOfExperience], [LicenseNumber], [IssuingAuthority], [LicenseYear], [SessionTypes], [VerificationStatus], [ApprovedById], [ApprovedAt], [RejectedById], [RejectedAt], [CurrentOnboardingStep], [CompletedOnboardingSteps], [LastOnboardingUpdate], [AverageRating], [IsActive], [PhoneSessionPrice], [InOfficeSessionPrice])
    VALUES (@Lawyer1Id, '2021-06-01T00:00:00', N'محام متخصص في القضايا التجارية وصياغة العقود وتمثيل الشركات الناشئة.', 8, N'قيد/تجاري/١٢٣٤', N'نقابة المحامين المصرية', N'2018', N'["InOffice","Phone"]', 2, @AdminId, '2026-04-16T12:00:00', NULL, NULL, 4, N'[1,2,3,4]', '2026-04-16T12:00:00', 4.8, 1, 350.00, 500.00);
END;

IF NOT EXISTS (SELECT 1 FROM [Lawyers] WHERE [Id] = @Lawyer2Id)
BEGIN
    INSERT INTO [Lawyers] ([Id], [JoinedDate], [Bio], [YearsOfExperience], [LicenseNumber], [IssuingAuthority], [LicenseYear], [SessionTypes], [VerificationStatus], [ApprovedById], [ApprovedAt], [RejectedById], [RejectedAt], [CurrentOnboardingStep], [CompletedOnboardingSteps], [LastOnboardingUpdate], [AverageRating], [IsActive], [PhoneSessionPrice], [InOfficeSessionPrice])
    VALUES (@Lawyer2Id, '2020-02-15T00:00:00', N'محامية متخصصة في الأحوال الشخصية وقضايا العمل والتسويات الودية.', 10, N'قيد/أحوال/٥٦٧٨', N'نقابة المحامين المصرية', N'2016', N'["Phone","InOffice"]', 2, @AdminId, '2026-04-16T12:10:00', NULL, NULL, 4, N'[1,2,3,4]', '2026-04-16T12:10:00', 4.6, 1, 300.00, 450.00);
END;

IF NOT EXISTS (SELECT 1 FROM [Specializations] WHERE [Name] = N'القانون التجاري')
BEGIN
    INSERT INTO [Specializations] ([Name], [Description], [IsActive], [CreatedById], [CreatedOn], [UpdatedById], [UpdatedOn])
    VALUES (N'القانون التجاري', N'يشمل تأسيس الشركات والعقود التجارية وتسوية المنازعات بين التجار.', 1, @AdminId, '2026-04-16T12:20:00', NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [Specializations] WHERE [Name] = N'الأحوال الشخصية')
BEGIN
    INSERT INTO [Specializations] ([Name], [Description], [IsActive], [CreatedById], [CreatedOn], [UpdatedById], [UpdatedOn])
    VALUES (N'الأحوال الشخصية', N'يشمل الطلاق والنفقة والحضانة وإثبات الزواج والولاية.', 1, @AdminId, '2026-04-16T12:21:00', NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [Specializations] WHERE [Name] = N'قانون العمل')
BEGIN
    INSERT INTO [Specializations] ([Name], [Description], [IsActive], [CreatedById], [CreatedOn], [UpdatedById], [UpdatedOn])
    VALUES (N'قانون العمل', N'يشمل عقود العمل والتعويضات والفصل التعسفي وتسوية النزاعات العمالية.', 1, @AdminId, '2026-04-16T12:22:00', NULL, NULL);
END;

SELECT @SpecializationCorporateId = [Id] FROM [Specializations] WHERE [Name] = N'القانون التجاري';
SELECT @SpecializationFamilyId = [Id] FROM [Specializations] WHERE [Name] = N'الأحوال الشخصية';
SELECT @SpecializationLaborId = [Id] FROM [Specializations] WHERE [Name] = N'قانون العمل';

IF NOT EXISTS (SELECT 1 FROM [LawyerSpecializations] WHERE [LawyerId] = @Lawyer1Id AND [SpecializationId] = @SpecializationCorporateId)
BEGIN
    INSERT INTO [LawyerSpecializations] ([LawyerId], [SpecializationId])
    VALUES (@Lawyer1Id, @SpecializationCorporateId);
END;

IF NOT EXISTS (SELECT 1 FROM [LawyerSpecializations] WHERE [LawyerId] = @Lawyer1Id AND [SpecializationId] = @SpecializationLaborId)
BEGIN
    INSERT INTO [LawyerSpecializations] ([LawyerId], [SpecializationId])
    VALUES (@Lawyer1Id, @SpecializationLaborId);
END;

IF NOT EXISTS (SELECT 1 FROM [LawyerSpecializations] WHERE [LawyerId] = @Lawyer2Id AND [SpecializationId] = @SpecializationFamilyId)
BEGIN
    INSERT INTO [LawyerSpecializations] ([LawyerId], [SpecializationId])
    VALUES (@Lawyer2Id, @SpecializationFamilyId);
END;

IF NOT EXISTS (SELECT 1 FROM [LawyerSpecializations] WHERE [LawyerId] = @Lawyer2Id AND [SpecializationId] = @SpecializationLaborId)
BEGIN
    INSERT INTO [LawyerSpecializations] ([LawyerId], [SpecializationId])
    VALUES (@Lawyer2Id, @SpecializationLaborId);
END;

IF NOT EXISTS (SELECT 1 FROM [AcademicQualifications] WHERE [Id] = @Lawyer1QualificationId)
BEGIN
    INSERT INTO [AcademicQualifications] ([Id], [LawyerId], [DegreeType], [FieldOfStudy], [UniversityName], [GraduationYear], [DocumentId])
    VALUES (@Lawyer1QualificationId, @Lawyer1Id, N'بكالوريوس', N'القانون التجاري', N'جامعة القاهرة', 2016, @Lawyer1DegreeFileId);
END;

IF NOT EXISTS (SELECT 1 FROM [AcademicQualifications] WHERE [Id] = @Lawyer2QualificationId)
BEGIN
    INSERT INTO [AcademicQualifications] ([Id], [LawyerId], [DegreeType], [FieldOfStudy], [UniversityName], [GraduationYear], [DocumentId])
    VALUES (@Lawyer2QualificationId, @Lawyer2Id, N'ماجستير', N'الأحوال الشخصية', N'جامعة المنصورة', 2015, @Lawyer2DegreeFileId);
END;

IF NOT EXISTS (SELECT 1 FROM [ProfessionalCertifications] WHERE [Id] = @Lawyer1CertificationId)
BEGIN
    INSERT INTO [ProfessionalCertifications] ([Id], [LawyerId], [CertificateName], [IssuingOrganization], [YearObtained], [DocumentId])
    VALUES (@Lawyer1CertificationId, @Lawyer1Id, N'شهادة التحكيم التجاري', N'مركز القاهرة الإقليمي للتحكيم التجاري الدولي', 2020, @Lawyer1CertificateFileId);
END;

IF NOT EXISTS (SELECT 1 FROM [ProfessionalCertifications] WHERE [Id] = @Lawyer2CertificationId)
BEGIN
    INSERT INTO [ProfessionalCertifications] ([Id], [LawyerId], [CertificateName], [IssuingOrganization], [YearObtained], [DocumentId])
    VALUES (@Lawyer2CertificationId, @Lawyer2Id, N'شهادة الوساطة الأسرية', N'المركز العربي للتدريب القانوني', 2019, @Lawyer2CertificateFileId);
END;

IF NOT EXISTS (SELECT 1 FROM [WorkExperiences] WHERE [Id] = @Lawyer1WorkExperienceId)
BEGIN
    INSERT INTO [WorkExperiences] ([Id], [LawyerId], [JobTitle], [OrganizationName], [StartYear], [EndYear], [IsCurrentJob], [Description])
    VALUES (@Lawyer1WorkExperienceId, @Lawyer1Id, N'محام أول', N'مكتب الريادة للمحاماة', 2018, NULL, 1, N'يتولى التفاوض في العقود التجارية ومراجعة الامتثال القانوني للشركات.');
END;

IF NOT EXISTS (SELECT 1 FROM [WorkExperiences] WHERE [Id] = @Lawyer2WorkExperienceId)
BEGIN
    INSERT INTO [WorkExperiences] ([Id], [LawyerId], [JobTitle], [OrganizationName], [StartYear], [EndYear], [IsCurrentJob], [Description])
    VALUES (@Lawyer2WorkExperienceId, @Lawyer2Id, N'محامية استشارات', N'مؤسسة العدل للاستشارات القانونية', 2016, NULL, 1, N'تقدم استشارات في النزاعات الأسرية والعمالية وتدير جلسات الصلح قبل التقاضي.');
END;

IF NOT EXISTS (SELECT 1 FROM [VerificationDocuments] WHERE [Id] = @Lawyer1VerificationNationalFrontId)
BEGIN
    INSERT INTO [VerificationDocuments] ([Id], [LawyerId], [FileId], [Type])
    VALUES (@Lawyer1VerificationNationalFrontId, @Lawyer1Id, @Lawyer1NationalFrontFileId, N'NationalIdFront');
END;

IF NOT EXISTS (SELECT 1 FROM [VerificationDocuments] WHERE [Id] = @Lawyer1VerificationNationalBackId)
BEGIN
    INSERT INTO [VerificationDocuments] ([Id], [LawyerId], [FileId], [Type])
    VALUES (@Lawyer1VerificationNationalBackId, @Lawyer1Id, @Lawyer1NationalBackFileId, N'NationalIdBack');
END;

IF NOT EXISTS (SELECT 1 FROM [VerificationDocuments] WHERE [Id] = @Lawyer1VerificationLicenseId)
BEGIN
    INSERT INTO [VerificationDocuments] ([Id], [LawyerId], [FileId], [Type])
    VALUES (@Lawyer1VerificationLicenseId, @Lawyer1Id, @Lawyer1LicenseFileId, N'LawyerLicense');
END;

IF NOT EXISTS (SELECT 1 FROM [VerificationDocuments] WHERE [Id] = @Lawyer1VerificationCertificateId)
BEGIN
    INSERT INTO [VerificationDocuments] ([Id], [LawyerId], [FileId], [Type])
    VALUES (@Lawyer1VerificationCertificateId, @Lawyer1Id, @Lawyer1CertificateFileId, N'ProfessionalCertificate');
END;

IF NOT EXISTS (SELECT 1 FROM [VerificationDocuments] WHERE [Id] = @Lawyer2VerificationLicenseId)
BEGIN
    INSERT INTO [VerificationDocuments] ([Id], [LawyerId], [FileId], [Type])
    VALUES (@Lawyer2VerificationLicenseId, @Lawyer2Id, @Lawyer2LicenseFileId, N'LawyerLicense');
END;

IF NOT EXISTS (SELECT 1 FROM [VerificationDocuments] WHERE [Id] = @Lawyer2VerificationCertificateId)
BEGIN
    INSERT INTO [VerificationDocuments] ([Id], [LawyerId], [FileId], [Type])
    VALUES (@Lawyer2VerificationCertificateId, @Lawyer2Id, @Lawyer2CertificateFileId, N'ProfessionalCertificate');
END;

IF NOT EXISTS (
    SELECT 1
    FROM [AppointmentSlots]
    WHERE [LawyerId] = @Lawyer1Id
      AND [Date] = '2026-04-20'
      AND [StartTime] = '10:00:00'
      AND [EndTime] = '10:45:00'
)
BEGIN
    INSERT INTO [AppointmentSlots] ([LawyerId], [Date], [StartTime], [EndTime], [SessionType], [CreatedById], [CreatedOn], [UpdatedById], [UpdatedOn])
    VALUES (@Lawyer1Id, '2026-04-20', '10:00:00', '10:45:00', 0, @AdminId, '2026-04-16T13:00:00', NULL, NULL);
END;

IF NOT EXISTS (
    SELECT 1
    FROM [AppointmentSlots]
    WHERE [LawyerId] = @Lawyer2Id
      AND [Date] = '2026-04-21'
      AND [StartTime] = '18:00:00'
      AND [EndTime] = '18:30:00'
)
BEGIN
    INSERT INTO [AppointmentSlots] ([LawyerId], [Date], [StartTime], [EndTime], [SessionType], [CreatedById], [CreatedOn], [UpdatedById], [UpdatedOn])
    VALUES (@Lawyer2Id, '2026-04-21', '18:00:00', '18:30:00', 1, @AdminId, '2026-04-16T13:05:00', NULL, NULL);
END;

SELECT @Slot1Id = [Id]
FROM [AppointmentSlots]
WHERE [LawyerId] = @Lawyer1Id
  AND [Date] = '2026-04-20'
  AND [StartTime] = '10:00:00'
  AND [EndTime] = '10:45:00';

SELECT @Slot2Id = [Id]
FROM [AppointmentSlots]
WHERE [LawyerId] = @Lawyer2Id
  AND [Date] = '2026-04-21'
  AND [StartTime] = '18:00:00'
  AND [EndTime] = '18:30:00';

IF NOT EXISTS (SELECT 1 FROM [Appointments] WHERE [Id] = @Appointment1Id)
BEGIN
    INSERT INTO [Appointments] ([Id], [SlotId], [ClientId], [LawyerId], [Status], [CreatedAt], [ConfirmedAt], [CancelledAt], [CompletedAt])
    VALUES (@Appointment1Id, @Slot1Id, @ClientId, @Lawyer1Id, 3, '2026-04-16T14:00:00', '2026-04-16T14:10:00', NULL, '2026-04-20T10:45:00');
END;

IF NOT EXISTS (SELECT 1 FROM [Appointments] WHERE [Id] = @Appointment2Id)
BEGIN
    INSERT INTO [Appointments] ([Id], [SlotId], [ClientId], [LawyerId], [Status], [CreatedAt], [ConfirmedAt], [CancelledAt], [CompletedAt])
    VALUES (@Appointment2Id, @Slot2Id, @ClientId, @Lawyer2Id, 3, '2026-04-16T14:05:00', '2026-04-16T14:15:00', NULL, '2026-04-21T18:30:00');
END;

IF NOT EXISTS (SELECT 1 FROM [FavoriteLawyers] WHERE [UserId] = @ClientId AND [LawyerId] = @Lawyer1Id)
BEGIN
    INSERT INTO [FavoriteLawyers] ([UserId], [LawyerId])
    VALUES (@ClientId, @Lawyer1Id);
END;

IF NOT EXISTS (SELECT 1 FROM [FavoriteLawyers] WHERE [UserId] = @ClientId AND [LawyerId] = @Lawyer2Id)
BEGIN
    INSERT INTO [FavoriteLawyers] ([UserId], [LawyerId])
    VALUES (@ClientId, @Lawyer2Id);
END;

IF NOT EXISTS (SELECT 1 FROM [Reviews] WHERE [Id] = @Review1Id)
BEGIN
    INSERT INTO [Reviews] ([Id], [UserId], [LawyerId], [AppointmentId], [Rating], [Comment], [CreatedAt], [AiAnalysis_IsFlagged], [AiAnalysis_Confidence], [AiAnalysis_Summary])
    VALUES (@Review1Id, @ClientId, @Lawyer1Id, @Appointment1Id, 5.0, N'تجربة ممتازة، شرح المحامي التفاصيل القانونية بوضوح وساعدني في فهم خياراتي.', '2026-04-20T11:00:00', 0, 0.98, N'مراجعة إيجابية للغاية ولا تحتوي على محتوى مسيء.');
END;

IF NOT EXISTS (SELECT 1 FROM [Reviews] WHERE [Id] = @Review2Id)
BEGIN
    INSERT INTO [Reviews] ([Id], [UserId], [LawyerId], [AppointmentId], [Rating], [Comment], [CreatedAt], [AiAnalysis_IsFlagged], [AiAnalysis_Confidence], [AiAnalysis_Summary])
    VALUES (@Review2Id, @ClientId, @Lawyer2Id, @Appointment2Id, 4.0, N'الاستشارة مفيدة وكانت الإجابات دقيقة، وأرغب في متابعة القضية معها.', '2026-04-21T19:00:00', 0, 0.94, N'مراجعة جيدة مع ملاحظات إيجابية عن جودة الاستشارة القانونية.');
END;

IF NOT EXISTS (SELECT 1 FROM [SystemReviews] WHERE [Id] = @SystemReviewId)
BEGIN
    INSERT INTO [SystemReviews] ([Id], [UserId], [Rating], [Comment], [CreatedAt], [AiAnalysis_IsFlagged], [AiAnalysis_Confidence], [AiAnalysis_Summary])
    VALUES (@SystemReviewId, @ClientId, 5.0, N'المنصة سهلة الاستخدام وحجز الموعد تم بسرعة وبدون تعقيد.', '2026-04-21T19:15:00', 0, 0.97, N'انطباع إيجابي عن تجربة استخدام المنصة وسهولة الحجز.');
END;

IF NOT EXISTS (SELECT 1 FROM [EmailOtps] WHERE [Email] = N'leila.hassan@wakiliy.test' AND [Code] = N'654321' AND [Purpose] = 1)
BEGIN
    INSERT INTO [EmailOtps] ([Email], [Code], [ExpireAt], [IsUsed], [Purpose])
    VALUES (N'leila.hassan@wakiliy.test', N'654321', '2026-04-22T10:00:00', 0, 1);
END;

IF NOT EXISTS (SELECT 1 FROM [EmailOtps] WHERE [Email] = N'ahmed.elshafie@wakiliy.test' AND [Code] = N'112233' AND [Purpose] = 2)
BEGIN
    INSERT INTO [EmailOtps] ([Email], [Code], [ExpireAt], [IsUsed], [Purpose])
    VALUES (N'ahmed.elshafie@wakiliy.test', N'112233', '2026-04-22T10:05:00', 0, 2);
END;

COMMIT TRANSACTION;

select * from AspNetUsers