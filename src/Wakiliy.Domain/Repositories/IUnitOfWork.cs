using Wakiliy.Application.Repositories;
using Wakiliy.Domain.Repositories;

namespace Wakiliy.Domain.Repositories;

public interface IUnitOfWork
{
    IAppointmentRepository Appointments { get; }
    IAppointmentSlotRepository AppointmentSlots { get; }
    ILawyerRepository Lawyers { get; }
    IClientRepository Clients { get; }
    ISpecializationRepository Specializations { get; }
    IEmailOtpRepository EmailOtps { get; }
    IUploadedFileRepository UploadedFiles { get; }
    IAcademicQualificationRepository AcademicQualifications { get; }
    IProfessionalCertificationRepository ProfessionalCertifications { get; }
    IVerificationDocumentRepository VerificationDocuments { get; }
    IAdminRepository Admins { get; }
    IFavoriteLawyerRepository FavoriteLawyers { get; }
    IUserRepository Users { get; }
    IBookingIntentRepository BookingIntents { get; }
    IPaymentTransactionRepository PaymentTransactions { get; }
    IReviewRepository Reviews { get; }
    ILawyerEarningRepository LawyerEarnings { get; }
    IPayrollRepository Payrolls { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
