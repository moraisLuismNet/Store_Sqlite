using System.ComponentModel.DataAnnotations;

namespace Store.Validators
{
    public class ValidationFileType : ValidationAttribute
    {
        private readonly string[] validTypes;

        public ValidationFileType(string[] validTypes)
        {
            this.validTypes = validTypes;
        }

        // From the DTO we specify what type of file we are going to choose (in this case image)
        public ValidationFileType(GroupFileType groupFileType)
        {
            if (groupFileType == GroupFileType.Image)
            {
                validTypes = new string[] { "image/jpeg", "image/png", "image/gif" };
            }
        }

        // value represents the file
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            // IFormFile is the data as it comes in from the post

            IFormFile formFile = value as IFormFile;

            if (formFile == null)
            {
                return ValidationResult.Success;
            }

            // ContentType must be one of the valid types { "image/jpeg", "image/png", "image/gif" } to pass validation
            if (!validTypes.Contains(formFile.ContentType))
            {
                return new ValidationResult($"The file type must be one of the following: {string.Join(", ", validTypes)}");
            }

            return ValidationResult.Success;
        }
    }
}