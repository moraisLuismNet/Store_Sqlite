using System.ComponentModel.DataAnnotations;

namespace Store.Validators
{
    public class WeightFileValidation : ValidationAttribute
    {
        private readonly int maximumWeightInMegaBytes;

        public WeightFileValidation(int MaximumWeightInMegaBytes)
        {
            maximumWeightInMegaBytes = MaximumWeightInMegaBytes;
        }

        // value represents the file
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            // IFormFile is the data as it comes from the post
            IFormFile formFile = value as IFormFile;

            // If it is null we give the ok to save (in this case) the product without image
            if (formFile == null)
            {
                return ValidationResult.Success;
            }

            // If it exceeds the size we return an error
            if (formFile.Length > maximumWeightInMegaBytes * 1024 * 1024)
            {
                return new ValidationResult($"The file weight must not be greater than {maximumWeightInMegaBytes}mb");
            }

            // If we have come this far, it means that everything has gone well and the file complies with the size specified in the DTO.
            return ValidationResult.Success;
        }
    }
}