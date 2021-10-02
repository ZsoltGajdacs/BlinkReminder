using System;

namespace BRCore.Update.DTO
{
    public class UpdateResultDto
    {
        public bool IsSuccessfulUpdateCheck { get; private set; }
        public string ErrorMessage { get; private set; }
        public string UpdateLink { get; private set; }

        public UpdateResultDto(bool isSuccessfulUpdateCheck, string errorMessage, string updateLink)
        {
            IsSuccessfulUpdateCheck = isSuccessfulUpdateCheck;
            ErrorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
            UpdateLink = updateLink ?? throw new ArgumentNullException(nameof(updateLink));
        }
    }
}
