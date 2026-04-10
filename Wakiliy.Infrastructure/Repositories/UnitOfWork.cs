using Wakiliy.Application.Repositories;
using Wakiliy.Domain.Repositories;
using Wakiliy.Infrastructure.Data;

namespace Wakiliy.Infrastructure.Repositories;

internal class UnitOfWork(
    ApplicationDbContext dbContext,
    IAppointmentRepository appointments,
    IAppointmentSlotRepository appointmentSlots,
    ILawyerRepository lawyers,
    IClientRepository clients,
    ISpecializationRepository specializations,
    IEmailOtpRepository emailOtps,
    IUploadedFileRepository uploadedFiles,
    IAcademicQualificationRepository academicQualifications,
    IProfessionalCertificationRepository professionalCertifications,
    IVerificationDocumentRepository verificationDocuments,
    IAdminRepository admins,
    IFavoriteLawyerRepository favoriteLawyers,
    IUserRepository users,
    IReviewRepository reviews,
    ISystemReviewRepository systemReviews) : IUnitOfWork
{
    public IAppointmentRepository Appointments => appointments;
    public IAppointmentSlotRepository AppointmentSlots => appointmentSlots;
    public ILawyerRepository Lawyers => lawyers;
    public IClientRepository Clients => clients;
    public ISpecializationRepository Specializations => specializations;
    public IEmailOtpRepository EmailOtps => emailOtps;
    public IUploadedFileRepository UploadedFiles => uploadedFiles;
    public IAcademicQualificationRepository AcademicQualifications => academicQualifications;
    public IProfessionalCertificationRepository ProfessionalCertifications => professionalCertifications;
    public IVerificationDocumentRepository VerificationDocuments => verificationDocuments;
    public IAdminRepository Admins => admins;
    public IFavoriteLawyerRepository FavoriteLawyers => favoriteLawyers;
    public IUserRepository Users => users;
    public IReviewRepository Reviews => reviews;
    public ISystemReviewRepository SystemReviews => systemReviews;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await dbContext.SaveChangesAsync(cancellationToken);
}
