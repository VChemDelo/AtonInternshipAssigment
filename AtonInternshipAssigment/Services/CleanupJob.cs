using AtonInternshipAssigment.Repositories;
using Quartz;

namespace AtonInternshipAssigment.Services
{
    public class CleanupJob : IJob
    {
        private readonly UserRepository _userRepository;
        private readonly ILogger<CleanupJob> _logger;

        public CleanupJob(UserRepository userRepository, ILogger<CleanupJob> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Запуск задачи очистки...");
            await _userRepository.HardDeleteUsersQuartz();
            _logger.LogInformation("Очистка завершена.");
        }
    }
}
