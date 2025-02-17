namespace Store.Services
{
    public class FileManagerService : IFileManagerService
    {
        private readonly IWebHostEnvironment _env;
        // In order to locate wwwroot
        private readonly IHttpContextAccessor _httpContextAccessor;
        // To know the server configuration to build the image url

        public FileManagerService(IWebHostEnvironment env,
            IHttpContextAccessor httpContextAccessor)
        {
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task DeleteFile(string route, string folder)
        {
            if (route != null)
            {
                var fileName = Path.GetFileName(route);
                string fileDirectory = Path.Combine(_env.WebRootPath, folder, fileName);

                if (File.Exists(fileDirectory))
                {
                    File.Delete(fileDirectory);
                }
            }

            return Task.FromResult(0);
        }

        public async Task<string> EditFile(byte[] content, string extension, string folder, string route,
            string contentType)
        {
            await DeleteFile(route, folder);
            return await SaveFile(content, extension, folder, contentType);
        }

        public async Task<string> SaveFile(byte[] content, string extension, string folder,
            string contentType)
        {
            // We create a random name with the extension
            var fileName = $"{Guid.NewGuid()}{extension}";
            // The route will be wwwroot/img
            string folderName = Path.Combine(_env.WebRootPath, folder);

            // If the folder does not exist we create it
            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }

            // The path where we will leave the file will be the concatenation of the folder path and the file name.
            string route = Path.Combine(folderName, fileName);
            // We save the file
            await File.WriteAllBytesAsync(route, content);

            // The image url will be http or https://domain/folder/imageName
            var currentUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            var urlForBD = Path.Combine(currentUrl, folder, fileName).Replace("\\", "/");
            return urlForBD;
        }
    }
}
